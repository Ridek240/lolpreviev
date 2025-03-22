using UnityEngine;

public class PlayerElement
{
    public string PlayerName;
    public string Champion;
    public int kills;
    public int deaths;
    public int assists;
    public float KillPartipitation;
    public int Damage;
    public int VisionScore;
    public float VisionScoreMinute;
    public int Minions;
    public int GoldPerMinute;
    public Team Team;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum Team
{
    None,
    Blue,
    Red
}
