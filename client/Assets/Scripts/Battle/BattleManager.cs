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

    private Dictionary<uint, ModelParam> mParams = new Dictionary<uint, ModelParam>();

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

        if (mParams.ContainsKey(entity.configid) == false)
        {
            string path = AssetPath.Get(entity.configid);

            var xml = Resources.Load<TextAsset>(path);
            if (xml)
            {
                var param = BattleParam.Create(xml.text) as ModelParam;
                if (param != null)
                {
                    mParams.Add(entity.configid, param);
                }
            }
        }
        if (mParams.ContainsKey(entity.configid) == false)
        {
            return false;
        }

        if(entity.Init(mParams[entity.configid]) == false)
        {
            return false;
        }

        entities.Add(entity.id, entity);


        return true;
    }

    public ModelParam GetParam(uint configid)
    {
        if(mParams.ContainsKey(configid))
        {
            return mParams[configid];
        }
        return null;
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

    public BattleEntity FindClosestTarget(BattleEntity entity)
    {
        if(entity == null)
        {
            return null;
        }
        BattleEntity target = null;

        float searchDistance = entity.GetProperty(EntityProperty.PRO_SEARCH_DISTANCE) * 0.01f;

        float minDistance = 0;
        var it = entities.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Value.id != entity.id && it.Current.Value.campflag != entity.campflag)
            {
                float distance = Vector3.Distance(it.Current.Value.position, entity.position);
                if (distance < searchDistance)
                {
                    if (target == null || distance < minDistance)
                    {
                        target = it.Current.Value;
                        minDistance = distance;
                    }
                }
            }
        }

        return target;

    }

    public BattleEntity GetBlockEntity(BattleEntity entity)
    {
        if(entity == null)
        {
            return null;
        }

        BattleEntity target = null;

        var it = entities.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current.Value.id != entity.id )
            {
                if (entity.rectangle.Intersect(it.Current.Value.rectangle))
                {
                    return it.Current.Value;
                }
            }
        }

        return target;
    }


    public Vector3 MoveToWards(BattleEntity entity, BattleEntity block)
    {
        Vector3 direction = block.position - entity.position;
        Vector3 vertical = Vector3.Cross(direction.normalized, Vector3.up);
        vertical.y = 0;

        float radius = entity.GetProperty(EntityProperty.PRO_RADIUS) * 0.01f + block.GetProperty(EntityProperty.PRO_RADIUS) * 0.01f;

        Vector3 point = block.position + vertical * radius;
        bool isValid = block.rectangle.Contains(point);

        if (isValid)
        {
            isValid = !EntityContains(point);
        }

        if(isValid==false)
        {
            point = block.position - vertical * radius;
            isValid = block.rectangle.Contains(point);

            if (isValid)
            {
                isValid = !EntityContains(point);
            }
        }

        if (isValid == false)
        {
            point = entity.position + vertical * radius;
            isValid = !EntityContains(point);
           
        }
        if (isValid == false)
        {
            point = entity.position - vertical * radius;

            isValid = !EntityContains(point);
        }

        if (isValid)
        {
            return (point - entity.position).normalized;
        }

        return Vector3.zero;
    }

    public bool EntityContains(Vector3 point)
    {
        bool contains = false;
        var it = entities.GetEnumerator();
        while (it.MoveNext())
        {
            if (Vector3.Distance(it.Current.Value.position, point) < it.Current.Value.GetProperty(EntityProperty.PRO_RADIUS)*0.01f)
            {
                contains = true;
                break;
            }
        }

        return contains;
    }

    public void FixedUpdate(float deltaTime)
    {
        
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
                entity = ObjectPool.GetInstance<EffectTimeEntity>(); break;
            case EffectType.Move:
                entity = ObjectPool.GetInstance<EffectMoveEntity>(); break;
            case EffectType.Follow:
                entity = ObjectPool.GetInstance<EffectFollowEntity>(); break;
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

