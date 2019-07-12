using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SphereElement : MonoBehaviour
{
    public float offset;

    public float angle;

    void LateUpdate()
    {
        if (SphereCenter.Instance!= null)
        {
            Vector3 direction = (transform.position - SphereCenter.Instance.transform.position).normalized;

            transform.up = direction;

            Ray ray = new Ray(SphereCenter.Instance.transform.position, direction);
            Vector3 point = ray.GetPoint(SphereCenter.Instance.radius+ offset);

            transform.position = point;
   
            transform.Rotate(transform.up, angle, Space.World);

        }
    }
   
}
