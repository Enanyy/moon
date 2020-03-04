using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RemindListener : MonoBehaviour
{
    [SerializeField]
    private List<RemindID> IDs = new List<RemindID>();
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private Text text;
    private void Start()
    {
        for (int i = 0, max = IDs.Count; i < max; ++i)
        {
            RemindSystem.Instance.RegisterListener(OnCountChanged, (int)IDs[i]);
        }
    }
    private void OnDestroy()
    {
        RemindSystem.Instance.UnRegisterListener(OnCountChanged);
    }

    protected virtual void OnCountChanged(int count)
    {
        if (target != null)
        {
            target.SetActive(count > 0);
        }
        if (text != null)
        {
            text.text = count.ToString();
        }
    }
}

