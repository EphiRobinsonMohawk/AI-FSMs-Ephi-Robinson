using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Rigidbody2D rb2d;
    public float moveSpeed = 4f;

    private Vector2 input;

    private void Awake()
    {
        GameMan.player = this;
    }


    // Runs each frame
    public void Update() //Input goes here
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = worldPos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);



        //Movement Logic
        input.x = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            input.x = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            input.x = 1f;
        }

        input.y = 0f;

        if (Input.GetKey(KeyCode.S))
        {
            input.y = -1f;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            input.y = 1f;
        }

        input = input.normalized;
    }



    private void FixedUpdate() //Physics go here, runs every physics tick
    {
        rb2d.MovePosition(rb2d.position + input * moveSpeed * Time.fixedDeltaTime);
    }


}
