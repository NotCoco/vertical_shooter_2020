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
    public int currentHealth = 5;
    public Transform outerJoy;
    public Transform innerJoy;

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
            innerJoy.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(startJPos.x, startJPos.y, 10.00f));
            outerJoy.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(startJPos.x, startJPos.y, 10.00f));
            innerJoy.GetComponent<SpriteRenderer>().enabled = true;
            outerJoy.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetMouseButton(0))
        {

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
            Vector2 offset2 = Vector2.ClampMagnitude(offset, 35.0f); // determines how far the inner joystick can move
            Vector2 t = transform.position;
            if (offset.magnitude > 30.00f)
            {
                rigbody.MovePosition(t + direction * 0.08f);
            }
            Vector2 realPos = new Vector2(startJPos.x + direction.x * Mathf.Abs(offset2.x), startJPos.y + direction.y * Mathf.Abs(offset2.y));

            innerJoy.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(realPos.x - 7.00f, realPos.y, 10.00f));

        }
        else
        {
            innerJoy.GetComponent<SpriteRenderer>().enabled = false;
            outerJoy.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void changeHealth(int damage)
    {
        currentHealth -= damage;
    }
}
