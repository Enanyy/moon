using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
    public BattleEntity mEntity;
    public BattleEntity mEntity1;

    public static Test Instance { get; private set; }
    void Start ()
    {
        //var task =  AssetManager.Instance.LoadScene("testscene.unity", UnityEngine.SceneManagement.LoadSceneMode.Additive, (scene,mode) => {


        //});
        //task.Cancel();

        //return;

        AssetManager.Instance.LoadAsset<Material>("diban.mat", (asset) => { 
            
            if(asset!= null)
            {
                Debug.Log("Load Success!");
            }
            else
            {
                Debug.Log("Load Failed!");

            }

        });


        Instance = this;

        CameraManager.Instance.Init();

        mEntity = ObjectPool.GetInstance<BattleEntity>();
        mEntity.id = 1;
        mEntity.position = Vector3.zero;
        mEntity.config = "yingxiong_kulouqishi.txt";

        BattleManager.Instance.AddEntity(mEntity);
        mEntity.active = true;

        mEntity1 = ObjectPool.GetInstance<BattleEntity>();
        mEntity1.id = 2;
        mEntity1.position = new Vector3(10,0,0);
        mEntity1.config = "nvjiangjun.txt";

        BattleManager.Instance.AddEntity(mEntity1);
        mEntity1.active = true;

        for(int i = 0; i < 10; i++)
        {
            Debug.LogError("Error!!!!");
        }

    }

    

    // Update is called once per frame
    void Update () {
		BattleManager.Instance.Update(Time.deltaTime);

        if (mEntity != null && mEntity1 != null)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var attack = ObjectPool.GetInstance<EntityAction>();
                attack.target = mEntity1.id;
                

                mEntity.PlayAction(ActionType.Attack,attack);
            }

            if (Input.GetMouseButtonDown(0))
            {
                var run = ObjectPool.GetInstance<EntityAction>();
                run.AddPathPoint(CameraManager.Instance.GetWorldMousePosition(), Vector3.zero, true);
                mEntity.PlayAction(ActionType.Run,run);
            }
        }
	}
}
