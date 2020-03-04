using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemindTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            RemindSystem.Instance.ChangeStatus((int)RemindID.Test1, RemindStatus.On);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RemindSystem.Instance.ChangeCount((int)RemindID.Test2, 4);
        }
    }
}
