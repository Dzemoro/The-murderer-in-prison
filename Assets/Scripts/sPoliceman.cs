using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPoliceman : MonoBehaviour
{

    [SerializeField] GameObject _player;
    [SerializeField] GameObject _canvas;

    private bool _playerIsClose;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose && _player.GetComponent<sMovement>()._freeToAct)
        {
            _canvas.transform.localScale = new Vector3(1f, 1f, 1f);
            _player.GetComponent<sMovement>()._freeToAct = false;
        }
        else if (!_player.GetComponent<sMovement>()._freeToAct && Input.GetKeyDown(KeyCode.E))
        {
            _canvas.transform.localScale = new Vector3(0f, 1f, 1f);
            _player.GetComponent<sMovement>()._freeToAct = true;
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
