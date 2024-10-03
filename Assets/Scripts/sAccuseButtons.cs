using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class sAccuseButtons : MonoBehaviour
{
    public static sAccuseButtons Instance { get; private set; }

    public GameObject scrollViewContent;
    public GameObject buttonTemplate;
    private bool notCreated;

    // Start is called before the first frame update
    void Start()
    {
        notCreated = true;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateButtons()
    {
        if(notCreated)
        {
            GameObject[] _allPrisoners = GameObject.FindGameObjectsWithTag("NPC");
            foreach (GameObject _tempPrisoner in _allPrisoners)
            {
                GameObject button = (GameObject)Instantiate(buttonTemplate);
                button.transform.SetParent(scrollViewContent.transform, false);

                button.GetComponent<sAccuse>()._name = _tempPrisoner.GetComponent<sNPC>().GetNPCName();
            }
            notCreated = false;
        }
    }
}
