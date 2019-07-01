//////////////////////////////////////////////////////////////////////////
// 
// 文件：Scripts\Battle\BattleManager.cs
// 作者：Lee
// 时间：2019/01/21
// 描述：战斗单位管理器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager
{
    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new BattleManager();

            return _instance;
        }
    }

    private Dictionary<string, EntityParamModel> mParams = new Dictionary<string, EntityParamModel>();

    /// <summary>
    /// <nodeid,list>
    /// </summary>
    public Dictionary<uint, BattleEntity> entities { get; private set; }

    private List<uint> mRemoveList = new List<uint>();
    public List<EffectEntity> mEffectList ;
#if UNITY_EDITOR
    public bool GM_BATTLE = false;
#endif

    

    public BattleManager()
    {

        entities = new Dictionary<uint, BattleEntity>();

        mEffectList = new List<EffectEntity>();

    }
   

    public bool AddEntity(BattleEntity entity)
    {
        if (entity == null)
        {
            return false ;
        }

        if (GetEntity(entity.id) != null)
        {
            return false;
        }

        entities.Add(entity.id, entity);
        entity.Init();

        return true;
    }

    public void GetParam(string congfig,Action<EntityParamModel> callback)
    {
        if(mParams.ContainsKey(congfig))
        {
            if(callback!= null)
            {
                callback(mParams[congfig]);
            }
             
        }
        else
        {
            AssetManager.Instance.LoadAsset<TextAsset>(congfig, (asset) => {

                if (asset!= null && mParams.ContainsKey(congfig)==false)
                { 
                    var xml = asset.assetObject;
                    if (xml)
                    {
                        var param = EntityParam.Create(xml.text) as EntityParamModel;
                        if (param != null)
                        {
                            mParams.Add(congfig, param);
                        }
                    }
                    asset.Destroy();
                }
                if (callback != null)
                {
                    callback(mParams.ContainsKey(congfig) ? mParams[congfig] : null);
                }

            });

            
        }
    }

    public void Show()
    {
        var it = entities.GetEnumerator();
        while(it.MoveNext())
        {
            var entity = it.Current.Value;
            entity.active = true;
           
        }
    }

    

    public void Hide()
    {
        var it = entities.GetEnumerator();

        while (it.MoveNext())
        {
            var entity = it.Current.Value;
            entity.active = false; 
        }    
    }



    public BattleEntity GetEntity( uint id)
    {
        BattleEntity entity = null;
        entities.TryGetValue(id, out entity);

        return entity;
    }

    public void RemoveEntity(uint id)
    {
        mRemoveList.Add(id);
       
    }

    public void Update(float deltaTime)
    {
        var it = entities.GetEnumerator();
        while (it.MoveNext())
        {
            var entity = it.Current.Value;

            if (entity != null)
            {
                entity.OnUpdate(deltaTime);
            }
        }

        for (int i = 0; i < mRemoveList.Count; i++)
        {
            uint id = mRemoveList[i];
            if (entities.ContainsKey(id))
            {
                var entity = entities[id];


                ObjectPool.ReturnInstance(entity);

                entities.Remove(id);
            }
        }
        mRemoveList.Clear();

        for (int i = mEffectList.Count - 1; i >= 0; --i)
        {
            if (mEffectList[i] == null)
            {
                mEffectList.RemoveAt(i);
            }
            else
            {
                mEffectList[i].OnUpdate(deltaTime);
            }
        }
    }

    public EffectEntity CreateEffect(EffectType type)
    {
        EffectEntity entity = null;
        switch (type)
        {
            case EffectType.Time:
                entity = ObjectPool.GetInstance<EffectEntityTime>(); break;
            case EffectType.Move:
                entity = ObjectPool.GetInstance<EffectEntityMove>(); break;
            case EffectType.Follow:
                entity = ObjectPool.GetInstance<EffectEntityFollow>(); break;
            case EffectType.Parabola:
                entity = ObjectPool.GetInstance<EffectParabolaEntity>(); break;
           

        }
        if (entity != null)
        {
            mEffectList.Add(entity);
        }
        return entity;
    }
    public void RemoveEffect(EffectEntity entity)
    {
        if (entity == null)
        {
            return;
        }
        for (int i = mEffectList.Count - 1; i >= 0; --i)
        {
            var effect = mEffectList[i];
            if (effect == null || effect == entity)
            {
                mEffectList.RemoveAt(i);
            }
        }
    }

    public void Destroy()
    {
        var it = entities.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.Destroy();
        }
        entities.Clear();
        ObjectPool.Clear<BattleEntity>();
    }


 
}

