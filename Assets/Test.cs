using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour
{

    public RectTransform element;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            //Debug.Log("Wybrany element: " + EventSystem.current.currentSelectedGameObject.name);
            element.position = EventSystem.current.currentSelectedGameObject.transform.position;
            element.sizeDelta = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta;
        }
        else
        {
            //Debug.Log("Brak wybranego elementu.");
        }
    }
}
