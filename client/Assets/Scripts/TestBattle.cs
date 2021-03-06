﻿using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class TestBattle : MonoBehaviour {

	// Use this for initialization
    public BattleEntity mEntity;
    public BattleEntity mEntity1;

    public static TestBattle Instance { get; private set; }

    private void Awake()
    {
        ScreenLogger.SetActive(true);
    }

    private Scene mScene;

    void Start ()
    {
        var task = AssetLoader.LoadScene("testscene.unity", LoadSceneMode.Additive, (scene, mode) =>
        {
            mScene = scene;

        });
        task.isCancel =true;

        //return;

        AssetLoader.LoadAsset<TextAsset>("data.bytes", (asset) =>
        {
            if (asset != null)
            {
                byte[] bytes = asset.assetObject.bytes;

                //启动线程加载
                ThreadQueue.RunAsync(() => DataTableManager.Instance.Init(bytes),
                    () => {
                        
                        asset.Destroy();
                    });
            }
        });

        AssetLoader.LoadAsset<GameObject>("cube.prefab", (asset) => { 
            
            if(asset!= null)
            {
                Debug.Log("Load Success:cube.prefab!");
                asset.Destroy();
            }
            else
            {
                Debug.Log("Load Failed:cube.prefab!");

            }

        });

       

        AssetLoader.LoadAsset<GameObject>("cube1.prefab", (asset) =>
        {

            if (asset != null)
            {
                Debug.Log("Load Success:cube1.prefab!");
            }
            else
            {
                Debug.Log("Load Failed:cube1.prefab!");

            }

        });
        //task.isCancel = true;

        Instance = this;

        CameraManager.Instance.Init();

        mEntity = ObjectPool.GetInstance<BattleEntity>();
        mEntity.id = 1;
        mEntity.position = Vector3.zero;
        mEntity.config = "yingxiong_kulouqishi.txt";
        mEntity.properties.SetProperty<float>((uint)PropertyID.PRO_MOVE_SPEED, 6);

        BattleManager.Instance.AddEntity(mEntity);
        mEntity.active = true;

        mEntity1 = ObjectPool.GetInstance<BattleEntity>();
        mEntity1.id = 2;
        mEntity1.position = new Vector3(10, 0, 0);
        mEntity1.config = "nvjiangjun.txt";

        BattleManager.Instance.AddEntity(mEntity1);
        mEntity1.active = true;

        EventSystem.Instance.AddListener(1, 1,0,OnListen);
        EventSystem.Instance.Invoke(1,1,0);

        EventListener listener = new EventListener();
        listener.AddListener((Action)OnListen);
        listener.Invoke();
        listener.AddListener((Action<int>)OnListen);
        listener.Invoke(1);
    }

    void OnListen()
    {
        Debug.Log("EventNotify test");
    }    
    void OnListen(int a)
    {
        Debug.Log(a);
    }

    // Update is called once per frame
    void Update () {
		BattleManager.Instance.Update(Time.deltaTime);

        if (mEntity != null && mEntity1 != null)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                mEntity.Attack(mEntity1.id);
            }

            if (Input.GetMouseButtonDown(0))
            {
                mEntity.MoveTo(CameraManager.Instance.GetWorldMousePosition(),Vector3.zero,true);
              
            }
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            AssetLoader.UnloadScene(mScene, () => {
                Debug.Log("Unload scene success!");
            });
        }
	}
}
