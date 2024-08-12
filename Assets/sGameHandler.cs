using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

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
    public Prisoner(string name, PrisonerRole role, string roleDialogue)
    {
        Name = name;
        Role = role;
        RoleDialogue = roleDialogue;
    }

    public string Name { get; }
    public PrisonerRole Role { get; }
    public string RoleDialogue { get; }

}

public class sGameHandler : MonoBehaviour
{
    [SerializeField] private int WitnessCount;
    [SerializeField] private int InformantCount;
    [SerializeField] private int VerifierCount;
    private Dictionary<PrisonerRole, IReadOnlyList<string>> _dialogueDictionary = new()
    {
        { PrisonerRole.None, new[] { "I didn't do it! I don't know who did." } },
        { PrisonerRole.Murderer, new[] { "I didn't do it! I don't know who did." } },
        { PrisonerRole.Verifier, new[] { "It couldn't be {0}." } },
        { PrisonerRole.Witness, new[] { "It was {0}." } },
        { PrisonerRole.Informant, new[] { "{0} might know something." } },
    };


    private IReadOnlyDictionary<string, Prisoner> _prisoners = new Dictionary<string, Prisoner>();

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
        /*if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (var entry in PrisonerList)
            {
                Debug.Log(entry.Prisoner + " " + entry.Role);
            }
        } // Debug to check list of prisoners*/
    }

    private string GetRandomDialogue(PrisonerRole role, string relationName)
    {
        var templates = _dialogueDictionary[role];
        if (string.IsNullOrWhiteSpace(relationName))
            return templates[Random.Range(0, templates.Count)];
        return string.Format(templates[Random.Range(0, templates.Count)], relationName);
    }

    private IReadOnlyDictionary<string, Prisoner> CreateRolesDictionary(IEnumerable<string> names)
    {
        IList<string> namesList = names is IList<string> ? names as IList<string> : names.ToList();

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
                prisonersDictionary.Add(name, new Prisoner(name, role, GetRandomDialogue(role, relation)));
                System.Diagnostics.Debug.WriteLine($"{name} is {role} in relation to {relation}.");
                namesList.RemoveAt(nameIndex);
                availableRoles[role]--;
            }
        }

        return prisonersDictionary;
    }

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
            var list = prisoners.Where(p => predicate(p)).ToList();
            return list[Random.Range(0, list.Count)].Name;
        }
    }
}
