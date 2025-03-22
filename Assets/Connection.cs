using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class Connection : MonoBehaviour
{

    public static string Api_Key;
    public string _Api_Key;
    List<PlayerElement> playerElements = new List<PlayerElement>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Api_Key = _Api_Key;
        GetMatch("EUN1_3748310469");
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
        File.WriteAllText("test3.json", jsonString);
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
}
