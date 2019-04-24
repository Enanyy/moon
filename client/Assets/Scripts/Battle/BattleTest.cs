#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CampFlag
{
    Attack,
    Defense,
}
[System.Serializable]
public class BattleTestData
{
    public int type;
    public uint id;
    public bool AI;
    public int count=1;
    public CampFlag camp;
}


public class BattleTest : MonoBehaviour
{
    Plane mPlane = new Plane(Vector3.up, Vector3.zero);
    public static BattleEntity entity;
    public int width = 100;
    public List<BattleTestData> CreateList = new List<BattleTestData>();

    public static bool TEST = false;

    public RadiusType radiusType;
    
    private List<BattleTestRenderer> battleTestRadius = new List<BattleTestRenderer>();
    void Awake()
    {
        
    }
    // Use this for initialization
    void Start()
    {  
        TEST = true;
        for (int i = 0; i < CreateList.Count; ++i)
        {
            for (int j = 0; j < CreateList[i].count; ++j)
            {
                BattleEntity data = ObjectPool.GetInstance<BattleEntity>();

                data.id = (uint)(i * 1000 + j);

                data.configid = CreateList[i].id;
                data.campflag = (uint)CreateList[i].camp;
                if (CreateList[i].camp == CampFlag.Attack)
                { 
                    data.position = new Vector3(10+2 * (i + 1), 0, 10 + j * (width - 20)/ CreateList[i].count);
                }
                else
                {
                    data.position = new Vector3(width-(10 + 2 * (i + 1)), 0, 10 + j * (width - 20) / CreateList[i].count);
                }
                data.type = (uint)CreateList[i].type;

                data.hp = uint.MaxValue;

                if (BattleManager.Instance.AddEntity(data))
                {
                    if (CreateList[i].AI)
                    {
                        data.AddComponent<AIComponent>();
                    }
                    data.active = true;
                }
            }
        }
       
        

    }

    void CreateRadius()
    {
        var it = BattleManager.Instance.entities.GetEnumerator();
        while (it.MoveNext())
        {
            var entity = it.Current.Value;
            var model = entity.GetComponent<ModelComponent>();
            if (model!= null && model.gameObject != null)
            {
                if (model.gameObject.transform.Find("lines") == null)
                {
                    GameObject line = new GameObject("lines");
                    line.transform.SetParent(model.gameObject.transform);
                    line.transform.localPosition = new Vector3(0, 0.2f, 0);
                    line.transform.localScale = Vector3.one;

                    ShowRadius(line.transform,  RadiusType.AttackDistance, entity, entity.param.attackDistance, Color.red);
                    ShowRadius(line.transform, RadiusType.SearchDistance, entity, entity.param.searchDistance, Color.yellow);
                    ShowRadius(line.transform,  RadiusType.ModuleRadius, entity, entity.param.radius, Color.green);
                    ShowRadius(line.transform,  RadiusType. Rectangle, entity, entity.param.radius, Color.blue);
                }
            }
        }
    }
    void ShowRadius(Transform parent, RadiusType type, BattleEntity entity, float distance, Color color)
    {
        GameObject go = new GameObject(type.ToString());
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;
        if (type == RadiusType.Rectangle)
        {
            var r = go.AddComponent<BattleTestRectangle>();
            r.type = type;
            r.id = entity.configid;
            r.entity = entity;
            r.color = color;
            r.rendering = radiusType == RadiusType.All || radiusType == type;

            battleTestRadius.Add(r);
        }
        else
        {
            var r = go.AddComponent<BattleTestRadius>();
            r.type = type;
            r.id = entity.configid;
            r.radius = distance;
            r.color = color;
            r.rendering = radiusType == RadiusType.All || radiusType == type;

            battleTestRadius.Add(r);
        }
    }
    BattleTestData GetTestData(uint id)
    {
        for(int i = 0; i < CreateList.Count;++i)
        {
            if(CreateList[i].id == id)
            {
                return CreateList[i];
            }
        }
        return null;
    }

