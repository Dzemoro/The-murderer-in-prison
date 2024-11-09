using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sItem : MonoBehaviour
{
    private bool _playerIsClose;
    public int id;
    public string nameOfItem;
    public string description;
    private bool _isItemDelivered;

    // Start is called before the first frame update
    void Start()
    {
        _playerIsClose = false;
        nameOfItem = $"Page no. {id+1}";
        _isItemDelivered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) && _playerIsClose && sMovement.Instance.FreeToAct) || _isItemDelivered)
        {
            sNotebook.Instance.CreateNewEntry(nameOfItem, description);
            Destroy(gameObject);
        }
    }

    public void DeliverItem() => _isItemDelivered = true;

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
