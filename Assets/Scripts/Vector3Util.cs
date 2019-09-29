using UnityEngine;

public class Vector3Util
{

    /// <summary>
    /// 点到平面距离 调用U3D Plane类处理
    /// </summary>
    /// <param name="point"></param>
    /// <param name="surfacePoint1"></param>
    /// <param name="surfacePoint2"></param>
    /// <param name="surfacePoint3"></param>
    /// <returns></returns>
    public static float DisPoint2Surface(Vector3 point, Plane plane)
    {
        return plane.GetDistanceToPoint(point);
    }

    public static bool IsInTriAngle(Vector3 point,Vector3 a,Vector3 b,Vector3 c, Plane plane)
    {
        point = plane.ClosestPointOnPlane(point);
        Vector3 pa = a - point;
        Vector3 pb = b - point;
        Vector3 pc = c - point;
        Vector3 pab = Vector3.Cross(pa,pb);
        Vector3 pbc = Vector3.Cross(pb, pc);
        Vector3 pca = Vector3.Cross(pc, pa);
        
        float d1 = Vector3.Dot(pab, pbc);
        float d2 = Vector3.Dot(pab, pca);
        float d3 = Vector3.Dot(pbc, pca);

        if (d1 > 0 && d2 > 0 && d3 > 0) return true;
        return false;
    }

    /// <summary>
    /// 点到线距离
    /// </summary>
    /// <param name="point"></param>
    /// <param name="linePoint1"></param>
    /// <param name="linePoint2"></param>
    /// <returns></returns>
    public static float DistancePoint2Line(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        float fProj = Vector3.Dot(point - linePoint1, (linePoint1 - linePoint2).normalized);
        return Mathf.Sqrt((point - linePoint1).sqrMagnitude - fProj * fProj);
    }

    public static bool Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 line0, Vector3 line1)
    {
        bool lineCheck1 = FasterLineSegmentIntersection(new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z), new Vector2(line0.x, line0.z), new Vector2(line1.x, line1.z));
        bool lineCheck2 = FasterLineSegmentIntersection(new Vector2(p2.x, p2.z), new Vector2(p3.x, p3.z), new Vector2(line0.x, line0.z), new Vector2(line1.x, line1.z));
        bool lineCheck3 = FasterLineSegmentIntersection(new Vector2(p3.x, p3.z), new Vector2(p1.x, p1.z), new Vector2(line0.x, line0.z), new Vector2(line1.x, line1.z));

        return lineCheck1 || lineCheck2 || lineCheck3;
    }
    
    static bool FasterLineSegmentIntersection(Vector2 line1point1, Vector2 line1point2, Vector2 line2point1, Vector2 line2point2) 
    {
        Vector2 a = line1point2 - line1point1;
        Vector2 b = line2point1 - line2point2;
        Vector2 c = line1point1 - line2point1;
    
        float alphaNumerator = b.y * c.x - b.x * c.y;
        float betaNumerator  = a.x * c.y - a.y * c.x;
        float denominator    = a.y * b.x - a.x * b.y;
    
        if (denominator == 0) 
        {
            return false;
        } 
        else if (denominator > 0) 
        {
            if (alphaNumerator < 0 || alphaNumerator > denominator || betaNumerator < 0 || betaNumerator > denominator) 
            {
                return false;
            }
        } 
        else if (alphaNumerator > 0 || alphaNumerator < denominator || betaNumerator > 0 || betaNumerator < denominator) 
        {
            return false;
        }
        return true;
    }
}