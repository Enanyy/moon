using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRectangle : MonoBehaviour
{
    public float width = 10;
    public float height = 20;
    public Rectangle rectangle;

    public TestRectangle other;

    //public Vector3 a, b, c, d;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.z);
        Vector2 right = new Vector2(transform.right.x, transform.right.z);
        Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);

        rectangle.a = position - right * width * 0.5f;
        rectangle.b = position + right * width * 0.5f;
        rectangle.c = rectangle.b + forward * height;
        rectangle.d = rectangle.a + forward * height;

        if (other != null)
        {
            if (rectangle.Intersect(other.rectangle))
            {
                Debug.Log(string.Format("{0} Intersect {1}", gameObject.name, other.gameObject.name));
            }
            else if (other.rectangle.Intersect(rectangle))
            {
                Debug.Log(string.Format("{0} Intersect {1}", other.gameObject.name, gameObject.name));

            }

            if (rectangle.Contains(other.rectangle.center))
            {
                Debug.Log(string.Format("{0} Contains {1}", gameObject.name, other.gameObject.name));

            }
            else if (other.rectangle.Contains(rectangle.center))
            {
                Debug.Log(string.Format("{0} Contains {1}", other.gameObject.name, gameObject.name));

            }
           
        }

        //Vector2 aa = new Vector2(a.x,a.z);
        //Vector2 bb = new Vector2(b.x, b.z);
        //Vector2 cc = new Vector2(c.x, c.z);
        //Vector2 dd = new Vector2(d.x, d.z);

        //if (GeometryMath.SegmentIntersect(aa, bb, cc, dd))
        //{
        //    Debug.Log("AAA");
        //}
    }

    void OnDrawGizmos()
    {
        Vector3 a = new Vector3(rectangle.a.x, 0, rectangle.a.y);
        Vector3 b = new Vector3(rectangle.b.x, 0, rectangle.b.y);
        Vector3 c = new Vector3(rectangle.c.x, 0, rectangle.c.y);
        Vector3 d = new Vector3(rectangle.d.x, 0, rectangle.d.y);

        Vector3 center = new Vector3(rectangle.center.x,0,rectangle.center.y);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(a,b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
        Gizmos.DrawSphere(center,0.1f);
        
      
    }
}
