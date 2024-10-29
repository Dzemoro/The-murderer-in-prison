#nullable enable
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
    public Prisoner(string name, PrisonerRole role, IEnumerable<Dialogue> roleDialogues, string? relatesTo, bool isAlive, string crime, string background)
    {
        Name = name;
        Role = role;
        RoleDialogues = roleDialogues;
        RelatesTo = relatesTo;
        IsAlive = isAlive;
        Crime = crime;
        Background = background;
    }

    public string Name { get; }
    public PrisonerRole Role { get; }
    public string? RelatesTo { get; }
    public IEnumerable<Dialogue> RoleDialogues { get; }
    public bool IsAlive { get; }
    public string Crime { get; }
    public string Background { get; }

    public override string ToString()
        => string.Format("Name: {0} Role: {1} Status: {2} Relation: {3}", Name, Role, IsAlive ? "Alive" : "Dead", RelatesTo);
}

public class PrisonerFileData
{
    public PrisonerFileData(string name, Dictionary<PrisonerRole, Dictionary<DialogueType, string[]>> dialogues, string crime, string background)
    {
        Name = name;
        Dialogues = dialogues;
        Crime = crime;
        Background = background;
    }

    public string Name { get; }
    public Dictionary<PrisonerRole, Dictionary<DialogueType, string[]>> Dialogues { get; }
    public string Crime { get; }
    public string Background { get; }

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

    #region This object management

#pragma warning disable CS8618 // Non-nullable variable must contain a non-null value when exiting constructor
    public static sGameHandler Instance { get; private set; }
#pragma warning restore CS8618 


    // Start is called before the first frame update
    void Start()
    {
        sMovement.Instance.TimerEvent.AddListener(new(AddMurderCaseEvent));
        sNotebook.Instance.UpdateClue(GetRandomClue().Text);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        Instance = null!;
    }
    #endregion

    #region Initial prisoner data randomization
    [SerializeField] private int WitnessCount;
    [SerializeField] private int InformantCount;
    [SerializeField] private int VerifierCount;

    private readonly Dictionary<string, PrisonerFileData> _prisonersFileData = ReadTsvDialogueFile();

