using TMPro;
using UnityEngine;

public class CurrentTaskEntry : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI UsernameText { get; private set;}
    [field: SerializeField]
    public TextMeshProUGUI DescriptionText { get; private set;}
    [field: SerializeField]
    public TaskEntry Task { get; private set; }

    public void SetTask(TaskEntry task)
    {
        Task = task;
        UsernameText.text = task.User;
        DescriptionText.text = task.Description;
        
    }
}
