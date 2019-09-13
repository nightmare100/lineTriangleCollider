using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour {

	public class CollisionResult 
	{
		public bool needFlip;
		public Vector3 hitPoint;

		//出力法线向量
		public Vector3 overNormal;
		public float overDistance;
	}

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
			Debug.Log("[Collision] : " + result.overDistance);
			DoColliderChange(result);
		}

	}

	/// <summary>
	/// 碰撞推送
	/// </summary>
	void DoColliderChange(CollisionResult collRes)
	{
		if (collRes.needFlip)
		{
			// 1. 求交叉直线 2点 上下关系
			float dotp0p1 = Vector3.Dot(line.p0.transform.position - Triangle.p0.transform.position, collRes.overNormal); 
			float dotp1p1 = Vector3.Dot(line.p1.transform.position - Triangle.p0.transform.position, collRes.overNormal);
			float overDis = 0;
			if (dotp1p1 > 0 && dotp0p1 < 0)
			{
				//line p1 在面上方
				overDis = Mathf.Abs(Vector3Util.DisPoint2Surface(line.p1.transform.position, Triangle.p0.transform.position, Triangle.p1.transform.position, Triangle.p2.transform.position) );
				overDis += line.p1.radius;
			}
			else if (dotp0p1 > 0 && dotp1p1 < 0)
			{
				//line p0 在面上方 不可能出现这种情况、 腿部骨骼的跟节点和hip在一个点
			}
			
			var currTip = Triangle.Current.position + collRes.overNormal * overDis;
			var sibTip = Triangle.Sibling.position + collRes.overNormal * overDis;
			Triangle.Current.position = ((currTip - Triangle.p0.transform.position).normalized * Triangle.p2.SpringLength) + Triangle.p0.transform.position;
			Triangle.Sibling.position = ((sibTip - Triangle.p0.transform.position).normalized * Triangle.p1.SpringLength) + Triangle.p0.transform.position;
			
		}
		else
		{
			var currTip = Triangle.Current.position + collRes.overNormal * collRes.overDistance;
			var sibTip = Triangle.Sibling.position + collRes.overNormal * collRes.overDistance;
			Triangle.Current.position = ((currTip - Triangle.p0.transform.position).normalized * Triangle.p2.SpringLength) + Triangle.p0.transform.position;
			Triangle.Sibling.position = ((sibTip - Triangle.p0.transform.position).normalized * Triangle.p1.SpringLength) + Triangle.p0.transform.position;

			//直接推
			// Triangle.Current.position += collRes.overNormal * collRes.overDistance;
			// Triangle.Sibling.position += collRes.overNormal * collRes.overDistance;
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

	public CollisionResult Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Line ray)
	{

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
			float disp0 = Vector3Util.DisPoint2Surface(ray.p0.transform.position, p1, p2, p3);
			float disp1 = Vector3Util.DisPoint2Surface(ray.p1.transform.position, p1, p2, p3);
			if (Mathf.Abs(disp0) < ray.p0.radius || Mathf.Abs(disp1) < ray.p1.radius)
			{
				float overDis = Mathf.Max(ray.p1.radius - disp1, ray.p0.radius - disp0);
				return new CollisionResult()
				{
					needFlip = false,
					overNormal = triNormal.normalized,
					overDistance = overDis
				};
			}

			//判断 是否在半径范围内
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
			return null; 
		}

		// intersection point
		var hittedPoint  = p1 + u * e1 + v * e2;

		if ((Vector3.Dot(e2, q) * invDet) > Mathf.Epsilon)
		{
			//做整体翻转
			//交叉
			return new CollisionResult()
			{
				needFlip = true,
				hitPoint = hittedPoint,
				overNormal = triNormal.normalized
			};
		}

		// No hit at all
		return null;
	}


}
