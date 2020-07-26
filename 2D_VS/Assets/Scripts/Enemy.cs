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
    public Stack<Tuple<int, int>> latestPath;
    private Locator locator;
    private Rigidbody2D rigidbody;
    private Vector3 oldPos;
    LayerMask mask;
    int bugCounter = 0;
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
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        mask = LayerMask.GetMask("Battleground", "Tilemap");
        oldPos = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        lookTowardsCannon();
        // if (pathing == true && pathingTimer < 0)
        // {
        //     latestPath = getPathToCannon();
        //     pathingTimer = 10f;
        // }
        // else
        // {
        //     pathingTimer -= Time.deltaTime;
        // }

    }

    void FixedUpdate()
    {
        if (pathing == true)
        {
            if (!locator.seeCannon)
            {
                if (latestPath.Count != 0)
                {
                    var targetPoint2 = latestPath.Peek();
                    Vector3 targetPoint = tMap.CellToWorld(new Vector3Int(pathfinder.mapToTile_X(targetPoint2.Item1), pathfinder.mapToTile_Y(targetPoint2.Item2), 0));
                    targetPoint.x += 0.151909f;
                    targetPoint.y += 0.161415f;
                    if (rigidbody.position.x == targetPoint.x && rigidbody.position.y == targetPoint.y)
                    {
                        latestPath.Pop();
                    }
                    else
                    {
                        if (targetPoint == oldPos)
                        {
                            bugCounter++;
                        }
                        if (bugCounter < 120)
                        {
                            rigidbody.position = Vector3.MoveTowards(rigidbody.position, new Vector3(targetPoint.x, targetPoint.y, 0), speed * 1.70f * Time.deltaTime);
                        }
                        else
                        {
                            latestPath.Pop();
                            bugCounter = 0;
                        }
                        Debug.Log(targetPoint);
                    }
                    oldPos = targetPoint;
                }
                else
                {
                    latestPath = getPathToCannon();
                    Debug.Log("I'm stuck lol");
                }
            }
            else
            {
                latestPath.Clear();
                rigidbody.position = Vector3.MoveTowards(rigidbody.position, locator.cannonPos, speed * 1.70f * Time.deltaTime);

            }
        }
        if (attackTimer < 0 && !attacking)
        {
            attacking = true;

        }
        else if (attacking)
        {

            attackTimer = 2.00f;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    public Stack<Tuple<int, int>> getPathToCannon()
    {
        Vector3Int enemyPos = tMap.WorldToCell(transform.position);
        Vector3Int cPos = tMap.WorldToCell(cannon.transform.position);
        Stack<Tuple<int, int>> path = pathfinder.aStarSearch(Tuple.Create(enemyPos.x, enemyPos.y), Tuple.Create(cPos.x, cPos.y));
        pathfinder.printPath(path);
        latestPath = path;
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
