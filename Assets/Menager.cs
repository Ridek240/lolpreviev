using UnityEngine;
using System.Collections.Generic;
public class Menager : MonoBehaviour
{
    public List<tablelement> list = new List<tablelement>();

    public static Menager Instance;
    public Camera camera;

    private void Awake()
    {
        if(Instance==null)
        {
            Instance = this;

        }
    }
    private void Update()
    {
        camera.backgroundColor = new Color(0f,0f,0f,0f);
    }
}
