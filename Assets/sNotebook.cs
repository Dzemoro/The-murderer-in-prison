using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class sNotebook : MonoBehaviour
{
    public static sNotebook Instance { get; private set; }
    public Dictionary<string, string> _entries = new ();
    private string _tempEntryConsolidate;
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

    private void OnDestroy()
    {
        Instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] _allPrisoners = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject _tempPrisoner in _allPrisoners)
        {
            _tempEntryConsolidate = "\nPrisoner is located in: " + _tempPrisoner.GetComponent<sNPC>()._location +
                                    "\nPrisoner was previously: " + _tempPrisoner.GetComponent<sNPC>()._background +
                                    "\nPrisoner convited of: " + _tempPrisoner.GetComponent<sNPC>()._crime;

            _entries.Add(_tempPrisoner.GetComponent<sNPC>().GetNPCName(), _tempEntryConsolidate);
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
        GameObject.FindGameObjectWithTag("NotebookText").GetComponent<TMP_Text>().text = "<u>A crime was commited!</u>\nIn our local prison, one of the inmates was brutally murdered. " +
                                                                                         "We are sure that is was one of the other inmates but we currently don't know who might have done such a thing. " +
                                                                                         "Please help us find out before future crimes are commited.\nWe might have a clue:" +
                                                                                         _crimeClue + "\n\n\n";
        foreach (var _entry in _entries)
        {
            GameObject.FindGameObjectWithTag("NotebookText").GetComponent<TMP_Text>().text += "<u>" + _entry.Key + "</u>\n" + _entry.Value + "\n\n\n";
        }
    }
    
    public void UpdateNotebook(string _prisonerName, string _stringAppend)
    {
        _entries[_prisonerName] += "\nHeard from: " + _stringAppend;

        FillNotebook();
    }
    
    public void UpdateClue(string _clue)
    {
        _crimeClue = _clue;

        FillNotebook() ;
    }
}
