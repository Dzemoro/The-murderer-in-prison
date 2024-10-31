using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sItem : MonoBehaviour
{

    private bool _playerIsClose;
    public string _name;
    public string _description;
    // Start is called before the first frame update
    void Start()
    {
        _playerIsClose = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose)
        {
            sNotebook.Instance.CreateNewEntry(_name, _description);
            Destroy(gameObject);
        }
            
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsClose = false;
        }
    }
}
