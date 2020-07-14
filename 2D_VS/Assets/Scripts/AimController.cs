using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    public static ArrayList objects;
    public GameObject cannon;
    private Transform aimTransform;
    public GameObject projectilePref;
    private Enemy closestEnemy;
    private Vector3 aimDirection;
    float shootTime = 1.00f;
    float retarget_time = 0.20f;
    void Awake()
    {

        objects = new ArrayList();
        aimTransform = transform.Find("Cannon_Head");
        aimDirection = new Vector3(0, 0, -10);
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
        closestEnemy = FindClosestEnemy();
    }
    void Update()
    {
        if (retarget_time < 0)
        {
            closestEnemy = FindClosestEnemy();
            retarget_time = 0.20f;
        }
        Vector3 enemyPosition = closestEnemy.transform.position;
        aimDirection = (enemyPosition - cannon.transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle - 90);

        if (cannon.GetComponent<CannonController>().getIsMoving() == false)
        {
            if (shootTime < 0)
            {
                Launch(aimDirection);
                shootTime = 1.00f;
            }
        }
        else
        { // allows strafe shooting
            shootTime = 0.5f;
        }
        shootTime -= Time.deltaTime;
        retarget_time -= Time.deltaTime;
    }

    void Launch(Vector3 direction)
    {
        GameObject projectileObject = Instantiate(projectilePref, transform.position, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(direction, 300);

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
