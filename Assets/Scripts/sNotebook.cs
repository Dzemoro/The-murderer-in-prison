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
    private TMP_Text _notebookText => _notebookTextParent.GetComponent<TMP_Text>();
    [SerializeField] private TMP_Text _search;
    [SerializeField] private AudioClip _scribble;
    [SerializeField] private GameObject _notebookTextParent;
    private string _crimeClue;
    private bool windowIsOpen;

    private AudioSource _audioSource;

    public bool searchIsActive;

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
        _audioSource = GetComponent<AudioSource>();
        searchIsActive = false;
        windowIsOpen = false;
        //_notebookText = _notebookTextParent.GetComponent<TMP_Text>();
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
        if (Input.GetKeyDown(KeyCode.Q) && GetComponent<CanvasGroup>().interactable)
        {
            if (sMovement.Instance.FreeToAct && !windowIsOpen)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                sMovement.Instance.StopMovement();
                sMovement.Instance.timeRunning = false;
                windowIsOpen = true;
            }
            else if (!sMovement.Instance.FreeToAct && windowIsOpen && !searchIsActive)
            {
                transform.localScale = new Vector3(0f, 1f, 1f);
                sMovement.Instance.StartMovement();
                sMovement.Instance.timeRunning = true;
                windowIsOpen = false;
            }
        }
    }

    public void FillNotebook()
    {
        _notebookText.text = "<u>A crime was commited!</u>\nIn our local prison, one of the inmates was brutally murdered. " +
                                                                                         "We are sure that is was one of the other inmates but we currently don't know who might have done such a thing. " +
                                                                                         "Please help us find out before future crimes are commited.\nWe might have a clue:\n" +
                                                                                         _crimeClue + "\n\n\n";
        foreach (var entry in _entries)
        {
            if( entry.Key.Contains( _search.text.Replace("\u200B", string.Empty),System.StringComparison.InvariantCultureIgnoreCase ) )
                _notebookText.text += "<u>" + entry.Key + "</u>\n" + entry.Value + "\n\n\n";
        }
    }
    
    public void UpdateNotebook(string prisonerName, string stringAppend)
    {
        _audioSource.clip = _scribble;

        if( !sGameMenu.Instance.soundMute ) _audioSource.Play();
        _entries[prisonerName] += $"\n{stringAppend}";
        FillNotebook();
    }
    
    public void UpdateClue(string clue)
    {
        _crimeClue = clue;
        FillNotebook() ;
    }

    public void CreateNewEntry(string entryKey, string entryText)
    {
        _audioSource.clip = _scribble;
        if (!sGameMenu.Instance.soundMute) _audioSource.Play();
        string _key = "Item: " + entryKey;
        string _text = "\n" + entryText;
        _entries.Add(_key, _text);
        FillNotebook();
    }
}
