using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    public static ArrayList objects;
    public GameObject cannon;
    void Awake()
    {
        objects = new ArrayList();
        //T[] asList = GameObject.FindObjectsOfType<Enemy>();
        //objects = new ArrayList(asList);
    }

    public static void AddToList(Enemy enemy)
    {
        objects.Add(enemy);
    }

    public static void RemoveFromList(Enemy enemy)
    {
        objects.Remove(enemy);
    }

    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Enemy closest = FindClosestEnemy();
            Debug.DrawLine(cannon.transform.position, closest.transform.position, Color.red, 10.00f);
            //Debug.Log(closest.transform.position.x + ", " + closest.transform.position.y);
        }

    }


    Enemy FindClosestEnemy()
    {
        float distanceClosest = Mathf.Infinity;
        Enemy closestEnemy = null;
        foreach (Enemy enemy in objects)
        {
            float distanceToEnemy = (enemy.transform.position - cannon.transform.position).sqrMagnitude;
            if (distanceToEnemy < distanceClosest)
            {
                closestEnemy = enemy;
                distanceClosest = distanceToEnemy;
            }
        }
        return closestEnemy;
    }
}
