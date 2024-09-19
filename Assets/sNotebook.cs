using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class sNotebook : MonoBehaviour
{
    public static sNotebook Instance { get; private set; }
    public Dictionary<string, string> _entries = new ();
    private TMP_Text _notebookText;
    private string _crimeClue;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _notebookText = GameObject.FindGameObjectWithTag("NotebookText").GetComponent<TMP_Text>();
        var allPrisoners = GameObject.FindGameObjectsWithTag("NPC").Select(p => p.GetComponent<sNPC>());
        foreach (var prisoner in allPrisoners)
        {
            var tempEntryConsolidate = "\nPrisoner is located in: " + prisoner.Location +
                                    "\nPrisoner was previously: " + prisoner.Background +
                                    "\nPrisoner convited of: " + prisoner.Crime;

            _entries.Add(prisoner.GetNPCName(), tempEntryConsolidate);
        }

        FillNotebook();
    }

    // Update is called once per frame
    void Update()
    {

        if (sMovement.Instance.FreeToAct && Input.GetKeyDown(KeyCode.Q) && !GetComponent<CanvasGroup>().interactable)
        {
            GetComponent<CanvasGroup>().alpha = 1.0f;
            GetComponent<CanvasGroup>().interactable = true;
            transform.localScale = new Vector3(1f, 1f, 1f);
            sMovement.Instance.StopMovement();
            sMovement.Instance.timeRunning = false;
        }
        else if (!sMovement.Instance.FreeToAct && Input.GetKeyDown(KeyCode.Q) && GetComponent<CanvasGroup>().interactable)
        {
            GetComponent<CanvasGroup>().alpha = 0.0f;
            GetComponent<CanvasGroup>().interactable = false;
            transform.localScale = new Vector3(0f, 1f, 1f);
            sMovement.Instance.StartMovement();
            sMovement.Instance.timeRunning = true;
        }
    }

    private void FillNotebook()
    {
        _notebookText.text = "<u>A crime was commited!</u>\nIn our local prison, one of the inmates was brutally murdered. " +
                                                                                         "We are sure that is was one of the other inmates but we currently don't know who might have done such a thing. " +
                                                                                         "Please help us find out before future crimes are commited.\nWe might have a clue:" +
                                                                                         _crimeClue + "\n\n\n";
        foreach (var entry in _entries)
        {
            _notebookText.text += "<u>" + entry.Key + "</u>\n" + entry.Value + "\n\n\n";
        }
    }
    
    public void UpdateNotebook(string prisonerName, string stringAppend)
    {
        _entries[prisonerName] += stringAppend;

        FillNotebook();
    }
    
    public void UpdateClue(string clue)
    {
        _crimeClue = clue;

        FillNotebook() ;
    }
}
