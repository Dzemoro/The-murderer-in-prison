using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPoliceman : MonoBehaviour
{

    [SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _results;

    private bool _playerIsClose;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose && sMovement.Instance.FreeToAct)
        {
            _canvas.transform.localScale = new Vector3(1f, 1f, 1f);
            sMovement.Instance.FreeToAct = false;
        }
        else if (!sMovement.Instance.FreeToAct && Input.GetKeyDown(KeyCode.E) && !_results.GetComponent<sResultVariables>()._open && _playerIsClose)
        {
            _canvas.transform.localScale = new Vector3(0f, 1f, 1f);
            sMovement.Instance.FreeToAct = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsClose = false;
        }
    }
}
