using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sNotebook : MonoBehaviour
{
    [SerializeField] GameObject _player;
    // Start is called before the first frame update
    void Start()
    {

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
}
