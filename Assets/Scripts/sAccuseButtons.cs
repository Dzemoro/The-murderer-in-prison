using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sAccuseButtons : MonoBehaviour
{

    public GameObject scrollViewContent;
    public GameObject buttonTemplate;

    // Start is called before the first frame update
    void Start()
    {
        int i = -20;

        GameObject[] _allPrisoners = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject _tempPrisoner in _allPrisoners)
        {
            GameObject button = (GameObject)Instantiate(buttonTemplate);
            button.transform.SetParent(scrollViewContent.transform, false);

            //I don't know why it needs to be these magic numbers but it finally works so who am I to judge?
            button.transform.position = new Vector2(button.transform.position.x, button.transform.position.y + i);
            i -= 50;

            button.GetComponent<sAccuse>()._name = _tempPrisoner.GetComponent<sNPC>().GetNPCName();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
