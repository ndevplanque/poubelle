using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class HttpServer : MonoBehaviour
{
    private HttpListener listener;
    private Thread listenerThread;

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://*:9876/");
        listener.Start();
        Debug.Log("HTTP Server started on port 9876");

        listenerThread = new Thread(new ThreadStart(ListenForRequests));
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    private void ListenForRequests()
    {
        while (listener.IsListening)
        {
            try
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

					default:
						Send(response, HttpStatusCode.NotFound, "Not found");
						break;
				}
            }
            catch (HttpListenerException ex)
            {
                Debug.LogError($"HttpListenerException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception: {ex.Message}");
            }
        }
    }

    void OnApplicationQuit()
    {
        if (listener != null)
        {
            listener.Stop();
            listener.Close();
        }
        if (listenerThread != null)
        {
            listenerThread.Abort();
        }
    }

	private void HandleInit(HttpListenerRequest request, HttpListenerResponse response)
	{
		using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
		{
			string body = reader.ReadToEnd();
			Debug.Log($"Received: {body}");
			
			Send(response, HttpStatusCode.OK, "ok");
		}
	}

	private void Send(HttpListenerResponse response, HttpStatusCode code, string body="")
	{
		byte[] buffer = Encoding.UTF8.GetBytes(body);
		response.ContentLength64 = buffer.Length;
		response.StatusCode = (int) code;
		var output = response.OutputStream;
		output.Write(buffer, 0, buffer.Length);
		output.Close();
	}	
}

