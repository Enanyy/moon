using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScalingSphere : MonoBehaviour {

    
   
    public float SphereRadius = 1;
    public int SphereDetail = 0;

    public Material material;


    public SphereGrid grid;

	// Use this for initialization
	void Awake () {
         grid = new SphereGrid();
        float time = Time.time;

        grid.Init(SphereRadius, SphereDetail);
        Debug.Log(Time.time - time);
        grid.Show(material);

        grid.root.position = transform.position;

        for (int i = 0; i < grid.roots.Count; ++i)
        {
            SetColor(grid.roots[i], new Color(i * 0.05f, i * 0.05f, i * 0.05f));
        }

    }

    void Update()
    {
       

        //index = Mathf.Clamp(index, 0, tiles.Count -1);
        //if (tiles.ContainsKey(index))
        //{
        //    var tile = tiles[index];
        //    for (int i = 0; i < tile.neihbors.Count; i++)
        //    {
        //        var t = tile.neihbors[i];
        //        var r = t.go.GetComponent<MeshRenderer>();
        //        r.material.SetColor("_Color",Color.green);
        //    }
        //}
    }
	
  
    void SetColor(Tile triangle, Color color)
    {
        if(triangle != null)
        {
            if(triangle.children.Count == 0)
            {
                if(triangle.go!= null)
                {
                    var r = triangle.go.GetComponent<MeshRenderer>();
                    r.material.SetColor("_Color", color);
                }
            }
            else
            {
                for(int i = 0; i < triangle.children.Count; ++i)

                {
                    SetColor(triangle.children[i], color);
                }
            }
        }
    }

    float Area(Vector2 one, Vector2 two, Vector2 three) { return Area((Vector3)one, (Vector3)two, (Vector3)three); }
    float Area(Vector3 one, Vector3 two, Vector3 three) {
        float s = Mathf.Abs(one.x * two.y + two.x * three.y + three.x * one.y - one.y * two.x - two.y * three.x - three.y * one.x) / 2f;

        return s;
    }

}
