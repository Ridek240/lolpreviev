using System;
using System.Net;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

public class HttpServer : MonoBehaviour
{
    private HttpListener listener;

    void Awake()
    {
        // Tworzymy nowy HttpListener, kt�ry b�dzie nas�uchiwa� na okre�lonym porcie.
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:2137/");  // Nas�uchujemy na porcie 8080
        listener.Start();
        Debug.Log("Serwer nas�uchuje na http://localhost:2137/");

        // Uruchamiamy nas�uchiwacz w osobnym w�tku.
        System.Threading.Thread listenerThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenForRequests));
        listenerThread.Start();
    }

    void ListenForRequests()
    {
        while (true)
        {
            // Czekamy na ��dania
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            // Odczytujemy dane z ��dania (np. JSON).
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
                    Debug.Log("Otrzymano dane: " + requestBody);
                    responseString = "\"Dane otrzymane\": " + requestBody;  // Mo�emy odpowiedzie� tymi danymi.
                }
            }

            // Ustawiamy nag��wki odpowiedzi i wysy�amy odpowied�.
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

    void OnApplicationQuit()
    {
        listener.Stop();  // Zatrzymujemy nas�uch po zako�czeniu aplikacji
    }
}