    private static Dictionary<string, PrisonerFileData> ReadTsvDialogueFile()
    {
        var text = File.ReadAllLines(Path.Combine(Application.streamingAssetsPath, "Dialogues.tsv")).Select(x => x.Split('\t'));
        var headerIndexes = text.First()
            .Select((value, index) => new { Value = value, Index = index })
            .ToDictionary(x => x.Index, x => x.Value);
        var lines = text.Skip(1)
            .Select(x => x.Select((value, index) => new { Value = value, Header = headerIndexes[index] }));
        var result = new Dictionary<string, PrisonerFileData>();
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
            result.Add(key, new PrisonerFileData(key, prisonerDict, string.Empty, string.Empty));
        }
        return result;
    }

    private static Dictionary<string, PrisonerFileData> ReadJsonDialogueFile(string filePath)
    {
        var json = JsonConvert.DeserializeObject<IEnumerable<JsonRecord>>(File.ReadAllText(filePath));
        return json.ToDictionary(x => x.Name,
            x => new PrisonerFileData(x.Name, GetDialoguesDict(x), x.Crime, x.Background));

        Dictionary<PrisonerRole, Dictionary<DialogueType, string[]>> GetDialoguesDict(JsonRecord jsonRecord)
        {
            string splitter = ";";
            var result = new Dictionary<PrisonerRole, Dictionary<DialogueType, string[]>>()
            {
                { PrisonerRole.Murderer, new Dictionary<DialogueType, string[]>() },
                { PrisonerRole.Verifier, new Dictionary<DialogueType, string[]>() },
                { PrisonerRole.Informant, new Dictionary<DialogueType, string[]>() },
                { PrisonerRole.None, new Dictionary<DialogueType, string[]>() },
                { PrisonerRole.Witness, new Dictionary<DialogueType, string[]>() }
            };
            result[PrisonerRole.Murderer].Add(DialogueType.Suspicion, jsonRecord.DialogueTexts.Suspicion.Murderer.Split(splitter));
            result[PrisonerRole.Murderer].Add(DialogueType.Alibi, jsonRecord.DialogueTexts.Alibi.Murderer.Split(splitter));
            result[PrisonerRole.Murderer].Add(DialogueType.Clue, new[] { jsonRecord.Clue });

            result[PrisonerRole.Verifier].Add(DialogueType.Suspicion, jsonRecord.DialogueTexts.Suspicion.Verifier.Split(splitter));
            result[PrisonerRole.Verifier].Add(DialogueType.Alibi, jsonRecord.DialogueTexts.Alibi.Verifier.Split(splitter));
            result[PrisonerRole.Verifier].Add(DialogueType.Clue, new[] { jsonRecord.Clue });

            result[PrisonerRole.Witness].Add(DialogueType.Suspicion, jsonRecord.DialogueTexts.Suspicion.Witness.Split(splitter));
            result[PrisonerRole.Witness].Add(DialogueType.Alibi, jsonRecord.DialogueTexts.Alibi.Witness.Split(splitter));
            result[PrisonerRole.Witness].Add(DialogueType.Clue, new[] { jsonRecord.Clue });

            result[PrisonerRole.Informant].Add(DialogueType.Suspicion, jsonRecord.DialogueTexts.Suspicion.Informant.Split(splitter));
            result[PrisonerRole.Informant].Add(DialogueType.Alibi, jsonRecord.DialogueTexts.Alibi.Informant.Split(splitter));
            result[PrisonerRole.Informant].Add(DialogueType.Clue, new[] { jsonRecord.Clue });

            result[PrisonerRole.None].Add(DialogueType.Suspicion, jsonRecord.DialogueTexts.Suspicion.None.Split(splitter));
            result[PrisonerRole.None].Add(DialogueType.Alibi, jsonRecord.DialogueTexts.Alibi.None.Split(splitter));
            result[PrisonerRole.None].Add(DialogueType.Clue, new[] { jsonRecord.Clue });
            return result;
        }

    }

    private Dictionary<string, Prisoner> CreateRolesDictionary(IEnumerable<string> names)
    {
        IList<string> namesList = names is IList<string> list ? list : names.ToList();

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
                var name = GetRandomElement(namesList);
                var prisonerData = CreatePrisonerData(prisonersDictionary.Values, role, name);
                prisonersDictionary.Add(name, prisonerData);
                System.Diagnostics.Debug.WriteLine($"{name} is {role} in relation to {prisonerData.RelatesTo}.");
                namesList.Remove(name);
                availableRoles[role]--;
            }
        }

        return prisonersDictionary;
    }

    private Dialogue GetRandomClue()
    {
        var candidates = Prisoners.Where(p => p.Value.Role != PrisonerRole.Murderer && p.Value.Role != PrisonerRole.None);
        var chosen = GetRandomElement(candidates);
        var clue = chosen.Value.RoleDialogues.Where(x => x.DialogueType == DialogueType.Clue).Single();
        System.Diagnostics.Debug.WriteLine($"Clue from {chosen.Key}: '{clue.Text}'");
        return clue;
    }
    #endregion

    #region Handle new murder cases
    private List<float> _executionPoints = new List<float>() { 1150, 600 };

    public void AddMurderCase()
    {
        var victimsSource = Prisoners.Where(p => p.Value.Role != PrisonerRole.Murderer && p.Value.IsAlive && p.Value.Role != PrisonerRole.Witness);
        var victim = UpdateRandomPrisoner(victimsSource, Enumerable.Empty<KeyValuePair<string, Prisoner>>(), PrisonerRole.None, isAlive: false);
        System.Diagnostics.Debug.WriteLine($"{victim} has been killed.");

        var newRolesSource = victimsSource.Where(x => x.Key != victim.Name && x.Value.Role == PrisonerRole.None);
        var newWitness = UpdateRandomPrisoner(newRolesSource, Prisoners, PrisonerRole.Witness);
        System.Diagnostics.Debug.WriteLine($"{newWitness} is the new witness.");

        var verifierRolesSource = victimsSource.Where(x => x.Key != newWitness.Name);
        UpdateRandomPrisoner(verifierRolesSource, Prisoners, PrisonerRole.Verifier);
    }

    private Prisoner UpdateRandomPrisoner(IEnumerable<KeyValuePair<string, Prisoner>> sourceCollection, IEnumerable<KeyValuePair<string, Prisoner>> relationSourceCollection, PrisonerRole targetRole, bool isAlive = true)
    {
        var targetNpc = GetRandomElement(sourceCollection);
        var newData = isAlive ? 
            CreatePrisonerData(relationSourceCollection.Where(x => x.Key != targetNpc.Key).Select(x => x.Value), targetRole, targetNpc.Key) :
            new Prisoner(targetNpc.Key, PrisonerRole.None, Enumerable.Empty<Dialogue>(), null, false, targetNpc.Value.Crime, targetNpc.Value.Background);
        _prisoners[targetNpc.Key] = newData;
        GameObject.FindGameObjectsWithTag("NPC").Select(x => x.GetComponent<sNPC>()).Single(p => p.GetNPCName() == targetNpc.Key).UpdatePrisonerData();
        return newData;
    }

    public void AddMurderCaseEvent(float timeLeft)
    {
        if (_executionPoints.Count > 0 && timeLeft < _executionPoints[0])
        {
            _executionPoints.RemoveAt(0);
            AddMurderCase();
        }
    }
    #endregion

    #region Randomized data helpers
    public static T GetRandomElement<T>(IEnumerable<T> source)
    {
        var list = source is IList<T> sourceList ? sourceList : source.ToList();
        return list[Random.Range(0, list.Count)];
    }
    private Prisoner CreatePrisonerData(IEnumerable<Prisoner> prisoners, PrisonerRole role, string name)
    {
        var relation = GetRandomRelation(prisoners, role);
        return new Prisoner(
            name, 
            role, 
            GetRandomDialogues(name, role, relation), 
            string.IsNullOrEmpty(relation) ? null : relation, 
            true, 
            _prisonersFileData[name].Crime, 
            _prisonersFileData[name].Background);
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
                return GetRandomElement(prisoners).Name;
            return GetRandomElement(prisoners.Where(p => predicate(p) && p.IsAlive)).Name;
        }
    }

    private IEnumerable<Dialogue> GetRandomDialogues(string name, PrisonerRole role, string relationName)
    {
        var roleTexts = _prisonersFileData[name].Dialogues[role];
        var result = new List<Dialogue>();
        foreach (var dialogueType in System.Enum.GetValues(typeof(DialogueType)).OfType<DialogueType>())
        {
            var templates = roleTexts[dialogueType];
            string text;
            if (dialogueType == DialogueType.Clue)
            {
                text = string.Format(GetRandomElement(templates), name);
            }
            else
            {
                text = string.IsNullOrWhiteSpace(relationName) ?
                    GetRandomElement(templates) :
                    string.Format(GetRandomElement(templates), relationName);
            }
            result.Add(new Dialogue(text, GetSummary(text, relationName, role, dialogueType), dialogueType));
        }
        return result;

        string GetSummary(string text, string relationName, PrisonerRole prisonerRole, DialogueType dialogueType)
        {
            if (dialogueType == DialogueType.Clue)
                return string.Empty;
            if (dialogueType == DialogueType.Alibi)
                return $"Alibi: {text}";
            if (dialogueType == DialogueType.Suspicion)
            {
                return prisonerRole switch
                {
                    PrisonerRole.None => "They didn't know anything about murder.",
                    PrisonerRole.Verifier => $"They have verified {relationName}'s allibi.",
                    PrisonerRole.Informant => $"They suggested to ask {relationName}.",
                    PrisonerRole.Witness => $"They suggested {relationName} might be the culprit.",
                    PrisonerRole.Murderer => $"They said \"{text}\"",
                    _ => throw new System.ArgumentException($"Unknown role {prisonerRole}")
                };
            }
            return $"\"{text}\"";
        }
    }
    #endregion

    #region Public helping functions

