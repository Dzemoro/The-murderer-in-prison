using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sMoveToDifLoc : MonoBehaviour
{

    [SerializeField] private string GoTo;
    [SerializeField] private float ToX;
    [SerializeField] private float ToY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = new Vector3(ToX,ToY);
            SceneManager.LoadScene(GoTo);
        }
            
    }
}