    private void FixedUpdate()
    {
        BattleManager.Instance.FixedUpdate(Time.fixedDeltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        BattleManager.Instance.Update(Time.deltaTime);

        if (entity == null || entity.isPool)
        {
            SwitchEntity();
        }
 
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchEntity();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
           
            entity.Die();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
           
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            entity.isPause = !entity.isPause;

        }
        //if(Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    Plane plane = new Plane(Vector3.up, Vector3.zero);
        //    float distance = 0;
        //    plane.Raycast(ray, out distance);
        //    Vector3 destination = ray.GetPoint(distance);

        //    EntityAction action = entity.GetFirst(ActionType.Run);
        //    if (action != null)
        //    {
        //        action.paths.AddLast(destination);
        //    }
        //    else
        //    {
        //        action = ObjectPool.GetInstance<EntityAction>();
        //        action.paths.AddLast(destination);

        //        entity.PlayAction(ActionType.Run, action);
        //    }

           
        //}

        Move();
        Drag();

        // 鼠标滚轮触发缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < -0.001 || scroll > 0.001)
        {
            float displacement = scrollSpeed * scroll;

            Camera.main.transform.position += Camera.main.transform.forward * displacement;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            float distance = 0;
            mPlane.Raycast(ray, out distance);

            if (distance < minScrollDistance)
            {
                Camera.main.transform.position = ray.GetPoint(distance - minScrollDistance);
            }
            else if (distance > maxScrollDistance)
            {
                Camera.main.transform.position = ray.GetPoint(distance - maxScrollDistance);
            }
        }
        
        UpdateRadius();
        CreateRadius();
    }
    //缩放距离限制   

    private float minScrollDistance = 1;
    private float maxScrollDistance = 100;
    private float scrollSpeed = 20;

  
    void UpdateRadius()
    {

        for (int i = 0; i < battleTestRadius.Count; ++i)
        {
            if (battleTestRadius[i] != null)
            {
                var data = GetEntityByConfig(battleTestRadius[i].id);
                if (data != null)
                {
                    switch (battleTestRadius[i].type)
                    {
                        case RadiusType.AttackDistance:
                            {
                                var radius = battleTestRadius[i] as BattleTestRadius;
                                radius.radius = data.param.attackDistance;
                            }
                            break;
                        case RadiusType.SearchDistance:
                            {
                                var radius = battleTestRadius[i] as BattleTestRadius;
                                radius.radius = data.param.searchDistance;
                            } break;
                        case RadiusType.ModuleRadius:
                            {
                                var radius = battleTestRadius[i] as BattleTestRadius;
                                radius.radius = data.param.radius;
                            } break;
                    }
                }

                battleTestRadius[i].rendering = radiusType == RadiusType.All || battleTestRadius[i].type == radiusType;
            }
        }
    }

    BattleEntity GetEntityByConfig(uint configid)
    {
        var it = BattleManager.Instance.entities.GetEnumerator();
        while(it.MoveNext())
        {
            if (it.Current.Value.configid == configid)
            {
                return it.Current.Value;
            }
        }
        return null;
    }

