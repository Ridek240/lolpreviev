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
using UnityEngine.UI;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;


public class Connection : MonoBehaviour
{

    public static string Api_Key;
    public string _Api_Key;
    List<PlayerElement> playerElements = new List<PlayerElement>();
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI LeftTeam;
    public TextMeshProUGUI RightTeam;
    public Objectives BlueSide;
    public Objectives RedSide;
    public static Connection Instance;
    public Image Table;
    public Camera environment;
    public Dictionary<string, List<string>> PlayersInTeams = new Dictionary<string, List<string>>();
    public List<TextMeshProUGUI> gui = new List<TextMeshProUGUI>();
    public List<Image> images = new List<Image>();
    public List<Image> banns = new List<Image>();
    public Dictionary<int, string> IdToChamps = new Dictionary<int, string>();
    public bool[] Ban_Availbe = new bool[10];

    Dictionary<int, int> BlueSideNubers = new Dictionary<int, int>
        {
            { 1, 0 },
            { 3, 1 },
            { 5, 2 },
            { 2, 3 },
            { 4, 4 }
        };
    Dictionary<int, int> RedSideNubers = new Dictionary<int, int>
        {
            { 2, 5 },
            { 4, 6 },
            { 6, 7 },
            { 1, 8 },
            { 3, 9 }
        };
// Start is called once before the first execution of Update after the MonoBehaviour is created


void StartUP()
    {
        string filePath = "C:\\tournament\\Settings.json";

        // Sprawdzamy, czy plik istnieje
        if (File.Exists(filePath))
        {
            // Wczytujemy zawartoœæ pliku
            string fileContent = File.ReadAllText(filePath);
            JObject playerInfo = JObject.Parse(fileContent);


            Material mat = Table.material;
            ColorUtility.TryParseHtmlString($"#{playerInfo["PrimaryColor"]}", out Color PrimaryColor);
            ColorUtility.TryParseHtmlString($"#{playerInfo["SecondaryColor"]}", out Color SecondaryColor);
            ColorUtility.TryParseHtmlString($"#{playerInfo["KeyColor"]}", out Color KeyColor);
            mat.SetColor("_ReplaceColor1", PrimaryColor);
            mat.SetColor("_ReplaceColor2", SecondaryColor);
            environment.backgroundColor = KeyColor;

            foreach (var playerElement in Menager.Instance.list)
            {
                playerElement.SetElementsColor(PrimaryColor);
            }
            foreach(var el in gui)
            {
                el.color = PrimaryColor;
            }
            foreach(var el in images)
            {
                el.material.SetColor("_Color1", PrimaryColor);
            }
        }
    }
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
            PlayersInTeams.Clear();
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
    async void Start()
    {
        StartUP();
        Api_Key = _Api_Key;
        LoadApiKey();
        //LoadData();
        IdToChamps = await GetChampionDictionaryAsync();

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
        LoadData();
        Debug.Log($"SZukam meczu dla {matchid}");
        playerElements.Clear();
        string api_url = $"https://europe.api.riotgames.com/lol/match/v5/matches/{matchid}?api_key={Api_Key}";
        UnityWebRequest request = UnityWebRequest.Get(api_url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error fetching player: {request.error}");
            if(request.responseCode == 403)
            {
                HttpServer.assignedData = "Api Key Error";
            }
            if (request.responseCode == 404)
            {
                HttpServer.assignedData = "MatchId Error";
            }
            yield break;
        }
        HttpServer.assignedData = "OK";

        JObject playerInfo = JObject.Parse(request.downloadHandler.text);
        string jsonString = playerInfo.ToString(Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText("test3.json", jsonString);


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
                Champion = IdToChamps[item["championId"].Value<int>()],
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
        i = 0;
        foreach (var frame in banns)
        {
            frame.sprite = Resources.Load<Sprite>($"Champions/None");
        }
        foreach (var item in teams)
        {
            Objectives _object;
            Dictionary<int, int> dict;
            if (item["teamId"].Value<int>() == 100)
            {
                dict = BlueSideNubers;
                _object = BlueSide;
                LeftTeam.text = item["win"].Value<bool>() ? "WIN" : "LOSE";
                
            }
            if (item["teamId"].Value<int>() == 200)
            {
                _object = RedSide;
                dict = RedSideNubers;
                RightTeam.text = item["win"].Value<bool>() ? "WIN" : "LOSE";
            }
            else
            {
                _object = BlueSide;
                dict = BlueSideNubers;
            }

            var obj = item["objectives"];
            _object.Baron.text = obj["baron"]["kills"].Value<int>().ToString();
            _object.Dragon.text = obj["dragon"]["kills"].Value<int>().ToString();
            _object.Atakhan.text = obj["atakhan"]["kills"].Value<int>().ToString();
            _object.Horde.text = obj["horde"]["kills"].Value<int>().ToString();
            _object.Inhib.text = obj["inhibitor"]["kills"].Value<int>().ToString();
            _object.Herald.text = obj["riftHerald"]["kills"].Value<int>().ToString();
            _object.Turret.text = obj["tower"]["kills"].Value<int>().ToString();

            foreach (var ban in item["bans"])
            {

                banns[dict[ban["pickTurn"].Value<int>()]].sprite = Resources.Load<Sprite>($"Champions/{CleanChampionName(IdToChamps[ban["championId"].Value<int>()])}");
                i++;
            }

                
        }

        
    }

    void Settime(int timestamp)
    {
        int minutes = (int)(timestamp / 60);
        int seconds = (int)(timestamp % 60);
        TimeText.text = $"{minutes}:{seconds:D2}";
    }

    public static string CleanChampionName(string name)
    {
        // Usuwa wszystkie znaki niebêd¹ce literami lub cyframi
        var chars = name.ToCharArray();
        var cleanChars = Array.FindAll(chars, c => char.IsLetterOrDigit(c));
        return new string(cleanChars);
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

    public static async Task<Dictionary<int, string>> GetChampionDictionaryAsync()
    {

        
        using HttpClient client = new HttpClient();
        string versionJson = await client.GetStringAsync("https://ddragon.leagueoflegends.com/api/versions.json");
        List<string> versions = JsonConvert.DeserializeObject<List<string>>(versionJson);
        string latestVersion = versions[0];
        string url = $"https://ddragon.leagueoflegends.com/cdn/{latestVersion}/data/en_US/champion.json";

        string json = await client.GetStringAsync(url);

        JObject root = JObject.Parse(json);
        JObject data = (JObject)root["data"];

        var championDict = new Dictionary<int, string>();

        foreach (var champ in data.Properties())
        {
            int id = int.Parse(champ.Value["key"].ToString());
            string name = champ.Value["name"].ToString();
            championDict[id] = name;
        }

        return championDict;
    }

    async Task DownloadAllChampionIcons()
    {
        using HttpClient client = new HttpClient();

        string VersionsUrl = "https://ddragon.leagueoflegends.com/api/versions.json";
        string ChampionListUrlTemplate = "https://ddragon.leagueoflegends.com/cdn/{0}/data/en_US/champion.json";
        string ChampionIconUrlTemplate = "https://ddragon.leagueoflegends.com/cdn/{0}/img/champion/{1}.png";
        string version = "latest";

    // Pobierz najnowsz¹ wersjê
    string versionJson = await client.GetStringAsync(VersionsUrl);
        JArray versions = JArray.Parse(versionJson);
        version = versions[0].ToString();

        // Pobierz listê championów
        string champListUrl = string.Format(ChampionListUrlTemplate, version);
        string champJson = await client.GetStringAsync(champListUrl);
        JObject data = JObject.Parse(champJson)["data"] as JObject;

        // Utwórz folder jeœli nie istnieje
        string folderPath = Path.Combine(Application.dataPath, "Resources/Champions");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        foreach (var champ in data.Properties())
        {
            string champName = champ.Name;
            string iconUrl = string.Format(ChampionIconUrlTemplate, version, champName);
            string savePath = Path.Combine(folderPath, $"{champName}.png");

            if (File.Exists(savePath)) continue; // nie pobieraj ponownie

            byte[] iconData = await client.GetByteArrayAsync(iconUrl);
            File.WriteAllBytes(savePath, iconData);

            Debug.Log($"Pobrano ikonê: {champName}");
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
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
