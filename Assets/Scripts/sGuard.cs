using DialogueEditor;
using System.Collections;
using UnityEngine;

public class sGuard : MonoBehaviour
{
    [SerializeField] private NPCConversation Conversation;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float leftPatrolX;
    [SerializeField] private float rightPatrolX;
    [SerializeField] private float minPauseTime;
    [SerializeField] private float maxPauseTime;
    [SerializeField] private float minWalkTime;
    [SerializeField] private float maxWalkTime;
    [SerializeField] private int facingDirection = -1;

    private float _randomTime;
    public float _timer;
    private bool _isWalking = true;
    private bool _playerIsClose;
    private bool _isGuardAbleToMove = true;
    private bool _isChangingDirection;
    private float _lastX;

    // Start is called before the first frame update
    void Start()
    {
        _randomTime = Random.Range(minWalkTime, maxWalkTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose && !ConversationManager.Instance.IsConversationActive && sMovement.Instance.FreeToAct)
        {
            StopGuardMovement();
            sMovement.Instance.StopMovement();
            ConversationManager.Instance.StartConversation(Conversation);
        }

    }

    private void FixedUpdate()
    {
        if (_isGuardAbleToMove)
        {
            if (transform.position.x > rightPatrolX || transform.position.x < leftPatrolX)
                facingDirection *= -1;

            if (_isWalking)
                rb.MovePosition(rb.position + Vector2.right * facingDirection * speed * Time.fixedDeltaTime);
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

    public void StopGuardMovement() => _isGuardAbleToMove = false;
    public void StartGuardMovement() => _isGuardAbleToMove = true;
}
