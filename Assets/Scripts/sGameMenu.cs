using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sGameMenu : MonoBehaviour
{

    public static sGameMenu Instance { get; private set; }

    [SerializeField] private bool _menuOpen;
    [SerializeField] private bool _playerWasActive;
    [SerializeField] private bool _timeWasRunning;

    [SerializeField] private TMP_Text _musicButton;
    public bool musicMute;
    [SerializeField] private TMP_Text _soundButton;
    public bool soundMute;

    // Start is called before the first frame update
    void Start()
    {
        musicMute = false;
        soundMute = false;
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

    public void ResumeGame()
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

    public void RestartGane() { SceneManager.LoadScene("StartingScene"); }

    public void QuitGame() { Application.Quit(); }

    public void changeMusic()
    {
        if (musicMute)
        {
            musicMute = false;
            _musicButton.text = "Mute Music";
            sMovement.Instance.GetComponent<AudioSource>().mute = false;
        }
        else
        {
            musicMute = true;
            _musicButton.text = "Unmute Music";
            sMovement.Instance.GetComponent<AudioSource>().mute = true;
        }
    }

    public void changeSound()
    {
        if (soundMute)
        {
            soundMute = false;
            _soundButton.text = "Mute Sound";
        }
        else
        {
            soundMute = true;
            _soundButton.text = "Unmute Sound";
        }
    }
}
