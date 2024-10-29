using UnityEngine;

public class sCellEntranceTrigger : MonoBehaviour
{
    public string CellEntranceAnimatorParamName = "CellEntranceOpen";
    private bool playerIsClose;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            var isCellEntranceOpen = animator.GetBool(CellEntranceAnimatorParamName);
            animator.SetBool(CellEntranceAnimatorParamName, !isCellEntranceOpen);
        }
            
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerIsClose = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerIsClose = false;
    }
}