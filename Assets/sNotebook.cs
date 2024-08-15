using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class sNotebook : MonoBehaviour
{
    [SerializeField] GameObject _player;

    public Dictionary<string, string> _entries = new ();
    private string _tempEntryConsolidate;

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

        if (_player.GetComponent<sMovement>()._freeToAct && Input.GetKeyDown(KeyCode.Q) && !GetComponent<CanvasGroup>().interactable)
        {
            GetComponent<CanvasGroup>().alpha = 1.0f;
            GetComponent<CanvasGroup>().interactable = true;
            transform.localScale = new Vector3(1f, 1f, 1f);
            _player.GetComponent<sMovement>()._freeToAct = false;
        }
        else if (!_player.GetComponent<sMovement>()._freeToAct && Input.GetKeyDown(KeyCode.Q) && GetComponent<CanvasGroup>().interactable)
        {
            GetComponent<CanvasGroup>().alpha = 0.0f;
            GetComponent<CanvasGroup>().interactable = false;
            transform.localScale = new Vector3(0f, 1f, 1f);
            _player.GetComponent<sMovement>()._freeToAct = true;
        }
    }

    private void FillNotebook()
    {
        GameObject.FindGameObjectWithTag("NotebookText").GetComponent<TMP_Text>().text = "";
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
}
