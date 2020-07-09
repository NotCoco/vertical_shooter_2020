using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    private Vector2 startJPos; // the starting position of the joystick
    private Vector2 endJPos; // the ending position
    private bool touchStart = false; // joystick movement bool
    // Start is called before the first frame update
    // Start is called before the first frame update
    private Rigidbody2D rigbody;

    public float speed = 0.1f;
    void Start()
    {
        rigbody = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { // when the screen is initially tapped
            touchStart = true;
            //startJPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.00f));
            startJPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButton(0))
        {
            //endJPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.00f));
            endJPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else
        {
            touchStart = false;
        }
    }

    private void FixedUpdate()
    {
        if (touchStart)
        {
            Vector2 offset = endJPos - startJPos;
            Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);

            Vector2 t = transform.position;
            rigbody.MovePosition(t + direction * 0.08f);

        }
    }
}
