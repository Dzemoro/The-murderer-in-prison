using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class sAccuse : MonoBehaviour
{

    public string PrisonerName;
    public int PrisonerId;
    [SerializeField] private GameObject _buttonText;
    [SerializeField] private GameObject _resultScreen;
    [SerializeField] private GameObject _resultText;
    [SerializeField] private GameObject _accuseMenu;

    public AudioClip win;
    public AudioClip loose;

    // Start is called before the first frame update
    void Start()
    {
        _buttonText.GetComponent<TMP_Text>().text += PrisonerName;

        _resultScreen = GameObject.Find("PanelResult");
        _resultText = GameObject.Find("ResultText");
        _accuseMenu = GameObject.Find("AccuseMenuAll");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Accuse()
    {
        bool final = sGameHandler.Instance.Prisoners[PrisonerId].Role == PrisonerRole.Murderer;

        _accuseMenu.transform.localScale = new Vector3(0f, 1f, 1f);
        _resultScreen.transform.localScale = new Vector3(1f, 1f, 1f);
        _resultScreen.GetComponent<sResultVariables>()._open = true;
        sMovement.Instance.timeRunning = false;
        if (final)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(sMovement.Instance.timeLeft);
            string timeText = timeSpan.ToString(@"mm\:ss");
            _resultText.GetComponent<TMP_Text>().text = "You have found the murderer!\nTime left: " + timeText;
            //string.Format("{0:00} : {1:00}", (sMovement.Instance.timeLeft / 60 ), (sMovement.Instance.timeLeft % 60))
            GetComponent<AudioSource>().clip = win;
        }
        else
        {
            _resultText.GetComponent<TMP_Text>().text = "You have accused the wrong person!";
            GetComponent<AudioSource>().clip = loose;
        }

        if (!sGameMenu.Instance.soundMute) GetComponent<AudioSource>().Play();

        //Debug.Log(_name + " was: " + final);
    }
}
