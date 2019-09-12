using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {


	public Point p0;
	public Point p1;

	public float GetRadius(Vector3 hitpoint)
	{
		float minRadius = Mathf.Min(p0.radius, p1.radius);
		float hitp0 = (p0.transform.position - hitpoint).sqrMagnitude;
		float hitp1 = (p1.transform.position - hitpoint).sqrMagnitude;
		if (hitp0 >= hitp1)
		{
			return p0.radius - (p0.radius - p1.radius) * (1 - hitp1 / hitp0);
		}
		else
		{
			return p1.radius - (p1.radius - p0.radius) * (1 - hitp0 / hitp1);
		}
		
	}

	public Vector3 origin
	{
		get { return p0.transform.position; }
	}

	public Vector3 direction
	{
		get { return (p1.transform.position - p0.transform.position).normalized; }
	}





}