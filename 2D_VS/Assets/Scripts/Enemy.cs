using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int collisionDamage = 1;
    public int attackDamage = 1;
    public float attackTimer = 2.00f;
    private bool attacking = false;
    public GameObject cannon;
    private Vector3 cannonPos;
    // Start is called before the first frame update
    void Awake()
    {
        cannonPos = cannon.transform.position;
        AimController.AddToList(this);
    }
    void Start()
    {
        Vector3 newDir = cannon.transform.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, newDir);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newDir = cannon.transform.position - transform.position;
        Quaternion rotated = Quaternion.FromToRotation(Vector3.up, newDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotated, 1.00f);
    }

    void FixedUpdate()
    {
        if (attackTimer < 0 && !attacking)
        {
            attacking = true;

        }
        else if (attacking)
        {


            //Debug.DrawRay(transform.position, newDirection, Color.red);
            attackTimer = 2.00f;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void Destroy()
    {
        AimController.RemoveFromList(this);
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        CannonController cannon = other.gameObject.GetComponent<CannonController>();
        if (cannon != null)
        {
            cannon.changeHealth(-collisionDamage);
        }
    }
}
