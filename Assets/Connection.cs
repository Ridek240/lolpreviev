using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;

public class Connection : MonoBehaviour
{

    public static string Api_Key;
    public string _Api_Key;
    List<PlayerElement> playerElements = new List<PlayerElement>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Api_Key = _Api_Key;
        GetMatch("EUN1_3753654959");
        GetDragonData();
    }

    public async void GetMatch(string matchid)
    {
        playerElements.Clear();
        string api_url = $"https://europe.api.riotgames.com/lol/match/v5/matches/{matchid}?api_key={Api_Key}";
        UnityWebRequest request = UnityWebRequest.Get(api_url);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching player: {request.error}");
            return;
        }

        JObject playerInfo = JObject.Parse(request.downloadHandler.text);
        string jsonString = playerInfo.ToString(Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText("test4timeline.json", jsonString);
        
        var sus = playerInfo["info"];
        var sus2 = sus["participants"];
        int i = 0;
        foreach(var item in sus2)
        {

            var element = new PlayerElement
            {
                Champion = item["championName"].ToString(),
                PlayerName = item["riotIdGameName"].ToString(),
                kills = item["kills"].Value<int>(),
                deaths = item["deaths"].Value<int>(),
                assists = item["assists"].Value<int>(),
                KillPartipitation = item["challenges"]["killParticipation"].Value<float>(),
                Damage = item["totalDamageDealtToChampions"].Value<int>(),
                VisionScore = item["visionScore"].Value<int>(),
                VisionScoreMinute = item["challenges"]["visionScorePerMinute"].Value<float>(),
                Minions = item["totalMinionsKilled"].Value<int>(),
                GoldPerMinute = item["challenges"]["goldPerMinute"].Value<int>(),
                Team = item["teamId"].Value<int>() == 100 ? Team.Blue : Team.Red
            };

            playerElements.Add(element);

            Menager.Instance.list[i].SetField(element);
            i++;
            //Debug.Log($"{.ToString()}");
            //Debug.Log($"{item["riotIdGameName"].ToString()}");
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private string apiUrl = "https://127.0.0.1:2999/liveclientdata/allgamedata";
    private async Task<string> GetDragonData()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Akceptujemy wszystkie certyfikaty SSL (pomaga, jeúli masz problemy z certyfikatem)
                client.DefaultRequestHeaders.Add("User-Agent", "UnityClient");

                // Pobierz dane z API
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // Sprawdü, czy odpowiedü jest poprawna
                if (response.IsSuccessStatusCode)
                {
                    // Odczytaj dane w formacie JSON
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializuj JSON do obiektu
                    GameData gameData = JsonUtility.FromJson<GameData>(jsonResponse);

                    // Przygotuj dane o smokach w formacie string
                    string dragonInfo = $"Pierwszy smok: {gameData.gameData.nextDragonType}\nDrugi smok: {gameData.gameData.secondDragonType}";

                    return dragonInfo;
                }
                else
                {
                    return "B≥πd po≥πczenia: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                return "B≥πd: " + ex.Message;
            }
        }
    }

    // Klasa do przechowywania danych gry (dostosuj do struktury JSON)
    [System.Serializable]
    public class GameData
    {
        public GameDataInfo gameData;
    }

    [System.Serializable]
    public class GameDataInfo
    {
        public string nextDragonType;
        public string secondDragonType;
    }

}
