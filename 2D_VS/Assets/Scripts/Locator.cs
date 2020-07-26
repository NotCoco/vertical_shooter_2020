using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    LayerMask mask;
    public bool seeCannon = false;
    public Vector3 cannonPos;
    private float lostTimer = 1.00f;
    private Enemy parent;
    bool pathFound = false;
    // Start is called before the first frame update
    void Start()
    {
        mask = LayerMask.GetMask("Battleground", "Tilemap");
        parent = gameObject.transform.parent.gameObject.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(new Vector3(0, 0, 700.00f * Time.deltaTime));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), 2000.00f, mask);
        if (hit.collider != null)
        {
            CannonController c = hit.collider.GetComponent<CannonController>();
            if (c != null)
            {
                pathFound = false;
                seeCannon = true;
                cannonPos = hit.collider.gameObject.transform.position;
                lostTimer = 1.00f;
            }
            else
            {
                lostTimer -= Time.deltaTime;
                if (lostTimer < 0 && !pathFound)
                {
                    parent.latestPath.Clear();
                    parent.getPathToCannon();
                    pathFound = true;
                    seeCannon = false;
                }
            }
        }
    }
}
