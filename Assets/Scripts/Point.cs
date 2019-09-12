using UnityEngine;
using System.Collections;

public class Point : MonoBehaviour {

	public Color color  = new Color(1, 0, 0, 1f);
	public Color colorSelected  = new Color(1, 1, 1, 1f);

	public float radius = 0.5f;

	public Point sibling;

	public bool useGravity = false;
	float gravityY = 0f;
	public float SpringLength = 3.0f;

	void OnDrawGizmos() {
		Gizmos.color = color;
		Gizmos.DrawCube(transform.position, new Vector3(0.25f, 0.25f, 0.25f));
		Gizmos.DrawWireSphere(transform.position, radius);
		if (sibling != null)
		{
			Gizmos.DrawLine(transform.position + new Vector3(radius, 0, 0), sibling.transform.position + new Vector3(sibling.radius, 0, 0));
            Gizmos.DrawLine(transform.position - new Vector3(radius, 0, 0), sibling.transform.position - new Vector3(sibling.radius, 0, 0));
            Gizmos.DrawLine(transform.position + new Vector3(0, radius, 0), sibling.transform.position + new Vector3(0, sibling.radius, 0));
            Gizmos.DrawLine(transform.position - new Vector3(0, radius, 0), sibling.transform.position - new Vector3(0, sibling.radius, 0));
            Gizmos.DrawLine(transform.position + new Vector3(0, 0, radius), sibling.transform.position + new Vector3(0, 0, sibling.radius));
            Gizmos.DrawLine(transform.position - new Vector3(0, 0, radius), sibling.transform.position - new Vector3(0, 0, sibling.radius));
		}
	}

	void Update()
	{
		if (useGravity)
		{
			var pos = transform.position;
			pos.y -= gravityY;
			transform.position = pos;
		}
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = colorSelected;
		Gizmos.DrawCube(transform.position, new Vector3(0.25f, 0.25f, 0.25f));
	}

}