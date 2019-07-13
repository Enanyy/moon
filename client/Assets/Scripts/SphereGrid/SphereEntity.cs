﻿using UnityEngine;
using System.Collections.Generic;

public class SphereEntity : MonoBehaviour
{

    private SphereGrid grid;
    public float moveSpeed = 10;


    public Tile current;

    public Stack<Tile> paths = new Stack<Tile>();

    void Awake()
    {
        grid = FindObjectOfType<SphereGridTest>().grid;
    }

    public void SetTile(Tile tile)
    {
        current = tile;
        grid = current.grid;
        
    }
     
  

    public void MoveTo(Tile tile)
    {
        grid = tile.grid;
        if (current == null)
        {
            paths.Push(tile);
        }
        else
        {
            paths = grid.FindPath(current, tile, (t) => { return grid.tilesType.ContainsKey(t.index) && grid.tilesType[t.index] == TileType.Free; });
        }
    }

    void Update()
    {
        if (grid != null)
        {

            if (paths.Count > 0)
            {
                current = paths.Peek();

                Vector3 destination = current.center;

                float displacement = Time.deltaTime * moveSpeed;

                Vector3 worldPosition = grid.root.TransformPoint(destination);

                float distance = Vector3.Distance(worldPosition, transform.position);
                if (distance > displacement)
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
                    paths.Pop();
                }
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
