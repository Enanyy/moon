using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestAsset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AssetLoader.LoadAsset<GameObject>("nvjiangjun.prefab", (asset) => { 
        
        
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AssetLoader.LoadScene("testscene.unity", LoadSceneMode.Single, (scene, mode) =>
            {


            });
        }
    }
}
