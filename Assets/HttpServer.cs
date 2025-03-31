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
        // Tworzymy nowy HttpListener, który bêdzie nas³uchiwa³ na okreœlonym porcie.
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:2137/");  // Nas³uchujemy na porcie 8080
        listener.Start();
        Debug.Log("Serwer nas³uchuje na http://localhost:2137/");

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
                    Debug.Log("Otrzymano dane: " + requestBody);
                    responseString = "\"Dane otrzymane\": " + requestBody;  // Mo¿emy odpowiedzieæ tymi danymi.
                }
            }

            // Ustawiamy nag³ówki odpowiedzi i wysy³amy odpowiedŸ.
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }

    void OnApplicationQuit()
    {
        listener.Stop();  // Zatrzymujemy nas³uch po zakoñczeniu aplikacji
    }
}