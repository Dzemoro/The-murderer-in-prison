using DialogueEditor;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class sNPC : MonoBehaviour
{
    public NPCConversation Conversation;
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
        PrisonerData = GameObject.FindGameObjectWithTag("GameController").GetComponent<sGameHandler>().Prisoners[Name];
        FillConversation(PrisonerData.RoleDialogue);
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

    private void FillConversation(string text)
    {
        var conv = this.Conversation.DeserializeForEditor();
        conv.GetRootNode().Name = Name;
        var speechNode = conv.SpeechNodes.Single(x => x.Text == "0");
        speechNode.Text = text;
        this.Conversation.Serialize(conv);
    }
}
