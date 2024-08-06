using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sGameHandler : MonoBehaviour
{

    public struct PrisonerRole
    {
        public string Prisoner { get; }
        public string Role { get; }

        public PrisonerRole(string Prisoner, string Role)
        {
            this.Prisoner = Prisoner;
            this.Role = Role;
        }
    }
    [SerializeField] private List<PrisonerRole> PrisonerList = new List<PrisonerRole>();
    [SerializeField] private List<string> RolesToAssign;

    // Start is called before the first frame update
    void Start()
    {
        //PrisonerList.Add(new PrisonerRole("Gene Roddenberry", "Totaly OK")); <- Just for testing the loading of pre-existing values of the list
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

    public string GetRole(string Name)
    {
        bool exists = false;
        string Role = "Empty";

        foreach (var entry in PrisonerList) { if (entry.Prisoner == Name) { exists = true; Role = entry.Role; } }

        if (!exists) {
            int entry = Random.Range(0, RolesToAssign.Count);
            Role = RolesToAssign[entry];
            PrisonerList.Add(new PrisonerRole(Name, Role));
            RolesToAssign.RemoveAt(entry);
        }
        return Role;
    }
}
