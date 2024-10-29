using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class sGameMenu : MonoBehaviour
{

    public static sGameMenu Instance { get; private set; }

    [SerializeField] private bool _menuOpen;
    [SerializeField] private bool _playerWasActive;
    [SerializeField] private bool _timeWasRunning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_menuOpen)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                if (sMovement.Instance.FreeToAct)
                {
                    sMovement.Instance.StopMovement();
                    _playerWasActive = true;
                }

                if (sMovement.Instance.timeRunning)
                {
                    sMovement.Instance.timeRunning = false;
                    _timeWasRunning = true;
                }
                _menuOpen = true;
            }
            else
            {
                transform.localScale = new Vector3(0f, 1f, 1f);

                if (_playerWasActive)
                {
                    _playerWasActive = false;
                    sMovement.Instance.StartMovement();

                }

                if (_timeWasRunning)
                {
                    _timeWasRunning = false;
                    sMovement.Instance.timeRunning = true;

                }
                _menuOpen = false;
            }
        }
    }
}
