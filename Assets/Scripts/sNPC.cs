using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        this.Conversation = sGameHandler.Instance.GetBaseConversation();
        UpdatePrisonerData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose && PrisonerData.IsAlive)
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
        conv.SpeechNodes.ForEach(speech => speech.Name = PrisonerData.Name);
        foreach (var dialogue in dialogues.Where(x => (int)x.DialogueType > 0))
        {
            var speechNode = conv.GetSpeechByUID((int)dialogue.DialogueType);
            speechNode.Text = dialogue.Text;
            var eventHolder = Conversation.GetNodeData((int)dialogue.DialogueType);
            eventHolder.Event.AddListener(new(() => AddToNotebook(dialogue)));
        }
        this.Conversation.Serialize(conv);
        this.Conversation.DefaultName = Name;
    }

    private void AddToNotebook(Dialogue dialogue)
    {
        if (dialogue.AddedToNotebook)
            return;
        dialogue.SetAsAdded();
        sNotebook.Instance.UpdateNotebook(PrisonerData.Name, dialogue.NotebookSummary);
    }

    public void UpdatePrisonerData()
    {
        PrisonerData = sGameHandler.Instance.Prisoners[Name];
        FillConversation(PrisonerData.RoleDialogues);
    }
}
