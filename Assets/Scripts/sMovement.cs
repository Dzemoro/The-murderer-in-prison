using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMovement : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 movementDirection;
    public bool _freeToAct;

    // Start is called before the first frame update
    void Start()
    {
        _freeToAct = true;
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (_freeToAct)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.up = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

            movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            movementDirection.Normalize();
        }
    }

    private void FixedUpdate()
    {
        if (_freeToAct && Input.GetKey(KeyCode.LeftShift))
        {
            rb.MovePosition(rb.position + movementDirection * (speed / 3) * Time.fixedDeltaTime);
        }
        else if (_freeToAct)
        {
            rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
        }
    }
}
