﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    private Vector2 startJPos; // the starting position of the joystick
    private Vector2 endJPos; // the ending position
    private bool touchStart = false; // joystick movement bool
    public bool isMoving = false;
    private Rigidbody2D rigbody;
    public int currentHealth = 5;
    public Transform outerJoy;
    public Transform innerJoy;
    public float cameraDistance = 10.00f;
    public float speed = 0.08f;
    void Start()
    {
        rigbody = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { // when the screen is initially tapped
            setTouchStart(true);
            setIsMoving(true);

            startJPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            innerJoy.transform.position = screenToWorldCoordinates(new Vector2(startJPos.x, startJPos.y));
            outerJoy.transform.position = screenToWorldCoordinates(new Vector2(startJPos.x, startJPos.y));
            spriteEnabled(innerJoy, true);
            spriteEnabled(outerJoy, true);
        }
        if (Input.GetMouseButton(0))
        {

            endJPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else
        {
            setTouchStart(false);
            setIsMoving(false);
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
                rigbody.MovePosition(t + direction * speed);
            }
            Vector2 realPos = new Vector2(startJPos.x + direction.x * Mathf.Abs(offset2.x), startJPos.y + direction.y * Mathf.Abs(offset2.y));

            innerJoy.transform.position = screenToWorldCoordinates(new Vector2(realPos.x - 7.00f, realPos.y));

        }
        else
        {
            spriteEnabled(innerJoy, false);
            spriteEnabled(outerJoy, false);
        }
    }

    private Vector3 screenToWorldCoordinates(Vector2 screenCoordinates)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(screenCoordinates.x, screenCoordinates.y, cameraDistance));
    }

    /**
    Sets the sprite renderer.enabled value to the given bool
    Takes tranform associated with the sprite
    **/
    private void spriteEnabled(Transform transformOfSprite, bool newValue)
    {
        transformOfSprite.GetComponent<SpriteRenderer>().enabled = newValue;
    }
    public void setTouchStart(bool b)
    {
        touchStart = b;
    }

    public bool getTouchStart()
    {
        return touchStart;
    }

    public void setIsMoving(bool b)
    {
        isMoving = b;
    }

    public bool getIsMoving()
    {
        return isMoving;
    }

    public void changeHealth(int damage)
    {
        currentHealth -= damage;
    }
}
