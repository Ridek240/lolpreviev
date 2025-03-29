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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetField(PlayerElement element)
    {
        PlayerName.text = element.PlayerName;
        Champion.text = element.Champion;
    }
}
