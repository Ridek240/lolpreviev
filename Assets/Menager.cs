using UnityEngine;
using System.Collections.Generic;
public class Menager : MonoBehaviour
{
    public List<tablelement> list = new List<tablelement>();

    public static Menager Instance;

    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;

        }
    }
}
