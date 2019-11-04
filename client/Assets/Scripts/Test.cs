using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
    public BattleEntity mEntity;
    public BattleEntity mEntity1;

    public static Test Instance { get; private set; }
    void Start ()
    {
        //AssetManager.Instance.LoadScene("testscene.unity", UnityEngine.SceneManagement.LoadSceneMode.Single, (scene) => {


        //});

        //return;
        SQLite.Instance.Open("D:/WorkSpace/moon/client/Assets/R/database/data.bytes");

        //string sql = "select * from TB_Hero";


        //SQLiteDataTable table = SQLite.Instance.GetDataTable(sql);
        //if (table != null)
        //{

        //    while (table.Read())
        //    {
        //        //READ_START

        //       int id = table.GetByColumnName("id", 0);


        //        Debug.Log(id);
        //        //READ_END
        //    }
        //    table.Close();
        //}
        //else
        //{
        //   // Debug.LogError("Can't find table:" + data.name);
        //}
        string drop = @"DROP TABLE IF EXISTS 'TB_Role';";
        SQLite.Instance.Excute(drop);
        string create = @"
CREATE TABLE TB_Role (
    id             INT           PRIMARY KEY
                                 NOT NULL
                                 DEFAULT (0),
    name           VARCHAR (256) NOT NULL,
    type           INT           NOT NULL,
    config         VARCHAR (256) NOT NULL,
    hp             INT           NOT NULL,
    attack         INT           NOT NULL,
    defense        INT           NOT NULL,
    movedistance   INT           NOT NULL,
    movedirection  INT           NOT NULL,
    attackdistance INT           NOT NULL,
    searchdistance INT           NOT NULL,
    radius         DECIMAL       NOT NULL,
    height         DECIMAL       NOT NULL,
    UNIQUE (
        id
    )
); ";

         SQLite.Instance.Excute(create);
       

        string insert = @"INSERT INTO TB_Role (id, name, type, config, hp, attack, defense, movedistance, movedirection, attackdistance, searchdistance, radius, height) VALUES ({0}, '炮手', 1, 'paoshou.txt', 900, 90, 10, 2, 2, 5, 10, 1, 2);";

        for (int i = 0; i < 10; ++i)
        {
            
            SQLite.Instance.Excute(string.Format(insert, i));
            
        }

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
