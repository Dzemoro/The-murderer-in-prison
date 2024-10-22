using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sSearch : MonoBehaviour
{

    [SerializeField] GameObject notebook;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.GetComponent<TMP_InputField>().isFocused) notebook.GetComponent<sNotebook>().searchIsActive = true;
        else notebook.GetComponent<sNotebook>().searchIsActive = false;
    }
}
