using System;
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

                // Determine the requested route and handle accordingly
                string responseString = HandleRequest(request.Url.AbsolutePath);
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
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

    private string HandleRequest(string path)
    {
        switch (path)
        {
            case "/":
                return "<html><body>Home Page</body></html>";

            case "/init":
                // Your /init route handling logic here
                return "<html><body>Initialization Route</body></html>";

            default:
                return "<html><body>404 Not Found</body></html>";
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
}

