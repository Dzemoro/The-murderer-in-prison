using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class sTimerDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text display;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (sMovement.Instance.timeRunning)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(sMovement.Instance.timeLeft);
            display.text = timeSpan.ToString(@"mm\:ss");
            GetComponent<CanvasGroup>().alpha = 1.0f;
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 0.0f;
        }


        //display.text = string.Format("{0:00} : {1:00}", (sMovement.Instance.timeLeft / 60), (sMovement.Instance.timeLeft % 60));
    }
}