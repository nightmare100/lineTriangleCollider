using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour {

	public class CollisionResult 
	{
		public bool isCross;
		public Vector3 hitPoint;

		//出力法线向量
		public Vector3 overNormal;
		public float overDistance;

		public Plane plane;
	}

	CollisionResult currentResult = new CollisionResult();

	public Color lineColor  = new Color(1, 0, 0, 1f);
	public Color lineHitColor  = new Color(1, 1, 1, 1f);
	public Color triColor  = new Color(1, 1, 0, 1f);

	public Line line;
	public Triangle Triangle;

	private Vector3 hit;
	private bool collision = false;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		collision = false;

		CollisionResult result = Intersect(Triangle.p0.transform.position, Triangle.p1.transform.position, Triangle.p2.transform.position, line);
		if (result != null)
		{
			collision = true;
			hit = result.hitPoint;
			Debug.Log("[Collision] : " + result.overDistance + " [Flip]" + result.isCross);
			DoColliderChange(result);
		}

	}

	/// <summary>
	/// 碰撞推送
	/// </summary>
	void DoColliderChange(CollisionResult collRes)
	{
		if (collRes.isCross)
		{
			// 1. 求交叉直线 2点 上下关系
			float dotp0p1 = Vector3.Dot(line.p0.transform.position - Triangle.p0.transform.position, collRes.overNormal); 
			float dotp1p1 = Vector3.Dot(line.p1.transform.position - Triangle.p0.transform.position, collRes.overNormal);
			
			
			if (dotp1p1 > 0 && dotp0p1 < 0)
			{
				//line p1 在面上方
				CollisionResult res = CheckForRadius(collRes.plane, collRes.overNormal,Triangle.p0.transform.position, Triangle.p1.transform.position, Triangle.p2.transform.position, line);
				if (res != null)
				{
					currentResult = res;
					DoColliderChange(res);
				}
				return;
			}
			else if (dotp0p1 > 0 && dotp1p1 < 0)
			{
				//line p0 在面上方 不太可能出现这种情况 除非穿膜严重 
				Vector3 l2l1 = line.p0.transform.position - line.p1.transform.position;
				Vector3 ptl1 = line.p0.transform.position + l2l1.normalized * line.p0.radius;

				Vector3 s1l1 = Triangle.p0.transform.position - ptl1;
				Vector3 s1hit = Triangle.p0.transform.position - collRes.hitPoint;
				float ptAngle = Mathf.Acos(Vector3.Dot(s1l1.normalized, s1hit.normalized)) * Mathf.Rad2Deg;
				Vector3 rotateAix = Vector3.Cross(s1hit, s1l1);

				Quaternion rotChange = Quaternion.AngleAxis(ptAngle, rotateAix);
				Vector3 s2normal = (rotChange * (Triangle.p1.transform.position - Triangle.p0.transform.position)).normalized;
				Vector3 s3normal = (rotChange * (Triangle.p2.transform.position - Triangle.p0.transform.position)).normalized;

				Triangle.Current.position = (s3normal * Triangle.p2.SpringLength) + Triangle.p0.transform.position;
				Triangle.Sibling.position = (s2normal * Triangle.p1.SpringLength) + Triangle.p0.transform.position;
				return;

			}
		}
		else
		{
			var currTip = Triangle.Current.position + collRes.overNormal * collRes.overDistance;
			var sibTip = Triangle.Sibling.position + collRes.overNormal * collRes.overDistance;
			Triangle.Current.position = ((currTip - Triangle.p0.transform.position).normalized * Triangle.p2.SpringLength) + Triangle.p0.transform.position;
			Triangle.Sibling.position = ((sibTip - Triangle.p0.transform.position).normalized * Triangle.p1.SpringLength) + Triangle.p0.transform.position;
		}
		
	}


	void OnDrawGizmos() 
	{
		// Draw Line
		if ( collision )
		{
			Gizmos.color = lineColor;
			Gizmos.DrawLine(line.p0.transform.position, hit);

			Gizmos.color = lineHitColor;
			Gizmos.DrawLine(hit, line.p1.transform.position);
		}
		else
		{
			Gizmos.color = lineColor;
			Gizmos.DrawLine(line.p0.transform.position, line.p1.transform.position);
		}
	}

	CollisionResult CheckForRadius(Plane triPlane, Vector3 triNormal, Vector3 p1, Vector3 p2, Vector3 p3, Line ray)
	{
		var p0closer = triPlane.ClosestPointOnPlane(ray.p0.transform.position);
		var p1closer = triPlane.ClosestPointOnPlane(ray.p1.transform.position);


		if (!Vector3Util.Intersect(p1, p2, p3, p0closer, p1closer))
		{
			return null;
		}

		float disnowpt0 = triPlane.GetDistanceToPoint(ray.p0.transform.position);
		float disnowpt1 = triPlane.GetDistanceToPoint(ray.p1.transform.position);
		

		float displane1 = Vector3Util.DistancePoint2Line(p0closer, p2, p3);
		float displane2= Vector3Util.DistancePoint2Line(p1closer, p2, p3);
		float lineplanecompare = displane2 / (displane1 + displane2);
		float resultdis = disnowpt0 - (disnowpt0 - disnowpt1) * (1 - lineplanecompare);

		float currRadiu = ray.GetRadius(lineplanecompare);
		if (resultdis < currRadiu)
		{
			currentResult.isCross = false;
			currentResult.hitPoint = Vector3.zero;
			currentResult.overNormal = triNormal.normalized;
			currentResult.overDistance = currRadiu - resultdis;
			currentResult.plane = triPlane;
			return currentResult;
		}
		else
		{
			return null;
		}
	}

	public CollisionResult Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Line ray)
	{
		var triPlane = new Plane(p1, p2, p3);
		// Vectors from p1 to p2/p3 (edges)
		//Find vectors for edges sharing vertex/point p1
		Vector3 e1 = p2 - p1;
		Vector3 e2 = p3 - p1;

		// Calculate determinant
		Vector3 p = Vector3.Cross(ray.direction, e2);

		//Calculate determinat
		float det = Vector3.Dot(e1, p);

		//if determinant is near zero, ray lies in plane of triangle otherwise not
		//平形 检测
		if (det > -Mathf.Epsilon && det < Mathf.Epsilon) { return null; }

		//检测line是否在面的同一端
		Vector3 triNormal = Vector3.Cross(e2, e1);
		float dotp0p1 = Vector3.Dot(ray.p0.transform.position - p1, triNormal); 
		float dotp1p1 = Vector3.Dot(ray.p1.transform.position - p1, triNormal);
		if (dotp0p1 < 0 && dotp1p1 < 0 || dotp0p1 > 0 && dotp1p1 > 0)
		{
			//线段在面的一边
			float disp0 = Vector3Util.DisPoint2Surface(ray.p0.transform.position, triPlane);
			float disp1 = Vector3Util.DisPoint2Surface(ray.p1.transform.position, triPlane);
			bool p0intri = Vector3Util.IsInTriAngle(ray.p0.transform.position, p1, p2, p3, triPlane);
			bool p1intri = Vector3Util.IsInTriAngle(ray.p1.transform.position, p1, p2, p3, triPlane);
			if (Mathf.Abs(disp0) < ray.p0.radius && p0intri || 
				Mathf.Abs(disp1) < ray.p1.radius && p1intri)
			{
				float overDis = Mathf.Max(ray.p1.radius - disp1, ray.p0.radius - disp0);

				currentResult.isCross = false;
				currentResult.hitPoint = Vector3.zero;
				currentResult.overNormal = triNormal.normalized;
				currentResult.overDistance = overDis;
				currentResult.plane = triPlane;
				return currentResult;
			}
			else if (!p1intri && Mathf.Abs(disp1) < ray.p1.radius)
			{	
				// 这里还需要处理ray p1 不在三角形范围内的情况
				return CheckForRadius(triPlane, triNormal,p1, p2, p3, ray);
			}
			
			return null;
		}

		
		float invDet = 1.0f / det;

		//calculate distance from p1 to ray origin
		Vector3 t = ray.origin - p1;
	
		//Calculate u parameter
		float u = Vector3.Dot(t, p) * invDet;

		//Check for ray hit
		if (u < 0 || u > 1) { return null; }

		//Prepare to test v parameter
		Vector3 q = Vector3.Cross(t, e1);

		//Calculate v parameter
		float v = Vector3.Dot(ray.direction, q) * invDet;

		//Check for ray hit
		if (v < 0 || u + v > 1) 
		{
			// check for radius
			// 线2点在面2边 但未hit 检测半径内是否有hit
			return CheckForRadius(triPlane, triNormal,p1, p2, p3, ray);
		}

		// intersection point
		var hittedPoint  = p1 + u * e1 + v * e2;

		if ((Vector3.Dot(e2, q) * invDet) > Mathf.Epsilon)
		{
			//交叉
			currentResult.isCross = true;
			currentResult.hitPoint = hittedPoint;
			currentResult.overNormal = triNormal.normalized;
			currentResult.overDistance = 0;
			currentResult.plane = triPlane;
			return currentResult;
		}

		// No hit at all
		return null;
	}


}
