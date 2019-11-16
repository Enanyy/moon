using UnityEngine;
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
#if UNITY_EDITOR
            var it = paths.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.SetDefaultColor();
            }
#endif
            paths = grid.FindPath(current, tile, (t) => { return grid.tilesType.ContainsKey(t.index) && grid.tilesType[t.index] == TileType.Free; });
#if UNITY_EDITOR

            it = paths.GetEnumerator();
            while(it.MoveNext())
            { 
                it.Current.SetColor(Color.blue);
            }
#endif
        }
    }

    void Update()
    {
        if (grid != null)
        {

            if (paths.Count > 0)
            {
                current = paths.Peek();

                Vector3 destination = GetPathPoint(paths);

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

                    rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);

                    transform.SetPositionAndRotation(position, rotation);

                }
                else
                {
                    transform.position = worldPosition;
#if UNITY_EDITOR
                    current.SetDefaultColor();
#endif
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

    Vector3 GetPathPoint(Stack<Tile> paths)
    {
        if (paths.Count > 1)
        {
            var current = paths.Pop();
            var next = paths.Peek();
            paths.Push(current);

            var it = next.neihbors.GetEnumerator();
            while (it.MoveNext())
            {
                if (current.neihbors.ContainsKey(it.Current.Key))
                {
                    var edge = it.Current.Key;

                    return (edge.from + edge.to) / 2;
                }
            }

            return current.center;

        }
        else if(paths.Count > 0)
        {
            return paths.Peek().center;
        }

        return  Vector3.zero;
    }
}
