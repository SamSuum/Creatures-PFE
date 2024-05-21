using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Camera cam;
    private Rigidbody2D rb;
    private bool isWrappingX;
    private bool isWrappingY;

    public float thrust = 3f;
    public Sprite ship;
    public Sprite shipAccelerate;
    // Start is called before the first frame update
    void Start()
    {
        rb =GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void FixedUpdate()
    {

        GetComponent<SpriteRenderer>().sprite = ship;
        if (Input.GetAxis("Vertical") > 0) 
        {
            rb.AddForce(transform.right * thrust, ForceMode2D.Impulse);
            GetComponent<SpriteRenderer>().sprite = shipAccelerate;
        }

    }
    void ScreenWrap()
    {
        var isVisible = GetComponent<SpriteRenderer>().isVisible;

        if (isVisible)
        {
            isWrappingX = false;
            isWrappingY = false;
            return;
        }

        Vector3 viewportPosition = cam.WorldToViewportPoint(transform.position);
        Vector3 newPosition = transform.position;

        if (!isWrappingX && (viewportPosition.x > 1 || viewportPosition.x < 0))
        {
            newPosition.x = -newPosition.x;

            isWrappingX = true;
        }

        if (!isWrappingY && (viewportPosition.y > 1 || viewportPosition.y < 0))
        {
            newPosition.y = -newPosition.y;

            isWrappingY = true;
        }

        transform.position = newPosition;
    }
}
