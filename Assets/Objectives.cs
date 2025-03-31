using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    public TextMeshProUGUI Baron;
    public TextMeshProUGUI Dragon;
    public TextMeshProUGUI Herald;
    public TextMeshProUGUI Inhib;
    public TextMeshProUGUI Horde;
    public TextMeshProUGUI Atakhan;
    public TextMeshProUGUI Turret;
    public TextMeshProUGUI Gold;
    public TextMeshProUGUI Team;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateData(int Baron, int Dragon, int Herald, int Inhib, int Horde, int Atakhan, int Turret)
    {
        this.Baron.text = Baron.ToString();
        this.Dragon.text = Dragon.ToString();
        this.Herald.text = Herald.ToString();
        this.Inhib.text = Inhib.ToString();
        this.Horde.text = Horde.ToString();
        this.Atakhan.text = Atakhan.ToString();
        this.Turret.text = Turret.ToString();
    }
}
