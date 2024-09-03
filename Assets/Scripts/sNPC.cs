using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class sNPC : MonoBehaviour
{
    private NPCConversation Conversation;
    private bool _playerIsClose;
    public Prisoner PrisonerData;
    [SerializeField] private string Name;

    //NPC Information for notebook
    public string _location;
    public string _background;
    public string _crime;

    // Start is called before the first frame update
    void Start()
    {
        var gameHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<sGameHandler>();
        this.Conversation = gameHandler.GetBaseConversation();
        PrisonerData = gameHandler.Prisoners[Name];
        FillConversation(PrisonerData.RoleDialogues);
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

    public string GetNPCName()
    {
        return Name;
    }

    private void FillConversation(IEnumerable<Dialogue> dialogues)
    {
        var conv = this.Conversation.DeserializeForEditor();
        foreach (var dialogue in dialogues.Where(x => (int)x.DialogueType > 0))
        {
            var speechNode = conv.GetSpeechByUID((int)dialogue.DialogueType);
            speechNode.Text = dialogue.Text;
        }
        this.Conversation.Serialize(conv);
        this.Conversation.DefaultName = Name;
    }
}
