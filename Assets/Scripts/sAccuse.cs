using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class sAccuse : MonoBehaviour
{

    public string _name;
    [SerializeField] private GameObject _buttonText;
    [SerializeField] private GameObject _resultScreen;
    [SerializeField] private GameObject _resultText;
    [SerializeField] private GameObject _accuseMenu;
    [SerializeField] private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _buttonText.GetComponent<TMP_Text>().text += _name;

        _resultScreen = GameObject.Find("PanelResult");
        _resultText = GameObject.Find("ResultText");
        _accuseMenu = GameObject.Find("AccuseMenuAll");
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Accuse()
    {
        bool final = GameObject.FindGameObjectWithTag("GameController").GetComponent<sGameHandler>().CheckPrisoner(_name);

        _accuseMenu.transform.localScale = new Vector3(0f, 1f, 1f);
        _resultScreen.transform.localScale = new Vector3(1f, 1f, 1f);
        _resultScreen.GetComponent<sResultVariables>()._open = true;
        _player.GetComponent<sMovement>().timeRunning = false;
        if (final)
        {
            _resultText.GetComponent<TMP_Text>().text = "You have found the murderer!\nTime left: " + string.Format("{0:00} : {1:00}", (_player.GetComponent<sMovement>().timeLeft / 60 ), (_player.GetComponent<sMovement>().timeLeft % 60));
        }
        else
        {
            _resultText.GetComponent<TMP_Text>().text = "You have accused the wrong person!";
        }

        //Debug.Log(_name + " was: " + final);
    }
}
