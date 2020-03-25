using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int Vertical;
    public static int Horizontal;
    public static int Columns, Rows;
    public static Vector3 GetGridPosition(int x, int y)
    {
        return new Vector3(x - (Horizontal - 0.5f), y - (Vertical - 0.5f));
    }
    public static int GetTile(int x, int y)
    {
        if (y == Rows - 1 && x == 0)
            return 0;
        else if (y == Rows - 1 && x != 0 && x != Columns - 1)
            return 1;
        else if (y == Rows - 1 && x == Columns - 1)
            return 2;
        else if (x == 0 && y != 0 && y != Rows - 1)
            return 3;
        else if (x == Columns - 1 && y != 0 && y != Rows - 1)
            return 5;
        else if (x == 0 && y == 0)
            return 6;
        if (x != 0 && x != Columns - 1 && y == 0)
            return 7;
        else if (x == Columns - 1 && y == 0)
            return 8;
        else
            return 4;
    }
}
