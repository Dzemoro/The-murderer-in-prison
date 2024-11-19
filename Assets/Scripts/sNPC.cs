using DialogueEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class sNPC : MonoBehaviour
{
    private NPCConversation Conversation;
    private bool _playerIsClose;
    
    public Prisoner PrisonerData {  get; private set; }

    [SerializeField]
    public int PrisonerId;


    //NPC Information for notebook
    public string Location;
    public string Background { get; private set; }
    public string Crime { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        this.Conversation = sGameHandler.Instance.GetBaseConversation();
        UpdatePrisonerData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose && PrisonerData.IsAlive && !ConversationManager.Instance.IsConversationActive && sMovement.Instance.FreeToAct)
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

    public string GetNPCName() => PrisonerData.Name;

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
            if (!string.IsNullOrWhiteSpace(dialogue.AudioFileName))
            {
                var audio = (AudioClip)Resources.Load(dialogue.AudioFileName);
                if (audio != null)
                {
                    eventHolder.Audio = audio;
                    speechNode.Volume = 1;
                }
                else
                {
                    Debug.LogWarning($"Missing audio clip resource: {dialogue.AudioFileName}.");
                }
            }
        }
        this.Conversation.Serialize(conv);
        this.Conversation.DefaultName = PrisonerData.Name;
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
        PrisonerData = sGameHandler.Instance.Prisoners[PrisonerId];
        if (!string.IsNullOrEmpty(PrisonerData.Crime))
            Crime = PrisonerData.Crime;
        if (!string.IsNullOrEmpty(PrisonerData.Background))
            Background = PrisonerData.Background;
        FillConversation(PrisonerData.RoleDialogues);
    }

    public static sNPC GetById(int id) => GameObject.FindGameObjectsWithTag("NPC").Select(x => x.GetComponent<sNPC>()).Single(p => p.PrisonerId == id);
    public static GameObject GetGameObjectById(int id) => GameObject.FindGameObjectsWithTag("NPC").Where(x => x.GetComponent<sNPC>().PrisonerId == id).Single();
}