#pragma warning disable CS8618 // Non-nullable variable must contain a non-null value when exiting constructor
    [SerializeField] private DialogueEditor.NPCConversation BaseConversation;
#pragma warning restore CS8618 

    public DialogueEditor.NPCConversation GetBaseConversation() => Object.Instantiate(BaseConversation);

    /// <summary>
    /// Checks if prisoner is a murderer
    /// </summary>
    public bool CheckPrisoner(string name)
        => Prisoners[name].Role == PrisonerRole.Murderer;
    #endregion
}

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class GPTJsonRecord
{
    public IEnumerable<JsonRecord> Prisoners { get; set; }
}
public class JsonRecord
{
    public string Name { get; set; }
    public string Background { get; set; }
    public string Crime { get; set; }
    public string Clue { get; set; }

    [JsonProperty("dialogues")]
    public DialoguesTexts? DialoguesGPT { get; set; }


    [JsonProperty("dialogue")]
    public DialoguesTexts? DialoguesGemini { get; set; }

    [JsonIgnore]
    public DialoguesTexts DialogueTexts => DialoguesGemini is null ? DialoguesGPT : DialoguesGemini;

    public class DialoguesTexts
    {
        [JsonIgnore]
        public RolesTexts Suspicion => KnowAnythingGemini is null ? KnowAnythingGPT : KnowAnythingGemini;

        public RolesTexts Alibi { get; set; }

        [JsonProperty("knowAnything")]
        public RolesTexts? KnowAnythingGemini { get; set; }


        [JsonProperty("knows_anything")]
        public RolesTexts? KnowAnythingGPT { get; set; }
    }

    public class RolesTexts
    {
        public string Murderer { get; set; }
        public string Witness { get; set; }
        public string Informant { get; set; }
        public string Verifier { get; set; }
        public string None { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}