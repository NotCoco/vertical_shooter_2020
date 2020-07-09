using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    public static ArrayList objects;
    public GameObject cannon;
    private Transform aimTransform;

    float retarget_time = 1.00f;
    void Awake()
    {

        objects = new ArrayList();
        aimTransform = transform.Find("Cannon_Head");
        //aimTransform = transform.Find("Aim");
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
        if (retarget_time < 0)
        {
            Enemy closest = FindClosestEnemy();
            Debug.DrawLine(cannon.transform.position, closest.transform.position, Color.red, 10.00f);
            Vector3 enemyPosition = closest.transform.position;
            Vector3 aimDirection = (enemyPosition - cannon.transform.position).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            aimTransform.eulerAngles = new Vector3(0, 0, angle - 90);
            retarget_time = 1.00f;
        }
        retarget_time -= Time.deltaTime;
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
