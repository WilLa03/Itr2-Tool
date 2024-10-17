using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    
    private Vector2 moveDir;
    private Vector2 moveVec;
    public float speed;

    public Rigidbody rb;
    private bool enabled = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 move = Vector2.zero;
        if (moveDir != Vector2.zero)
        {
            move = moveDir.normalized * (speed * Time.deltaTime);
        }

        //transform.position += new Vector3(move.x, 0, move.y);
        rb.MovePosition(transform.position+ new Vector3(move.x, 0, move.y));
    }

    void OnMove(InputValue value) //call move in the created player
    {
        moveDir = value.Get<Vector2>().normalized;
    }
    void OnInventory(InputValue value) //call move in the created player
    {
        if (!enabled)
        {
            PlayerInventory.Instance.UIEnabled();
            enabled = true;
        }
        else
        {
            PlayerInventory.Instance.UIDisabled();
            enabled = false;
        }
    }
}
