using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sNoCulprit : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _canvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NoCulprit()
    {
        _canvas.transform.localScale = new Vector3(0f, 1f, 1f);
        _player.GetComponent<sMovement>().FreeToAct = true;
    }
}
