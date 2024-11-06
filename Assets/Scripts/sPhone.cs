using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class sPhone : MonoBehaviour
{
    [SerializeField] private NPCConversation Conversation;
    [SerializeField] private GameObject notebook;
    private bool _playerIsClose;

    // Start is called before the first frame update
    void Start()
    {
        _playerIsClose = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose && !ConversationManager.Instance.IsConversationActive)
        {
            sMovement.Instance.StopMovement();
            ConversationManager.Instance.StartConversation(Conversation);
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
            //ZeroText();
        }
    }

    public void EndCall()
    {
        notebook.GetComponent<CanvasGroup>().interactable = true;
        sMovement.Instance.MovePlayerToPrison();
        sMovement.Instance.StartMovement();
        sMovement.Instance.timeRunning = true;
    }    
}
