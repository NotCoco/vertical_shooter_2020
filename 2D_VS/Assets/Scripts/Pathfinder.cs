using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    Tilemap tilemap;

    int Max_X = 8;
    int Min_X = -8;
    int Min_Y = -15;
    int Max_Y = 16;
    BoundsInt bounds;
    private float dislikeCost = 100.00f;

    //List<List<A_Tile>> map;
    // Start is called before the first frame update
    void Awake()
    {

        tilemap = GetComponent<Tilemap>();
        bounds = tilemap.cellBounds;
    }
    void Start()
    {

    }

    List<List<A_Tile>> makeMap()
    {
        List<List<A_Tile>> map = new List<List<A_Tile>>(16);
        for (int i = Min_X; i <= Max_X; i++)
        {
            map.Add(new List<A_Tile>(31));
            for (int j = Min_Y; j <= Max_Y; j++)
            {
                if (tilemap.GetColliderType(new Vector3Int(i, j, 0)) == Tile.ColliderType.None)
                {
                    map[tileToMap_X(i)].Add(new A_Tile(1));
                }
                else
                {
                    map[tileToMap_X(i)].Add(new A_Tile(0));
                }
            }
        }

        for (int i = Min_X; i <= Max_X; i++)
        {
            for (int j = Min_Y; j <= Max_Y; j++)
            {
                if (tilemap.GetColliderType(new Vector3Int(i, j, 0)) == Tile.ColliderType.Sprite)
                {
                    // if the current tile is blocked then all of it's manhattan surrounding blocks should also be blocked
                    if (j + 1 <= Max_Y) { map[tileToMap_X(i)][tileToMap_Y(j + 1)].dislikeTile = true; } // North
                    if (i + 1 <= Max_X && j + 1 <= Max_Y) { map[tileToMap_X(i + 1)][tileToMap_Y(j + 1)].dislikeTile = true; } // North-East
                    if (i + 1 <= Max_X) { map[tileToMap_X(i + 1)][tileToMap_Y(j)].dislikeTile = true; } // East
                    if (i + 1 <= Max_X && j - 1 >= Min_Y) { map[tileToMap_X(i + 1)][tileToMap_Y(j - 1)].dislikeTile = true; } // South-East
                    if (j - 1 >= Min_Y) { map[tileToMap_X(i)][tileToMap_Y(j - 1)].dislikeTile = true; } // South
                    if (i - 1 >= Min_X && j - 1 >= Min_Y) { map[tileToMap_X(i - 1)][tileToMap_Y(j - 1)].dislikeTile = true; } // South-West
                    if (i - 1 >= Min_X) { map[tileToMap_X(i - 1)][tileToMap_Y(j)].dislikeTile = true; } // West
                    if (i - 1 >= Min_X && j + 1 <= Max_Y) { map[tileToMap_X(i - 1)][tileToMap_Y(j + 1)].dislikeTile = true; } // North-West
                }
            }
        }
        return map;
    }
    bool isValid(int x, int y)
    {
        return (x <= tileToMap_X(Max_X) && x >= 0) && (y <= tileToMap_Y(Max_Y) && y >= 0);
    }
    bool isUnblocked(int x, int y, List<List<A_Tile>> map)
    {
        return map[x][y].isBlocked == false;
    }
    bool isDestination(int x, int y, Tuple<int, int> dest)
    {
        if (x == dest.Item1 && y == dest.Item2)
            return (true);
        else
            return (false);
    }
    float calculateHValue(int x, int y, Tuple<int, int> dest)
    {
        return ((float)Math.Sqrt((x - dest.Item1) * (x - dest.Item1)
                              + (y - dest.Item2) * (y - dest.Item2)));
    }

    Stack<Tuple<int, int>> tracePath(Tuple<int, int> dest, List<List<A_Tile>> map)
    {
        int x = dest.Item1;
        int y = dest.Item2;
        Stack<Tuple<int, int>> path = new Stack<Tuple<int, int>>();
        while ((x != -1 && y != -1) && !(map[x][y].parent_i == x && map[x][y].parent_j == y))
        {
            path.Push(Tuple.Create(x, y));
            int temp_x = map[x][y].parent_i;
            int temp_y = map[x][y].parent_j;
            x = temp_x;
            y = temp_y;
        }
        path.Push(Tuple.Create(x, y));
        return path;
    }

    public void printPath(Stack<Tuple<int, int>> pathReal)
    {
        Stack<Tuple<int, int>> path = new Stack<Tuple<int, int>>(pathReal);
        Vector3Int tempVec = new Vector3Int(mapToTile_X(path.Peek().Item1), mapToTile_Y(path.Peek().Item2), 0);
        while (!(path.Count == 0))
        {
            Tuple<int, int> p = path.Peek();
            path.Pop();
            //Debug.Log("-> (" + mapToTile_X(p.Item1) + ", " + mapToTile_Y(p.Item2) + ")");
            Debug.DrawLine(tilemap.GetCellCenterWorld(tempVec), tilemap.GetCellCenterWorld(new Vector3Int(mapToTile_X(p.Item1), mapToTile_Y(p.Item2), 0)), Color.red, 10.00f);
            tempVec = new Vector3Int(mapToTile_X(p.Item1), mapToTile_Y(p.Item2), 0);
        }
    }

    public Stack<Tuple<int, int>> aStarSearch(Tuple<int, int> src, Tuple<int, int> dest)
    {
        src = Tuple.Create(tileToMap_X(src.Item1), tileToMap_Y(src.Item2));
        dest = Tuple.Create(tileToMap_X(dest.Item1), tileToMap_Y(dest.Item2));
        List<List<A_Tile>> map = makeMap();
        if (isValid(src.Item1, src.Item2) == false)
        {
            Debug.Log("Source is invalid\n");
            return null;
        }
        if (isValid(dest.Item1, dest.Item2) == false)
        {
            Debug.Log("Destination is invalid\n");
            return null;
        }
        if (isUnblocked(src.Item1, src.Item2, map) == false ||
            isUnblocked(dest.Item1, dest.Item2, map) == false)
        {
            Debug.Log("Source or the destination is blocked\n");
            return null;
        }
        if (isDestination(src.Item1, src.Item2, dest) == true)
        {
            Debug.Log("We are already at the destination\n");
            return null;
        }

        List<List<bool>> closedList = new List<List<bool>>(16);
        for (int a = 0; a <= 16; a++)
        {
            closedList.Add(new List<bool>(31));
            for (int b = 0; b <= 31; b++)
            {
                bool c = false;
                closedList[a].Add(c);
            }
        }

        int i = src.Item1;
        int j = src.Item2;
        map[i][j].f = 0.00f;
        map[i][j].g = 0.00f;
        map[i][j].h = 0.00f;
        map[i][j].parent_i = i;
        map[i][j].parent_j = j;

        SortedSet<Tuple<float, Tuple<int, int>>> openList = new SortedSet<Tuple<float, Tuple<int, int>>>();
        openList.Add(Tuple.Create(0.00f, Tuple.Create(i, j)));
        bool foundDest = false;

        while (!(openList.Count == 0))
        {
            Tuple<float, Tuple<int, int>> p = openList.First();
            openList.Remove(openList.First());
            i = p.Item2.Item1;
            j = p.Item2.Item2;
            closedList[i][j] = true;

            float gNew;
            float hNew;
            float fNew;

            // Successor #1 (North)
            if (isValid(i, j + 1) == true)
            {
                if (isDestination(i, j + 1, dest) == true)
                {
                    map[i][j + 1].parent_i = i;
                    map[i][j + 1].parent_j = j;
                    foundDest = true;
                    return tracePath(dest, map);
                }
                else if (closedList[i][j + 1] == false && isUnblocked(i, j + 1, map) == true)
                {
                    if (map[i][j + 1].dislikeTile == false)
                    {
                        gNew = map[i][j].g + 1.00f;
                        hNew = calculateHValue(i, j + 1, dest);
                        fNew = gNew + hNew;
                    }
                    else
                    {
                        gNew = map[i][j].g + dislikeCost;
                        hNew = calculateHValue(i, j + 1, dest);
                        fNew = gNew + hNew;
                    }

                    if (map[i][j + 1].f == float.MaxValue || map[i][j + 1].f > fNew)
                    {
                        openList.Add(Tuple.Create(fNew, Tuple.Create(i, j + 1)));
                        map[i][j + 1].f = fNew;
                        map[i][j + 1].g = gNew;
                        map[i][j + 1].h = hNew;
                        map[i][j + 1].parent_i = i;
                        map[i][j + 1].parent_j = j;
                    }
                }
            }
            // Successor #2 (North-East)
            if (isValid(i + 1, j + 1) == true)
            {
                if (isDestination(i + 1, j + 1, dest) == true)
                {
                    map[i + 1][j + 1].parent_i = i;
                    map[i + 1][j + 1].parent_j = j;
                    foundDest = true;
                    return tracePath(dest, map);
                }
                else if (closedList[i + 1][j + 1] == false && isUnblocked(i + 1, j + 1, map) == true)
                {
                    if (map[i + 1][j + 1].dislikeTile == false)
                    {
                        gNew = map[i][j].g + 1.00f;
                        hNew = calculateHValue(i + 1, j + 1, dest);
                        fNew = gNew + hNew;
                    }
                    else
                    {
                        gNew = map[i][j].g + dislikeCost;
                        hNew = calculateHValue(i + 1, j + 1, dest);
                        fNew = gNew + hNew;
                    }

                    if (map[i + 1][j + 1].f == float.MaxValue || map[i + 1][j + 1].f > fNew)
                    {
                        openList.Add(Tuple.Create(fNew, Tuple.Create(i + 1, j + 1)));
                        map[i + 1][j + 1].f = fNew;
                        map[i + 1][j + 1].g = gNew;
                        map[i + 1][j + 1].h = hNew;
                        map[i + 1][j + 1].parent_i = i;
                        map[i + 1][j + 1].parent_j = j;
                    }
                }
            }
            // Successor #3 (East)
            if (isValid(i + 1, j) == true)
            {
                if (isDestination(i + 1, j, dest) == true)
                {
                    map[i + 1][j].parent_i = i;
                    map[i + 1][j].parent_j = j;
                    foundDest = true;
                    return tracePath(dest, map);
                }
                else if (closedList[i + 1][j] == false && isUnblocked(i + 1, j, map) == true)
                {
                    if (map[i + 1][j].dislikeTile == false)
                    {
                        gNew = map[i][j].g + 1.00f;
                        hNew = calculateHValue(i + 1, j, dest);
                        fNew = gNew + hNew;
                    }
                    else
                    {
                        gNew = map[i][j].g + dislikeCost;
                        hNew = calculateHValue(i + 1, j, dest);
                        fNew = gNew + hNew;
                    }

                    if (map[i + 1][j].f == float.MaxValue || map[i + 1][j].f > fNew)
                    {
                        openList.Add(Tuple.Create(fNew, Tuple.Create(i + 1, j)));
                        map[i + 1][j].f = fNew;
                        map[i + 1][j].g = gNew;
                        map[i + 1][j].h = hNew;
                        map[i + 1][j].parent_i = i;
                        map[i + 1][j].parent_j = j;
                    }
                }
            }
            // Successor #4 (South-East)
            if (isValid(i + 1, j - 1) == true)
            {
                if (isDestination(i + 1, j - 1, dest) == true)
                {
                    map[i + 1][j - 1].parent_i = i;
                    map[i + 1][j - 1].parent_j = j;
                    foundDest = true;
                    return tracePath(dest, map);
                }
                else if (closedList[i + 1][j - 1] == false && isUnblocked(i + 1, j - 1, map) == true)
                {
                    if (map[i + 1][j - 1].dislikeTile == false)
                    {
                        gNew = map[i][j].g + 1.00f;
                        hNew = calculateHValue(i + 1, j - 1, dest);
                        fNew = gNew + hNew;
                    }
                    else
                    {
                        gNew = map[i][j].g + dislikeCost;
                        hNew = calculateHValue(i + 1, j - 1, dest);
                        fNew = gNew + hNew;
                    }

                    if (map[i + 1][j - 1].f == float.MaxValue || map[i + 1][j - 1].f > fNew)
                    {
                        openList.Add(Tuple.Create(fNew, Tuple.Create(i + 1, j - 1)));
                        map[i + 1][j - 1].f = fNew;
                        map[i + 1][j - 1].g = gNew;
                        map[i + 1][j - 1].h = hNew;
                        map[i + 1][j - 1].parent_i = i;
                        map[i + 1][j - 1].parent_j = j;
                    }
                }
            }
            // Successor #5 (South)
            if (isValid(i, j - 1) == true)
            {
                if (isDestination(i, j - 1, dest) == true)
                {
                    map[i][j - 1].parent_i = i;
                    map[i][j - 1].parent_j = j;
                    foundDest = true;
                    return tracePath(dest, map);
                }
                else if (closedList[i][j - 1] == false && isUnblocked(i, j - 1, map) == true)
                {
                    if (map[i][j - 1].dislikeTile == false)
                    {
                        gNew = map[i][j].g + 1.00f;
                        hNew = calculateHValue(i, j - 1, dest);
                        fNew = gNew + hNew;
                    }
                    else
                    {
                        gNew = map[i][j].g + dislikeCost;
                        hNew = calculateHValue(i, j - 1, dest);
                        fNew = gNew + hNew;
                    }

                    if (map[i][j - 1].f == float.MaxValue || map[i][j - 1].f > fNew)
                    {
                        openList.Add(Tuple.Create(fNew, Tuple.Create(i, j - 1)));
                        map[i][j - 1].f = fNew;
                        map[i][j - 1].g = gNew;
                        map[i][j - 1].h = hNew;
                        map[i][j - 1].parent_i = i;
                        map[i][j - 1].parent_j = j;
                    }
                }
            }
            // Successor #6 (South-West)
            if (isValid(i - 1, j - 1) == true)
            {
                if (isDestination(i - 1, j - 1, dest) == true)
                {
                    map[i - 1][j - 1].parent_i = i;
                    map[i - 1][j - 1].parent_j = j;
                    foundDest = true;
                    return tracePath(dest, map);
                }
                else if (closedList[i - 1][j - 1] == false && isUnblocked(i - 1, j - 1, map) == true)
                {
                    if (map[i - 1][j - 1].dislikeTile == false)
                    {
                        gNew = map[i][j].g + 1.00f;
                        hNew = calculateHValue(i - 1, j - 1, dest);
                        fNew = gNew + hNew;
                    }
                    else
                    {
                        gNew = map[i][j].g + dislikeCost;
                        hNew = calculateHValue(i - 1, j - 1, dest);
                        fNew = gNew + hNew;
                    }

                    if (map[i - 1][j - 1].f == float.MaxValue || map[i - 1][j - 1].f > fNew)
                    {
                        openList.Add(Tuple.Create(fNew, Tuple.Create(i - 1, j - 1)));
                        map[i - 1][j - 1].f = fNew;
                        map[i - 1][j - 1].g = gNew;
                        map[i - 1][j - 1].h = hNew;
                        map[i - 1][j - 1].parent_i = i;
                        map[i - 1][j - 1].parent_j = j;
                    }
                }
            }
            // Successor #7 (West)
            if (isValid(i - 1, j) == true)
            {
                if (isDestination(i - 1, j, dest) == true)
                {
                    map[i - 1][j].parent_i = i;
                    map[i - 1][j].parent_j = j;
                    foundDest = true;
                    return tracePath(dest, map);
                }
                else if (closedList[i - 1][j] == false && isUnblocked(i - 1, j, map) == true)
                {
                    if (map[i - 1][j].dislikeTile == false)
                    {
                        gNew = map[i][j].g + 1.00f;
                        hNew = calculateHValue(i - 1, j, dest);
                        fNew = gNew + hNew;
                    }
                    else
                    {
                        gNew = map[i][j].g + dislikeCost;
                        hNew = calculateHValue(i - 1, j, dest);
                        fNew = gNew + hNew;
                    }

                    if (map[i - 1][j].f == float.MaxValue || map[i - 1][j].f > fNew)
                    {
                        openList.Add(Tuple.Create(fNew, Tuple.Create(i - 1, j)));
                        map[i - 1][j].f = fNew;
                        map[i - 1][j].g = gNew;
                        map[i - 1][j].h = hNew;
                        map[i - 1][j].parent_i = i;
                        map[i - 1][j].parent_j = j;
                    }
                }
            }
            // Successor #8 (North-West)
            if (isValid(i - 1, j + 1) == true)
            {
                if (isDestination(i - 1, j + 1, dest) == true)
                {
                    map[i - 1][j + 1].parent_i = i;
                    map[i - 1][j + 1].parent_j = j;
                    foundDest = true;
                    return tracePath(dest, map);
                }
                else if (closedList[i - 1][j + 1] == false && isUnblocked(i - 1, j + 1, map) == true)
                {
                    if (map[i - 1][j + 1].dislikeTile == false)
                    {
                        gNew = map[i][j].g + 1.00f;
                        hNew = calculateHValue(i - 1, j + 1, dest);
                        fNew = gNew + hNew;
                    }
                    else
                    {
                        gNew = map[i][j].g + dislikeCost;
                        hNew = calculateHValue(i - 1, j + 1, dest);
                        fNew = gNew + hNew;
                    }

                    if (map[i - 1][j + 1].f == float.MaxValue || map[i - 1][j + 1].f > fNew)
                    {
                        openList.Add(Tuple.Create(fNew, Tuple.Create(i - 1, j + 1)));
                        map[i - 1][j + 1].f = fNew;
                        map[i - 1][j + 1].g = gNew;
                        map[i - 1][j + 1].h = hNew;
                        map[i - 1][j + 1].parent_i = i;
                        map[i - 1][j + 1].parent_j = j;
                    }
                }
            }
        }
        if (foundDest == false)
            Debug.Log("Failed to find the Destination Cell\n");

        return null;
    }
    public int mapToTile_X(int x)
    {
        return x - 8;
    }

    public int mapToTile_Y(int y)
    {
        return y - 15;
    }

    public int tileToMap_X(int x)
    {
        return x + 8;
    }

    public int tileToMap_Y(int y)
    {
        return y + 15;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