    uint mCurrentIndex = 0;
    void SwitchEntity()
    {      
        mCurrentIndex += 1;
        if (mCurrentIndex > BattleManager.Instance.entities.Count)
        {
            mCurrentIndex = 1;
        }
        int index = 1;

        var iter = BattleManager.Instance.entities.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.Value != null)
            {
                if (index == mCurrentIndex)
                {
                    entity = iter.Current.Value;                
                    break;
                }
                index++;
            }
        }
    }
    void Attack()
    {
        if (entity == null)
        {
            return;
        }

        EntityAction action = ObjectPool.GetInstance<EntityAction>();
        action.skillid = 1;
        action.duration = 0.4f;

        var iter = BattleManager.Instance.entities.GetEnumerator();
        while (iter.MoveNext())
        {
            var e = iter.Current.Value;
            if (e != null
                && e != entity && Vector3.Distance(e.position, entity.position) < entity.param.attackDistance)
            {
                action.target = e.id;
                break;
            }
        }
        if (action.target > 0)
        {
            entity.PlayAction(ActionType.Attack, action);
        }
    }
    void Move()
    {
        if(entity == null)
        {
            return;
        }

        if(entity.GetComponent<AIComponent>()!= null)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.001f || Mathf.Abs(vertical) > 0.001f)
        {

            Vector3 velocity = new Vector3(horizontal, 0, vertical).normalized;
            Quaternion r = Quaternion.LookRotation(velocity);
            Quaternion q = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y + Camera.main.transform.eulerAngles.y, r.eulerAngles.z);

            velocity = (q * Vector3.forward) * entity.param.movespeed;

            EntityAction action = entity.GetFirst(ActionType.Run);

            if (action != null)
            {
                action.velocity = velocity;
            }
            else
            { 
                action = ObjectPool.GetInstance<EntityAction>();
                action.velocity = velocity;
                entity.PlayAction(ActionType.Run, action);
            }
        }
        else
        {
            if (entity.machine != null && entity.machine.current != null && entity.machine.current.type == (int)ActionType.Run)
            {
                var action = entity.machine.current as EntityAction;
                if (action != null && action.paths.Count == 0)
                {
                    action.Done();
                }
            }
        }
    }

    Vector3 oldMousePosition;
    void Drag()
    {
        if (Input.GetMouseButton(1))
        {
            float xDelta = Input.GetAxis("Mouse X");
            float yDelta = Input.GetAxis("Mouse Y");
            if (xDelta != 0.0f || yDelta != 0.0f)
            {
                if (oldMousePosition != Vector3.zero)
                {
                    Ray rayDest = Camera.main.ScreenPointToRay(Input.mousePosition);

                    float distance = 0;
                    mPlane.Raycast(rayDest, out distance);

                    Vector3 dest = rayDest.GetPoint(distance);
                    distance = 0;
                    Ray rayOld = Camera.main.ScreenPointToRay(oldMousePosition);
                    mPlane.Raycast(rayOld, out distance);


                    Vector3 pos = Camera.main.transform.localPosition + rayOld.GetPoint(distance) - dest;

                    Camera.main.transform.localPosition = pos;
                }

                oldMousePosition = Input.mousePosition;
            }

        }
        if (Input.GetMouseButtonUp(1))
        {
            oldMousePosition = Vector3.zero;
        }
       
    }
    void CreateEntity(CampFlag camp)
    {
        for (int i = 0; i < CreateList.Count; ++i)
        {
            if (CreateList[i].camp == camp)
            {
                for (int j = 0; j < CreateList[i].count; ++j)
                {
                    BattleEntity data = ObjectPool.GetInstance<BattleEntity>();

                    data.id = BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0);

                    data.configid = CreateList[i].id;
                    data.campflag = (uint)CreateList[i].camp;
                    if (CreateList[i].camp == CampFlag.Attack)
                    {
                        data.position = new Vector3(10 + 2 * (i + 1), 0, 10 + j * (width - 20) / CreateList[i].count);
                    }
                    else
                    {
                        data.position = new Vector3(width - (10 + 2 * (i + 1)), 0, 10 + j * (width - 20) / CreateList[i].count);
                    }
                    data.type = (uint)CreateList[i].type;

                    data.hp = uint.MaxValue;

                    if (BattleManager.Instance.AddEntity(data))
                    {
                        if (CreateList[i].AI)
                        {
                            data.AddComponent<AIComponent>();
                        }
                        data.active = true;
                    }
                }
            }
           
        }
    }
    void OnGUI()
    {
  
        GUI.skin.label.normal.textColor = Color.green;

        GUI.Label(new Rect(20, 0, 1000, 20), "Tab：切换控制对象，方向键移动， Q：攻击，Z：死亡,V:胜利，R:撤退，P：暂停，红色圈：攻击半径，黄色圈：索敌半径，绿色圈：模型半径");
       
        if(GUI.Button(new Rect(20,40,60,20),"Show"))
        {
            BattleManager.Instance.Show();
            CreateRadius();
        }
        if (GUI.Button(new Rect(20, 80, 60, 20), "Hide"))
        {
            BattleManager.Instance.Hide();
        }
        if (GUI.Button(new Rect(20, 120, 100, 20), "Create Attack"))
        {
            CreateEntity(CampFlag.Attack);
        }
        if (GUI.Button(new Rect(20, 160, 100, 20), "Create Defense"))
        {
            CreateEntity(CampFlag.Defense);
        }
    }
}

