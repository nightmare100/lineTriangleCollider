using UnityEngine;

public class Vector3Util
{
    /// <summary>
    /// 点到直线距离
    /// </summary>
    /// <param name="point">点坐标</param>
    /// <param name="linePoint1">直线上一个点的坐标</param>
    /// <param name="linePoint2">直线上另一个点的坐标</param>
    /// <returns></returns>
    public static float DisPoint2Line(Vector3 point,Vector3 linePoint1,Vector3 linePoint2)
    {
        Vector3 vec1 = point - linePoint1;
        Vector3 vec2 = linePoint2 - linePoint1;
        Vector3 vecProj = Vector3.Project(vec1, vec2);
        float dis =  Mathf.Sqrt(Mathf.Pow(Vector3.Magnitude(vec1), 2) - Mathf.Pow(Vector3.Magnitude(vecProj), 2));
        return dis;
    }

    /// <summary>
    /// 点到平面距离 调用U3D Plane类处理
    /// </summary>
    /// <param name="point"></param>
    /// <param name="surfacePoint1"></param>
    /// <param name="surfacePoint2"></param>
    /// <param name="surfacePoint3"></param>
    /// <returns></returns>
    public static float DisPoint2Surface(Vector3 point, Vector3 surfacePoint1, Vector3 surfacePoint2, Vector3 surfacePoint3)
    {
        Plane plane = new Plane(surfacePoint1, surfacePoint2, surfacePoint3);
        return plane.GetDistanceToPoint(point);
    }
}