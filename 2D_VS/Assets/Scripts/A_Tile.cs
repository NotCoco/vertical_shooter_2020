using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Tile
{
    public int parent_i;
    public int parent_j;
    public float f;
    public float g;
    public float h;
    public bool isBlocked;
    public bool dislikeTile;
    // Start is called before the first frame update
    public A_Tile(int num)
    {
        if (num == 1)
        {
            isBlocked = false;
        }
        else
        {
            isBlocked = true;
        }
        f = float.MaxValue;
        g = float.MaxValue;
        h = float.MaxValue;
        parent_i = -1;
        parent_j = -1;
    }
}