public enum RadiusType
{
    All,
    AttackDistance, // 攻击距离
    SearchDistance, //索敌范围
    ModuleRadius,      //模型半径
     Rectangle,
    None,
}

[RequireComponent(typeof(LineRenderer))]
public class BattleTestRenderer : MonoBehaviour
{
    public RadiusType type;

    protected LineRenderer mRenderer;
    public uint id;

    public float width = 0.1f;
    public bool rendering = true;
    public Color color = Color.red;
    protected virtual void Start()
    {
        mRenderer = GetComponent<LineRenderer>();
        mRenderer.material = Resources.Load<Material>("r/material/line");
    }

    protected virtual void Update()
    {
        if (rendering)
        {
            mRenderer.startColor = color;
            mRenderer.endColor = color;
            mRenderer.startWidth = width;
            mRenderer.endWidth = width;
        }
        else
        {
            mRenderer.positionCount = 0;
        }
    }
}

public class BattleTestRadius : BattleTestRenderer
{

    public int pointCount = 50;

    public float radius = 10f;


    private float angle;

    private List<Vector3> points = new List<Vector3>();
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        angle = 360f / pointCount;
    }

    void CalculationPoints()
    {
        Vector3 v = transform.position + transform.forward * radius;
        points.Add(v);
        Quaternion r = transform.rotation;
        for (int i = 1; i < pointCount; i++)
        {
            Quaternion q = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y - (angle * i), r.eulerAngles.z);
            v = transform.position + (q * Vector3.forward) * radius;
            points.Add(v);
        }
    }
    void DrowPoints()
    {
        for (int i = 0; i < points.Count; i++)
        {
            mRenderer.SetPosition(i, points[i]);  //把所有点添加到positions里
        }
        if (points.Count > 0)   //这里要说明一下，因为圆是闭合的曲线，最后的终点也就是起点，
            mRenderer.SetPosition(pointCount, points[0]);
    }
    void ClearPoints()
    {
        points.Clear();  ///清除所有点
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (rendering)
        {
            mRenderer.positionCount = pointCount + 1;  ///这里是设置圆的点数，加1是因为加了一个终点（起点）
            CalculationPoints();
            DrowPoints();
        }

        ClearPoints();
    }
}

public class BattleTestRectangle : BattleTestRenderer
{
    public BattleEntity entity;
    protected override void Update()
    {
        base.Update();
        if (rendering && entity != null)
        {
            float y = transform.position.y;
            Rectangle rectangle = entity.rectangle;

            Vector3 a = new Vector3(rectangle.a.x, y, rectangle.a.y);
            Vector3 b = new Vector3(rectangle.b.x, y, rectangle.b.y);
            Vector3 c = new Vector3(rectangle.c.x, y, rectangle.c.y);
            Vector3 d = new Vector3(rectangle.d.x, y, rectangle.d.y);
        
            mRenderer.positionCount = 5;
            mRenderer.SetPosition(0, a);
            mRenderer.SetPosition(1, b);
            mRenderer.SetPosition(2, c);
            mRenderer.SetPosition(3, d);
            mRenderer.SetPosition(4, a);
        }
    }
}


#endif