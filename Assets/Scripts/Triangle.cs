using UnityEngine;
using System.Collections;

public class Triangle : MonoBehaviour {

	public Color color  = new Color(1f, 1f, 0, 1f);
	public Point p0;
	public Point p1;
	public Point p2;
	public Material material;

	public Transform Current;
	public Transform Sibling;

	private Mesh mesh;
	Vector3 gravity = new Vector3(0, -0.005f, 0);

	public bool useGravity = false;

	void Start()
	{
		gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		mesh = GetComponent<MeshFilter>().mesh;


		p1.SpringLength = Vector3.Distance(p0.transform.position, p1.transform.position);
		p2.SpringLength = Vector3.Distance(p0.transform.position, p2.transform.position);
	}

	void Update()
	{
		// Draw Triangle
		mesh.Clear();
		mesh.vertices = new Vector3[] {p0.transform.position, p1.transform.position, p2.transform.position};
		mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
		mesh.triangles = new int[] {0, 1, 2};
		gameObject.GetComponent<MeshRenderer>().material = material;


		if (useGravity)
		{
			DoGravity(p1);
			DoGravity(p2);
		}
	}

	void DoGravity(Point pt)
	{
		var currTipPos = pt.transform.position;
		currTipPos += gravity;
		pt.transform.position = ((currTipPos - p0.transform.position).normalized * pt.SpringLength) + p0.transform.position;
	}



	void OnDrawGizmos() {
		Gizmos.color = color;
		Gizmos.DrawLine(p0.transform.position, p1.transform.position);
		Gizmos.DrawLine(p1.transform.position, p2.transform.position);
		Gizmos.DrawLine(p2.transform.position, p0.transform.position);
	}

}