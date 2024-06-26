using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMovement : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 movementDirection;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.up = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y); 

        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDirection.Normalize();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.MovePosition(rb.position + movementDirection * (speed / 3) * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
        }
    }
}
