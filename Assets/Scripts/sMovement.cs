using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;

public class sMovement : MonoBehaviour
{
    public static sMovement Instance { get; private set; }

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    /// <summary>
    /// Is invoked every time the timer value is updated.
    /// </summary>
    [SerializeField] public UnityEvent<float> TimerEvent;
    private Vector2 movementDirection;
    public bool FreeToAct { get; set; }
    public bool timeRunning;
    public float timeLeft;
    private bool moving;

    // Start is called before the first frame update
    void Start()
    {
        FreeToAct = true;
        //TIME RUNNING WILL BE ACTIVATED UPON FINISHING THE CONVERSATION WITH THE TELEPHONE!!!
        TimerEvent.AddListener(new UnityAction<float>(TimeEnd));
        //timeRunning = true;
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

    private void OnDestroy()
    {
        Instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (FreeToAct)
        {
            movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            movementDirection.Normalize();
            Animate();
        }

        if (timeRunning)
        {
            timeLeft -= Time.deltaTime;
            TimerEvent.Invoke(timeLeft);
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

    public void TimeEnd(float timeLeft)
    {
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

    private void Animate()
    {
        if (movementDirection.magnitude > 0.1f || movementDirection.magnitude < -0.1f)
            moving = true;
        else
            moving = false;

        if (moving)
        {
            animator.SetFloat("X", movementDirection.x);
            animator.SetFloat ("Y", movementDirection.y);
        }

        animator.SetBool("Moving", moving);
    }
}
