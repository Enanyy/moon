﻿using UnityEngine;
using System.Collections;

public class SphereEntity : MonoBehaviour
{

    public SphereGrid grid;
    public float moveSpeed = 6;

    private Vector3 destination;



    public void MoveTo(Vector3 worldPosition)
    {
        if (grid != null)
        {
           
            destination = grid.root.InverseTransformPoint(grid.Sample(worldPosition));
            
        }
    }

    public void MoveTo(Tile tile)
    {
        grid = tile.grid;
        Vector3 worldPosition = grid.root.TransformPoint(tile.center);
        MoveTo(worldPosition);
    }

    void Update()
    {
        if (grid != null)
        {
            float displacement = Time.deltaTime * moveSpeed;

            Vector3 worldPosition = grid.root.TransformPoint(destination);

            float distance = Vector3.Distance(worldPosition, transform.position);
            if(distance > displacement)
            {
                Vector3 normal = (transform.position - grid.root.position).normalized;

                Plane plane = new Plane(normal, transform.position);

                var ray = new Ray(worldPosition, normal);

                plane.Raycast(ray, out distance);

                Vector3 point = ray.GetPoint(distance);

                Vector3 dir = point - transform.position;

                Vector3 position = transform.position + dir.normalized * moveSpeed * Time.deltaTime;

                position = grid.Sample(position);

                normal = (position - grid.root.position).normalized;
                plane.SetNormalAndPosition(normal, position);

                ray.direction = normal;
                ray.origin = position;

                plane.Raycast(ray, out distance);

                point = ray.GetPoint(distance);

                dir = point - transform.position;

                var rotation = Quaternion.LookRotation(dir, normal);

                transform.SetPositionAndRotation(position, rotation);
                
            }
            else
            {
                transform.position = worldPosition;
            }
        }
    }

    public void RotateTo(Vector3 worldPosition)
    {
        if (grid != null)
        {
            Vector3 normal = (transform.position - grid.root.position).normalized;

            transform.up = normal;

            Plane plane = new Plane(normal, transform.position);

            var ray = new Ray(worldPosition, normal);

            float distance;

            plane.Raycast(ray, out distance);

            Vector3 point = ray.GetPoint(distance);

            Vector3 dir = point - transform.position;

            transform.rotation = Quaternion.LookRotation(dir, transform.up);
        }
    }
}
