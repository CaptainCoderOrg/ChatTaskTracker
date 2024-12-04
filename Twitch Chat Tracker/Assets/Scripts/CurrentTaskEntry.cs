using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CurrentTaskEntry : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI UsernameText { get; private set;}
    [field: SerializeField]
    public TextMeshProUGUI DescriptionText { get; private set;}
    [field: SerializeField]
    public TextMeshProUGUI TimeText { get; private set; }

    [field: SerializeField]
    public TaskEntry Task { get; private set; }


    public void UpdateTime()
    {
        RenderTaskTime(Task);
    }

    public void SetTask(TaskEntry task)
    {
        Task = task;
        if (task.IsComplete())
        {
            gameObject.SetActive(false);
        }
        else if (task.IsSetTask())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        UsernameText.text = task.User;
        DescriptionText.text = task.Description;
        RenderTaskTime(task);
    }

    public void RenderTaskTime(TaskEntry task)
    {
        TimeSpan time = DateTime.Now - task.StartTime();
        string hours = time.Hours > 0 ? $"{time.Hours}:" : string.Empty;
        TimeText.text = $"{hours}{time.Minutes:00}:{time.Seconds:00}";
    }
}
