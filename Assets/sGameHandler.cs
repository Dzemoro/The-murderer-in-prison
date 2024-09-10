#nullable enable
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

#region Prisoner Data
public enum PrisonerRole
{
    /// <summary>
    /// Knows nothing. Uninvloved.
    /// </summary>
    None,
    /// <summary>
    /// The Murderer
    /// </summary>
    Murderer,
    /// <summary>
    /// Confirms someones alibi
    /// </summary>
    Verifier,
    /// <summary>
    /// Points at the witness
    /// </summary>
    Informant,
    /// <summary>
    /// Points at the murderer
    /// </summary>
    Witness 
}

public class Prisoner
{
    public Prisoner(string name, PrisonerRole role, IEnumerable<Dialogue> roleDialogues, string? relatesTo, bool isAlive)
    {
        Name = name;
        Role = role;
        RoleDialogues = roleDialogues;
        RelatesTo = relatesTo;
        IsAlive = isAlive;
    }

    public string Name { get; }
    public PrisonerRole Role { get; }
    public string? RelatesTo { get; }
    public IEnumerable<Dialogue> RoleDialogues { get; }
    public bool IsAlive { get; }
}
#endregion

#region Dialogues Structures
public enum DialogueType
{
    /// <summary>
    /// Characters alibi
    /// </summary>
    Alibi = 5,

    /// <summary>
    /// Starting clue pointing to character
    /// </summary>
    Clue = -1,

    /// <summary>
    /// Dialogue for asking about additional information
    /// </summary>
    Suspicion = 4
}

public class Dialogue
{
    public Dialogue(string text, string notebookSummary, DialogueType dialogueType, bool addedToNotebook = false)
    {
        Text = text;
        NotebookSummary = notebookSummary;
        DialogueType = dialogueType;
        AddedToNotebook = addedToNotebook;
    }

    public string Text { get; }
    public string NotebookSummary { get; }
    public DialogueType DialogueType { get; } 
    public bool AddedToNotebook { get; private set; }

    public void SetAsAdded() => AddedToNotebook = true;
}
#endregion

public class sGameHandler : MonoBehaviour
{
    [SerializeField] private int WitnessCount;
    [SerializeField] private int InformantCount;
    [SerializeField] private int VerifierCount;

#pragma warning disable CS8618 // Non-nullable variable must contain a non-null value when exiting constructor
    [SerializeField] private DialogueEditor.NPCConversation BaseConversation;
#pragma warning restore CS8618 
    
    private readonly Dictionary<string, Dictionary<PrisonerRole, Dictionary<DialogueType, string[]>>> _dialogueDictionary = ReadDialogueFile();


    private Dictionary<string, Prisoner> _prisoners = new();

    /// <summary>
    /// Dictionary of all prisoners names with roles assigned to them.
    /// </summary>
    public IReadOnlyDictionary<string, Prisoner> Prisoners
    {
        get 
        {
            if (!_prisoners.Any())
                _prisoners = CreateRolesDictionary(GameObject.FindGameObjectsWithTag("NPC").Select(x => x.GetComponent<sNPC>().GetNPCName()));
            return _prisoners;
        } 
    }
    
    public DialogueEditor.NPCConversation GetBaseConversation() => Object.Instantiate(BaseConversation);

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerable<Dialogue> GetRandomDialogues(string name, PrisonerRole role, string relationName)
    {
        var roleTexts = _dialogueDictionary[name][role];
        var result = new List<Dialogue>();
        foreach (var dialogueType in System.Enum.GetValues(typeof(DialogueType)).OfType<DialogueType>())
        {
            var templates = roleTexts[dialogueType];
            var text = string.IsNullOrWhiteSpace(relationName) ?
                templates[Random.Range(0, templates.Length)] :
                string.Format(templates[Random.Range(0, templates.Length)], relationName);
            result.Add(new Dialogue(text, GetSummary(text, relationName), dialogueType));
        }
        return result;

        string GetSummary(string text, string relationName)
        {
            var summary = $"\"{text}\"";
            if (!string.IsNullOrWhiteSpace(relationName))
                summary += $"regarding {relationName}";
            return summary;
        }
    }

    private Dictionary<string, Prisoner> CreateRolesDictionary(IEnumerable<string> names)
    {
        IList<string> namesList = names is IList<string> ? (IList<string>)names : names.ToList();

        var availableRoles = new Dictionary<PrisonerRole, int>() //WARNING: The order of entries here is important!
        {
            { PrisonerRole.Murderer, 1 },
            { PrisonerRole.Witness, WitnessCount },
            { PrisonerRole.Informant, InformantCount },
            { PrisonerRole.None, namesList.Count - (1 + WitnessCount + InformantCount + VerifierCount)},
            { PrisonerRole.Verifier, VerifierCount }
        };

        var prisonersDictionary = new Dictionary<string, Prisoner>();
        
        foreach (var role in availableRoles.Keys.ToList())
        {
            while (availableRoles[role] > 0)
            {
                var nameIndex = Random.Range(0, namesList.Count);
                var name = namesList[nameIndex];
                var relation = GetRandomRelation(prisonersDictionary.Values, role);
                prisonersDictionary.Add(name, new Prisoner(name, role, GetRandomDialogues(name, role, relation), string.IsNullOrEmpty(relation) ? null : relation, true));
                System.Diagnostics.Debug.WriteLine($"{name} is {role} in relation to {relation}.");
                namesList.RemoveAt(nameIndex);
                availableRoles[role]--;
            }
        }

        return prisonersDictionary;
    }

