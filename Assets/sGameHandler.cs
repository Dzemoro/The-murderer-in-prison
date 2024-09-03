using System.Collections.Generic;
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
    private Dictionary<string, Prisoner> _prisoners = new();

    [SerializeField] private int WitnessCount;
    [SerializeField] private int InformantCount;
    [SerializeField] private int VerifierCount;
    [SerializeField] private int AllPrisonersCount;
    public IReadOnlyDictionary<string, Prisoner> Prisoners => _prisoners;
    private RoleNumber[] _leftRoles;
    private Dictionary<PrisonerRole, IReadOnlyList<string>> _dialogueDictionary = new()
    {
        { PrisonerRole.None, new[] { "I didn't do it! I don't know who did." } },
        { PrisonerRole.Murderer, new[] { "I didn't do it! I don't know who did." } },
        { PrisonerRole.Verifier, new[] { "It couldn't be {0}." } },
        { PrisonerRole.Witness, new[] { "It was {0}." } },
        { PrisonerRole.Informant, new[] { "{0} might know something." } },
    };

    // Start is called before the first frame update
    void Start()
    {
        //PrisonerList.Add(new PrisonerRole("Gene Roddenberry", "Totaly OK")); <- Just for testing the loading of pre-existing values of the list
       
        GameObject[] _allPrisoners = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject _tempPrisoner in _allPrisoners)
        {
            _tempPrisoner.GetComponent<sNPC>().GetPrisonerData(GetPrisoner(_tempPrisoner.GetComponent<sNPC>().GetNPCName()));
            //Here we assign the dialogue before all the roles are handed out. Maybe it will require a second foreach? TBC
            //Here we assign the dialogue before all the roles are handed out. Maybe it will require a second foreach? TBC
            _tempPrisoner.GetComponent<sNPC>().FillConversation(_tempPrisoner.GetComponent<sNPC>().PrisonerData.RoleDialogue);
        }
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

    public Prisoner GetPrisoner(string name)
    {
        if (_leftRoles is null)
            _leftRoles = new RoleNumber[] { new(1, PrisonerRole.Murderer), new(WitnessCount, PrisonerRole.Witness), new(InformantCount, PrisonerRole.Informant), new(VerifierCount, PrisonerRole.Verifier) };

        if (_prisoners.ContainsKey(name))
            return _prisoners[name];

        var number = Random.Range(0, AllPrisonersCount);
        var lastTarget = -1;
        foreach ( var role in _leftRoles )
        {
            if (role.NumberLeft > 0)
            {
                if (number >= lastTarget + 1 && number <= lastTarget + role.NumberLeft)
                {
                    role.NumberLeft--;
                    AllPrisonersCount--;
                    _prisoners.Add(name, new Prisoner(name, role.Role, _dialogueDictionary[role.Role][Random.Range(0, _dialogueDictionary[role.Role].Count())]));
                    System.Diagnostics.Debug.WriteLine($"{name} is {_prisoners[name].Role}.");
                    return _prisoners[name];
                }
                else
                    lastTarget += role.NumberLeft;
            }
        }
        AllPrisonersCount--;
        _prisoners.Add(name, new Prisoner(name, PrisonerRole.None, _dialogueDictionary[PrisonerRole.None][Random.Range(0, _dialogueDictionary[PrisonerRole.None].Count())]));
        System.Diagnostics.Debug.WriteLine($"{name} is {_prisoners[name].Role}.");
        return _prisoners[name];
    }

    private record RoleNumber
    {
        public readonly PrisonerRole Role;
        public int NumberLeft;

        public RoleNumber(int numberLeft, PrisonerRole role)
        {
            Role = role;
            NumberLeft = numberLeft;
        }
    }

    public bool CheckPrisoner(string _name)
    {
        if ( Prisoners.GetValueOrDefault(_name).Role == PrisonerRole.Murderer) return true;

        return false;
    }
}
