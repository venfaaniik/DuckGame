using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    const float verticalLineGradient = 1e5f;

    float gradient;
    float y_intercept;
    Vector2 pointOnLine_01;
    Vector2 pointOnLine_02;

    float gradientPerdendicular;

    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dx == 0)
        {
            gradientPerdendicular = verticalLineGradient;
        }
        else
        {
            gradientPerdendicular = dy / dx;
        }

        if (gradientPerdendicular == 0 )
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -1 / gradientPerdendicular;
        }

        y_intercept = pointOnLine.y - gradient * pointOnLine.y;
        pointOnLine_01 = pointOnLine;
        pointOnLine_02 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    bool GetSide(Vector2 p)
    {
        return (p.x - pointOnLine_01.x) * (pointOnLine_02.y - pointOnLine_01.y) > (p.y - pointOnLine_01.y) * (pointOnLine_02.x - pointOnLine_01.x);
    }

    public bool HasCrossedLine(Vector2 p)
    {
       return GetSide(p) != approachSide;
    }

    public float DistanceFromPoint(Vector2 p)
    {
        float yInterceptPerpendicular = p.y - gradientPerdendicular * p.x;
        float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerdendicular);
        float intersectY = gradient * intersectX + y_intercept;
        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }

    public void DrawWithGizmos(float length)
    {
        //Vector3 lineDir = new Vector3 (1, gradient, 0).normalized;
		//Vector3 lineCentre = new Vector3 (pointOnLine_01.x, 0, pointOnLine_01.y) + Vector3.up;
		//Gizmos.DrawLine (lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
        //fuck you.
    }
}