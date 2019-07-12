using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereElement : MonoBehaviour
{

    private SphereCenter center;

    public float distance = 100;

    public Vector3 lookAtWorldPosition;

    void LateUpdate()
    {

        if (center != null)
        {
            Vector3 direction = (transform.position - center.transform.position).normalized;

            transform.up = direction;

            Ray ray = new Ray(center.transform.position, direction);
            Vector3 point = ray.GetPoint(distance);

            transform.position = point;

            Plane plane = new Plane(direction, point);
            ray = new Ray(lookAtWorldPosition, direction);
            float d;
            plane.Raycast(ray, out d);
            point = ray.GetPoint(d);

            transform.LookAt(point, transform.up);

        }
    }
}
