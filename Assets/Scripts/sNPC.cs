using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sNPC : MonoBehaviour
{
    public NPCConversation Conversation;
    private bool _playerIsClose;
    public string PrisonerName;
    public string PrisonerRole;

    // Start is called before the first frame update
    void Start()
    {
        PrisonerRole = GameObject.FindGameObjectWithTag("GameController").GetComponent<sGameHandler>().GetRole(PrisonerName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose)
            ConversationManager.Instance.StartConversation(Conversation);
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
}
