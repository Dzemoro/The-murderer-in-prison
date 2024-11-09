using DialogueEditor;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class sGuard : MonoBehaviour
{
    [SerializeField] private NPCConversation Conversation;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed;
    [SerializeField] private float leftPatrolX;
    [SerializeField] private float rightPatrolX;
    [SerializeField] private int facingDirection = -1;

    private bool _isWalking = true;
    private bool _playerIsClose;
    private bool _isGuardAbleToMove = true;
    private bool _isChangingDirection;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose && !ConversationManager.Instance.IsConversationActive && sMovement.Instance.FreeToAct)
        {
            StopGuardMovement();
            sMovement.Instance.StopMovement();
            ConversationManager.Instance.StartConversation(Conversation);
        }
    }

    private void FixedUpdate()
    {
        Animate();
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

    public void StopGuardMovement()
    {
        _isGuardAbleToMove = false;
        _isWalking = false;
    }
    public void StartGuardMovement()
    {
        _isGuardAbleToMove = true;
        _isWalking = true;
    }

    private void Animate()
    {
        if (_isWalking)
            animator.SetFloat("X", facingDirection);
        animator.SetBool("Moving", _isWalking);
    }
}