    /// <summary>
    /// Generates random relation from list of prisoners for a prisoner with given role.
    /// </summary>
    /// <returns>
    /// Name of a random prisoner the role can relate to.
    /// </returns>
    private string GetRandomRelation(IEnumerable<Prisoner> prisoners, PrisonerRole role)
    {
        return role switch
        {
            PrisonerRole.Witness => PickName(p => p.Role == PrisonerRole.Murderer), 
            PrisonerRole.Informant => PickName(p => p.Role == PrisonerRole.Witness),
            PrisonerRole.Verifier => PickName(p => p.Role != PrisonerRole.Murderer), //WARNING: The usage of this function without full list of prisoners and their roles makes some of the combinations impossible
            _ => string.Empty
        };


        string PickName(System.Func<Prisoner, bool> predicate)
        {
            if (predicate is null)
                return prisoners.ElementAt(Random.Range(0, prisoners.Count())).Name;
            var list = prisoners.Where(p => predicate(p) && p.IsAlive).ToList();
            return list[Random.Range(0, list.Count)].Name;
        }
    }

    /// <summary>
    /// Checks if prisoner is a murderer
    /// </summary>
    public bool CheckPrisoner(string name)
        => Prisoners[name].Role == PrisonerRole.Murderer;
    
    private static Dictionary<string, Dictionary<PrisonerRole, Dictionary<DialogueType, string[]>>> ReadDialogueFile()
    {
        var text = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, "Dialogues.tsv")).Select(x => x.Split('\t'));
        var headerIndexes = text.First()
            .Select((value, index) => new { Value = value, Index = index })
            .ToDictionary(x => x.Index, x => x.Value);
        var lines = text.Skip(1)
            .Select(x => x.Select((value, index) => new { Value = value, Header = headerIndexes[index] }));
        var result = new Dictionary<string, Dictionary<PrisonerRole, Dictionary<DialogueType, string[]>>>();
        foreach (var line in lines)
        {
            var prisonerDict = new Dictionary<PrisonerRole, Dictionary<DialogueType, string[]>>();
            string key = string.Empty;
            foreach (var item in line)
            {
                if (item.Header == "Prisoner Name")
                    key = item.Value;
                else
                {
                    var splitHeader = item.Header.Split('-');
                    var role = (PrisonerRole)System.Enum.Parse(typeof(PrisonerRole), splitHeader[0]);
                    var dialogueType = (DialogueType)System.Enum.Parse(typeof(DialogueType), splitHeader[1]);
                    if (!prisonerDict.ContainsKey(role))
                        prisonerDict.Add(role, new Dictionary<DialogueType, string[]>());
                    prisonerDict[role].Add(dialogueType, item.Value.Split(';'));
                }
            }
            result.Add(key, prisonerDict);
        }
        return result;
    }

    public void AddMurderCase()
    {
        var victimsSource = Prisoners.Where(p => p.Value.Role != PrisonerRole.Murderer && p.Value.IsAlive && p.Value.Role != PrisonerRole.Witness);
        var victim = victimsSource.ElementAt(Random.Range(0, victimsSource.Count()));
        _prisoners[victim.Key] = new Prisoner(victim.Key, PrisonerRole.None, Enumerable.Empty<Dialogue>(), null, true);
        var victimNPC = GameObject.FindGameObjectsWithTag("NPC").Select(x => x.GetComponent<sNPC>()).Single(p => p.GetNPCName() == victim.Key);
        victimNPC.UpdatePrisonerData();

        var newRolesSource = victimsSource.Where(x => x.Key != victim.Key && x.Value.Role == PrisonerRole.None);
        var newWitness = Prisoners.ElementAt(Random.Range(0, newRolesSource.Count()));
        var witnessRelation = GetRandomRelation(Prisoners.Values, PrisonerRole.Witness);
        _prisoners[newWitness.Key] = new Prisoner(newWitness.Key, PrisonerRole.Witness, GetRandomDialogues(name, PrisonerRole.Witness, witnessRelation), witnessRelation, true);
        GameObject.FindGameObjectsWithTag("NPC").Select(x => x.GetComponent<sNPC>()).Single(p => p.GetNPCName() == newWitness.Key).UpdatePrisonerData();


        var verifierRolesSource = victimsSource.Where(x => x.Key != newWitness.Key && x.Value.Role == PrisonerRole.None);
        var newVerifier = Prisoners.ElementAt(Random.Range(0, newRolesSource.Count()));
        var verifierRelation = GetRandomRelation(Prisoners.Values, PrisonerRole.Verifier);
        _prisoners[newVerifier.Key] = new Prisoner(newVerifier.Key, PrisonerRole.Verifier, GetRandomDialogues(name, PrisonerRole.Verifier, verifierRelation), witnessRelation, true);
        GameObject.FindGameObjectsWithTag("NPC").Select(x => x.GetComponent<sNPC>()).Single(p => p.GetNPCName() == newVerifier.Key).UpdatePrisonerData();
    }
}
