using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
    private BattleEntity mEntity;
    private BattleEntity mEntity1;
    void Start ()
    {

        CameraManager.Instance.Init();

        mEntity = ObjectPool.GetInstance<BattleEntity>();
        mEntity.id = 1;
        mEntity.position = Vector3.zero;
        mEntity.configid = 10009;

        BattleManager.Instance.AddEntity(mEntity);
        mEntity.active = true;

        mEntity1 = ObjectPool.GetInstance<BattleEntity>();
        mEntity1.id = 2;
        mEntity1.position = new Vector3(10,0,0);
        mEntity1.configid = 10009;

        BattleManager.Instance.AddEntity(mEntity1);
        mEntity1.active = true;

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
