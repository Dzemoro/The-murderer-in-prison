using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class sMovement : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 movementDirection;
    public bool FreeToAct { get; set; }
    public bool timeRunning;
    public float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        FreeToAct = true;

        //TIME RUNNING WILL BE ACTIVATED UPON FINISHING THE CONVERSATION WITH THE TELEPHONE!!!
        timeRunning = false;
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (FreeToAct)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.up = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

            movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            movementDirection.Normalize();
        }

        if (timeRunning)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                GameObject _resultScreen = GameObject.Find("PanelResult");
                GameObject _resultText = GameObject.Find("ResultText");
                GameObject _accuseMenu = GameObject.Find("AccuseMenuAll");

                _accuseMenu.transform.localScale = new Vector3(0f, 1f, 1f);
                _resultScreen.transform.localScale = new Vector3(1f, 1f, 1f);
                _resultScreen.GetComponent<sResultVariables>()._open = true;
                timeRunning = false;
                FreeToAct = false;
                _resultText.GetComponent<TMP_Text>().text = "The murderer escaped! You failed!";
            }
        }
    }

    private void FixedUpdate()
    {
        if (FreeToAct && Input.GetKey(KeyCode.LeftShift))
        {
            rb.MovePosition(rb.position + movementDirection * (speed / 3) * Time.fixedDeltaTime);
        }
        else if (FreeToAct)
        {
            rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
        }
    }

    public void StopMovement() => FreeToAct = false;
    public void StartMovement() => FreeToAct = true;
}
