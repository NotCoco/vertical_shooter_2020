using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
}
