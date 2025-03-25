using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DragonSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void CheckDragons()
    {
        string apiUrl = "https://127.0.0.1:2999/liveclientdata/events";
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            // Dodajemy nag≥Ûwki, jeúli sπ wymagane
            request.SetRequestHeader("Accept", "application/json");

            // Czekamy na odpowiedü (blokuje wπtek g≥Ûwny!)
            await request.SendWebRequest();


            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;

                JObject playerInfo = JObject.Parse(request.downloadHandler.text)
            }


    }
}
