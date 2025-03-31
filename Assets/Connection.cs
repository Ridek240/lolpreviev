using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using TMPro;

public class Connection : MonoBehaviour
{

    public static string Api_Key;
    public string _Api_Key;
    List<PlayerElement> playerElements = new List<PlayerElement>();
    public TextMeshProUGUI TimeText;
    public Objectives BlueSide;
    public Objectives RedSide;
    public static Connection Instance;
    public Dictionary<string, List<string>> PlayersInTeams = new Dictionary<string, List<string>>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void LoadApiKey()
    {
        // Œcie¿ka do pliku (przyk³ad - zmieñ na w³asn¹ œcie¿kê)
        string filePath = "C:\\tournament\\ApiKey.txt";

        // Sprawdzamy, czy plik istnieje
        if (File.Exists(filePath))
        {
            // Wczytujemy zawartoœæ pliku
            string fileContent = File.ReadAllText(filePath);
            Api_Key = fileContent;
            Debug.Log("File Content: " + fileContent);
        }
        else
        {
            Debug.LogError("Plik nie istnieje w podanej œcie¿ce!");
        }
    }
    void LoadData()
    {
        // Œcie¿ka do pliku (przyk³ad - zmieñ na w³asn¹ œcie¿kê)
        string filePath = "C:\\tournament\\Teams.json";

        // Sprawdzamy, czy plik istnieje
        if (File.Exists(filePath))
        {
            // Wczytujemy zawartoœæ pliku
            string fileContent = File.ReadAllText(filePath);
            JObject playerInfo = JObject.Parse(fileContent);

            foreach (var player in playerInfo["Teams"]) 
            {
                List<string> list= new List<string>();//PlayersInTeams.Find(x=>x.key == player["TeamName"].ToString());
                try
                {
                    list = PlayersInTeams[player["TeamName"].ToString()];
                }
                catch { }

                foreach (var plaername in player["Players"])
                {
                    
                    if(list != null)
                    {
                        list.Add(plaername.ToString());
                    }
                    else
                    {
                        list = new List<string>();
                        list.Add(plaername.ToString());
                    }
                }
                PlayersInTeams.Add(player["TeamName"].ToString(), list);
            }
            
        }
        else
        {
            Debug.LogError("Plik nie istnieje w podanej œcie¿ce!");
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

        Api_Key = _Api_Key;
        LoadApiKey();
        LoadData();
        //SetSystem("","","EUN1_3753654959");
        //GetDragonData();
    }
    public void SetSystem(string BlueTeam, string RedTeam, string MatchId)
    {
        LoadApiKey();
        BlueSide.Team.text = BlueTeam;
        RedSide.Team.text = RedTeam;
        StartCoroutine(GetMatch(MatchId));
    }
    public IEnumerator GetMatch(string matchid)
    {

        Debug.Log($"SZukam meczu dla {matchid}");
        playerElements.Clear();
        string api_url = $"https://europe.api.riotgames.com/lol/match/v5/matches/{matchid}?api_key={Api_Key}";
        UnityWebRequest request = UnityWebRequest.Get(api_url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching player: {request.error}");
            yield break;
        }

        JObject playerInfo = JObject.Parse(request.downloadHandler.text);
        string jsonString = playerInfo.ToString(Newtonsoft.Json.Formatting.Indented);
        //File.WriteAllText("test4timeline.json", jsonString);
        
        var sus = playerInfo["info"];

        Settime(playerInfo["info"]["gameDuration"].Value<int>());
        
        var sus2 = sus["participants"];
        int i = 0;
        int blueTeamit = 0;
        int redTeamit = 0;
        int[] gold = new int[2];
        foreach (var item in sus2)
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
                GoldPerMinute = item["challenges"]["goldPerMinute"].Value<float>(),
                Team = item["teamId"].Value<int>() == 100 ? Team.Blue : Team.Red
            };
            if (item["teamId"].Value<int>() == 100)
            {
                if (PlayersInTeams.TryGetValue(BlueSide.Team.text, out List<string> players))
                {
                    element.PlayerName = players[blueTeamit++];
                }
                gold[0] += item["goldEarned"].Value<int>();
            }
            else
            {
                if (PlayersInTeams.TryGetValue(RedSide.Team.text, out List<string> players))
                {
                    element.PlayerName = players[redTeamit++];
                }
                gold[1] += item["goldEarned"].Value<int>();
            }

            playerElements.Add(element);

            Menager.Instance.list[i].SetField(element);
            i++;
            //Debug.Log($"{.ToString()}");
            //Debug.Log($"{item["riotIdGameName"].ToString()}");
        }

        BlueSide.Gold.text = gold[0].ToString();
        RedSide.Gold.text = gold[1].ToString();
        var teams = sus["teams"];

        foreach (var item in teams)
        {
            Objectives _object;
            if (item["teamId"].Value<int>() == 100)
                _object = BlueSide;
            if (item["teamId"].Value<int>() == 200)
                _object = RedSide;
            else
                _object = BlueSide;

            var obj = item["objectives"];
            _object.Baron.text = obj["baron"]["kills"].Value<int>().ToString();
            _object.Dragon.text = obj["dragon"]["kills"].Value<int>().ToString();
            _object.Atakhan.text = obj["atakhan"]["kills"].Value<int>().ToString();
            _object.Horde.text = obj["horde"]["kills"].Value<int>().ToString();
            _object.Inhib.text = obj["inhibitor"]["kills"].Value<int>().ToString();
            _object.Herald.text = obj["riftHerald"]["kills"].Value<int>().ToString();
            _object.Turret.text = obj["tower"]["kills"].Value<int>().ToString();
                
        }

        
    }

    void Settime(int timestamp)
    {
        int minutes = (int)(timestamp / 60);
        int seconds = (int)(timestamp % 60);
        TimeText.text = $"{minutes}:{seconds}";
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
                // Akceptujemy wszystkie certyfikaty SSL (pomaga, jeœli masz problemy z certyfikatem)
                client.DefaultRequestHeaders.Add("User-Agent", "UnityClient");

                // Pobierz dane z API
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // SprawdŸ, czy odpowiedŸ jest poprawna
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
                    return "B³¹d po³¹czenia: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                return "B³¹d: " + ex.Message;
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
