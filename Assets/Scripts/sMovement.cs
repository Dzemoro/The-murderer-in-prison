using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMovement : MonoBehaviour
{

    private float speed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 movementDirection;

    // Start is called before the first frame update
    void Start()
    {
        speed = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = movementDirection * (speed/3);
        }
        else
        {
            rb.velocity = movementDirection * speed;
        }
    }
}
