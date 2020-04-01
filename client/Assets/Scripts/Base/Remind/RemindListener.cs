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

    private Reminder reminder;
    private void Start()
    {
        reminder = new Reminder(OnCountChanged);

        for (int i = 0, max = IDs.Count; i < max; ++i)
        {
            reminder.AddRemindID((int)IDs[i]);
        }
        RemindSystem.Instance.RegisterListener(reminder);

    }
    private void OnDestroy()
    {
        RemindSystem.Instance.UnRegisterListener(reminder);
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

