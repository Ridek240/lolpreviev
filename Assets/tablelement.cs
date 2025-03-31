using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class tablelement : MonoBehaviour
{
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI Champion;
    public TextMeshProUGUI KDA;
    public TextMeshProUGUI KDR;
    public TextMeshProUGUI KP;
    public TextMeshProUGUI DMG;
    public TextMeshProUGUI VS;
    public TextMeshProUGUI VSM;
    public TextMeshProUGUI Minion;
    public TextMeshProUGUI GPM;

    public Image ChampionIcon;


    public void SetField(PlayerElement element)
    {
        PlayerName.text = element.PlayerName;
        Champion.text = element.Champion;
        KDA.text = $"{element.kills}/{element.deaths}/{element.assists}";
        KDR.text = $"{(element.kills + element.assists) / (float)element.deaths :F2}";
        KP.text = $"{element.KillPartipitation * 100:F2}%";
        DMG.text = element.Damage.ToString();
        VS.text = element.VisionScore.ToString();
        VSM.text = $"{element.VisionScoreMinute:F2}";
        Minion.text = element.Minions.ToString();
        GPM.text = element.GoldPerMinute.ToString();
        ChampionIcon.sprite = Resources.Load<Sprite>($"LoLSmall/{element.Champion}");
    }
}
