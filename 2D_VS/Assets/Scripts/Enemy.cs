using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int collisionDamage = 1;
    public int attackDamage = 1;
    // Start is called before the first frame update
    void Start()
    {
        AimController.AddToList(this);
    }

    // Update is called once per frame
    void Update()
    {

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
            cannon.changeHealth(collisionDamage);
        }
    }
}
