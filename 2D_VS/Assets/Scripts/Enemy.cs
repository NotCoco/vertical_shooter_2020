using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public int collisionDamage = 1;
    public int attackDamage = 1;
    public float dashWait = 2.00f;
    public float dashTimer = 1.00f;
    private bool waiting = false;
    private bool dashing = false;
    private float DASHTIMER = 1.30f;
    private float TARGETRANGE = 4.00f;
    public float speed;
    public bool pathing;
    private float pathingTimer = 10.0f;
    private Vector3 cannonPreDashPos;
    public GameObject cannon;
    public GameObject tilemap;
    private Tilemap tMap;
    private Pathfinder pathfinder;
    public Stack<Tuple<int, int>> latestPath;
    private Locator locator;
    private Rigidbody2D rigidbody;
    LayerMask mask;
    int bugCounter = 0;
    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
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
        mask = LayerMask.GetMask("CannonLayer", "Tilemap");
        cannonPreDashPos = new Vector3(0, 0, 0); // this vector represents the line the enemy will dash across 
    }

    // Update is called once per frame
    void Update()
    {
        lookTowardsCannon();
    }

    void FixedUpdate()
    {
        if (pathing == true)
        {
            if (!locator.seeCannon)
            {
                aStarPath();
            }
            else
            {
                latestPath.Clear();
                Vector2 r = new Vector2(locator.cannonPos.x, locator.cannonPos.y);

                if ((r - rigidbody.position).magnitude > TARGETRANGE)
                {
                    dashWait = DASHTIMER;
                    rigidbody.position = Vector3.MoveTowards(rigidbody.position, locator.cannonPos, speed * 2.00f * Time.deltaTime);
                }
                else
                {
                    if (dashing && dashTimer >= 0)
                    {
                        rigidbody.position = Vector3.MoveTowards(rigidbody.position, cannonPreDashPos, speed * 10.00f * Time.deltaTime);
                        dashTimer -= Time.deltaTime;
                    }
                    else
                    {
                        dashing = false;
                        dashTimer = 1.00f;

                    }
                    if (dashWait < 0)
                    {
                        cannonPreDashPos = locator.cannonPos;
                        dashing = true;
                        dashWait = DASHTIMER;
                    }
                    if (!dashing)
                    {
                        dashWait -= Time.deltaTime;
                    }

                }
            }
        }

    }

    void aStarPath()
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
                rigidbody.position = Vector3.MoveTowards(rigidbody.position, new Vector3(targetPoint.x, targetPoint.y, 0), speed * 1.70f * Time.deltaTime);
            }
        }
        else
        {
            latestPath = getPathToCannon();
        }
    }

    public Stack<Tuple<int, int>> getPathToCannon()
    {
        Vector3Int enemyPos = tMap.WorldToCell(transform.position);
        Vector3Int cPos = tMap.WorldToCell(cannon.transform.position);
        Stack<Tuple<int, int>> path = pathfinder.aStarSearch(Tuple.Create(enemyPos.x, enemyPos.y), Tuple.Create(cPos.x, cPos.y));
        path.Pop();
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

    public void changeHealth(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth + damage, 0, maxHealth);
        if (currentHealth == 0)
        {
            Destroy();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CannonController cannon = other.gameObject.GetComponent<CannonController>();
        if (cannon != null)
        {
            cannon.changeHealth(-collisionDamage);
        }
    }
}
