using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Enemy : MonoBehaviour
{
    public int collisionDamage = 1;
    public int attackDamage = 1;
    public float attackTimer = 2.00f;
    private bool attacking = false;
    public float speed;
    public bool pathing;
    private float pathingTimer = 10.0f;
    public GameObject cannon;
    public GameObject tilemap;
    private Tilemap tMap;
    private Pathfinder pathfinder;
    private Stack<Tuple<int, int>> latestPath;
    private Locator locator;
    // Start is called before the first frame update
    void Awake()
    {

    }
    void Start()
    {
        AimController.AddToList(this);
        Vector3 newDir = cannon.transform.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, newDir);
        tMap = tilemap.GetComponent<Tilemap>();
        pathfinder = tilemap.GetComponent<Pathfinder>();
        latestPath = getPathToCannon();
        locator = transform.GetChild(0).GetComponent<Locator>();
    }

    // Update is called once per frame
    void Update()
    {
        lookTowardsCannon();
        if (pathing == true && pathingTimer < 0)
        {
            latestPath = getPathToCannon();
            pathingTimer = 10f;
        }
        else
        {
            pathingTimer -= Time.deltaTime;
        }
        if (pathing == true)
        {
            if (!locator.seeCannon)
            {
                if (latestPath.Count != 0)
                {
                    var targetPoint2 = latestPath.Peek();
                    Vector3 targetPoint = tMap.CellToWorld(new Vector3Int(pathfinder.mapToTile_X(targetPoint2.Item1), pathfinder.mapToTile_Y(targetPoint2.Item2), 0));
                    if (transform.position.x == targetPoint.x && transform.position.y == targetPoint.y)
                    {
                        latestPath.Pop();
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPoint.x, targetPoint.y, 0), speed * Time.deltaTime);
                        //Debug.Log("hi 124455");
                    }
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, locator.cannonPos, speed * Time.deltaTime);
            }
        }
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

    Stack<Tuple<int, int>> getPathToCannon()
    {
        Vector3Int enemyPos = tMap.WorldToCell(transform.position);
        Vector3Int cPos = tMap.WorldToCell(cannon.transform.position);
        Stack<Tuple<int, int>> path = pathfinder.aStarSearch(Tuple.Create(enemyPos.x, enemyPos.y), Tuple.Create(cPos.x, cPos.y));
        pathfinder.printPath(path);
        return path;
    }

    void lookTowardsCannon()
    {
        Vector3 newDir = cannon.transform.position - transform.position;
        Quaternion rotated = Quaternion.FromToRotation(Vector3.up, newDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotated, 1.00f);
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
