using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class HttpServer : MonoBehaviour
{
    private HttpListener listener;
    private Thread listenerThread;
    private bool isRunning = false;
    private ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    public GameObject TrashS;
    public GameObject TrashA;
    public GameObject TrashL;

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://*:9876/");
        listener.Start();
        Debug.Log("HTTP Server started on port 9876");

        isRunning = true;
        listenerThread = new Thread(new ThreadStart(ListenForRequests));
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    private void ListenForRequests()
    {
        try
        {
            while (isRunning)
            {
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;

                switch (request.Url.AbsolutePath)
                {
                    case "/init":
                        if (request.HttpMethod == "POST") HandleInit(request, response);
                        else Send(response, HttpStatusCode.BadRequest, "Invalid method");
                        break;

                    case "/update":
                        if (request.HttpMethod == "POST") HandleUpdate(request, response);
                        else Send(response, HttpStatusCode.BadRequest, "Invalid method");
                        break;

                    default:
                        Send(response, HttpStatusCode.NotFound, "Not found");
                        break;
                }
            }
        }
        catch (HttpListenerException ex)
        {
            if (isRunning) // Ignore errors when stopping the listener
            {
                Debug.LogError($"HttpListenerException: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception: {ex.Message}");
        }
    }

    void Update()
    {
        while (mainThreadActions.TryDequeue(out var action))
        {
            action();
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (listener != null)
        {
            listener.Stop();
            listener.Close();
        }
        if (listenerThread != null && listenerThread.IsAlive)
        {
            listenerThread.Join(); // Wait for the thread to finish
        }
    }

    private void HandleInit(HttpListenerRequest request, HttpListenerResponse response)
    {
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string body = reader.ReadToEnd();

            string[] payloads = body.Split(' ');
            foreach (string payload in payloads)
            {
                mainThreadActions.Enqueue(() => UpdateTrash(payload));
            }

            Send(response, HttpStatusCode.OK, "ok");
        }
    }

    private void HandleUpdate(HttpListenerRequest request, HttpListenerResponse response)
    {
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            string body = reader.ReadToEnd();

            mainThreadActions.Enqueue(() => UpdateTrash(body));

            Send(response, HttpStatusCode.OK, "ok");
        }
    }

    private void Send(HttpListenerResponse response, HttpStatusCode code, string body = "")
    {
        byte[] buffer = Encoding.UTF8.GetBytes(body);
        response.ContentLength64 = buffer.Length;
        response.StatusCode = (int)code;
        using (var output = response.OutputStream)
        {
            output.Write(buffer, 0, buffer.Length);
        }
    }

    private void UpdateTrash(string payload)
    {
        string[] data = payload.Split(':');
        if (data.Length != 2)
        {
            throw new FormatException("Payload format is invalid.");
        }

        GameObject trash = GetTrash(data[0]);
        if (trash != null)
        {
            TrashBin trashBin = trash.GetComponent<TrashBin>();
            if (trashBin != null)
            {
                trashBin.isEmpty = GetState(data[1]);
                Debug.Log($"Trash {data[0]} set to state {data[1]}.");
            }
            else
            {
                throw new Exception($"Trash bin component not found on {data[0]}.");
            }
        }
        else
        {
            throw new Exception($"Trash object {data[0]} not found.");
        }
    }

    private GameObject GetTrash(string id)
    {
        switch (id)
        {
            case "S":
                return TrashS;
            case "A":
                return TrashA;
            case "L":
                return TrashL;
            default:
                throw new Exception("No trash was found");
        }
    }

    private bool GetState(string state)
    {
        if (state == "1")
        {
            return true;
        }
        else if (state == "0")
        {
            return false;
        }
        else
        {
            throw new FormatException("The string is not a valid boolean.");
        }
    }
}

