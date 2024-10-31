using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTextFieldPopup : MonoBehaviour
{

    [SerializeField] private GameObject textField;
    // Start is called before the first frame update
    void Start()
    {
        textField.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            textField.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textField.SetActive(false);
        }
    }
}
