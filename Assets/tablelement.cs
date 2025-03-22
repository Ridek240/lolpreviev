using UnityEngine;
using TMPro;

public class tablelement : MonoBehaviour
{
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI Champion;


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
