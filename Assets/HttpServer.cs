using System;
using System.Net;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

public class HttpServer : MonoBehaviour
{
    private HttpListener listener;
    public static string Message;
    public static string assignedData = "Hello World";
    void Awake()
    {
        // Tworzymy nowy HttpListener, który bêdzie nas³uchiwa³ na okreœlonym porcie.
        listener = new HttpListener();
        listener.Prefixes.Add("http://*:2137/");  // Nas³uchujemy na porcie 8080
        listener.Start();
        string localIP = GetLocalIPAddress();

        if (!string.IsNullOrEmpty(localIP))
        {
            Debug.Log("Serwer nas³uchuje na http://" + localIP + ":2137/");
        }

        // Uruchamiamy nas³uchiwacz w osobnym w¹tku.
        System.Threading.Thread listenerThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenForRequests));
        listenerThread.Start();
    }

    void ListenForRequests()
    {
        while (true)
        {
            // Czekamy na ¿¹dania
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
            response.Headers.Add("Pragma", "no-cache");
            response.Headers.Add("Expires", "0");


            Debug.Log("Otrzymano ¿¹danie: " + request.Url.AbsolutePath);

            // Przypisujemy dynamiczne dane
            //assignedData = "Dane przypisane: " + DateTime.Now.ToString();  // Dynamiczne dane dla ³atwiejszego testowania
            Debug.Log("Przypisano nowe dane: " + assignedData);
            // Odczytujemy dane z ¿¹dania (np. JSON).
            string responseString = "Brak danych";
            if (request.HasEntityBody)
            {
                using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string requestBody = reader.ReadToEnd();
                    JObject playerInfo = JObject.Parse(requestBody);

                    UnityMainThreadDispatcher.Execute(() =>
                    {
                        Connection.Instance.SetSystem(
                            playerInfo["BlueTeam"].ToString(),
                            playerInfo["RedTeam"].ToString(),
                            playerInfo["MatchID"].ToString()
                        );
                    });
                    responseString = $"Otrzymano dane: {assignedData}";
                }
                //responseString = Message;
            }

            // Ustawiamy nag³ówki odpowiedzi i wysy³amy odpowiedŸ.
            response.ContentType = "text/plain";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

    string GetLocalIPAddress()
    {
        string localIP = string.Empty;
        foreach (var host in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (host.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIP = host.ToString();
                break;
            }
        }
        return localIP;
    }

    void OnApplicationQuit()
    {
        listener.Stop();  // Zatrzymujemy nas³uch po zakoñczeniu aplikacji
    }
}