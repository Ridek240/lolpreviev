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
        if(element.deaths>0)
        { 
            KDR.text = $"{(element.kills + element.assists) / (float)element.deaths :F2}";
        }
        else
        {
            KDR.text = $"{(float)(element.kills + element.assists):F2}";
        }
        KP.text = $"{element.KillPartipitation * 100:F2}%";
        DMG.text = element.Damage.ToString();
        VS.text = element.VisionScore.ToString();
        VSM.text = $"{element.VisionScoreMinute:F2}";
        Minion.text = element.Minions.ToString();
        GPM.text = $"{element.GoldPerMinute:F2}";
        ChampionIcon.sprite = Resources.Load<Sprite>($"Champions/{Connection.CleanChampionName(element.Champion)}");
    }

    public void SetElementsColor(Color color)
    {
        PlayerName.color = color;
        Champion.color = color;
        KDA.color = color;
        KDR.color = color;
        KP.color = color;
        DMG.color = color;
        VS.color = color;
        VSM.color = color;
        Minion.color = color;
        GPM.color = color;

        

    }
}
