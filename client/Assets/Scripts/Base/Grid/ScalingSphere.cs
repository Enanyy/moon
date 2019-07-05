using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScalingSphere : MonoBehaviour {

    
   
    public float SphereRadius = 1;
    public int SphereDetail = 0;
    public bool Activate = false;
    public bool SaveSphere = false;

    public Material material;

   
	// Use this for initialization
	void Awake () {
       
    }

    void Update()
    {
        // avoiding the use of a UI class.
        if (Activate)
        {
            StartCoroutine(ThreadGenerateGrid());
            Activate = false;
        }

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
	
  

    IEnumerator ThreadGenerateGrid()
    {
        float time = Time.time;
        int task = ThreadRunner.CreateThread(new System.Threading.ParameterizedThreadStart(ThreadedSphere), (object)transform.position);
        ThreadRunner.StartThread(task);

        while (!ThreadRunner.isComplete(task))
        {
            yield return null;
        }

        SphereGrid grid = (SphereGrid)ThreadRunner.FetchData(task);

        Debug.Log(Time.time - time);
        grid.Show(transform,material);
    }

   


    void ThreadedSphere(object d)
    {
        SphereGrid grid = new SphereGrid();
        grid.original = (Vector3) d;

        grid.GenerateTriangle(SphereRadius,SphereDetail);
        grid.CalculateNeihbors();
        ThreadRunner.ExportData(grid);
        ThreadRunner.MarkComplete();

    }

    float Area(Vector2 one, Vector2 two, Vector2 three) { return Area((Vector3)one, (Vector3)two, (Vector3)three); }
    float Area(Vector3 one, Vector3 two, Vector3 three) {
        float s = Mathf.Abs(one.x * two.y + two.x * three.y + three.x * one.y - one.y * two.x - two.y * three.x - three.y * one.x) / 2f;

        return s;
    }

}
