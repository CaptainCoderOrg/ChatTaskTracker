using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class TaskPanel : MonoBehaviour
{
    [field: SerializeField]
    public CurrentTaskEntry EntryPrefab { get; private set; }
    [field: SerializeField]
    public TaskDatabase Tasks { get; private set; }
    [field: SerializeField]
    public Transform Parent { get; private set; }
    private Dictionary<string, CurrentTaskEntry> _userLookup = new();
    void Start()
    {
        foreach (TaskEntry entry in Tasks.Entries)
        {
            RenderTask(entry);
        }
        Tasks.OnTaskAdded.AddListener(RenderTask);
        StartCoroutine(Tick());
    }

    public IEnumerator Tick()
    {
        YieldInstruction second = new WaitForSeconds(1);
        while (true)
        {
            foreach (var task in _userLookup.Values)
            {
                if (task.isActiveAndEnabled)
                {
                    task.UpdateTime();
                }
            }
            yield return second;
        }
    }

    [Button("ForceUpdate")]
    public void ForceUpdate()
    {
        Canvas.ForceUpdateCanvases();
    }

    public void RenderTask(TaskEntry entry)
    {
        if (entry == null) { return; }
        if (!_userLookup.TryGetValue(entry.User, out CurrentTaskEntry cte))
        {
            cte = Instantiate(EntryPrefab, Parent);
            cte.name = $"{entry.User} (CurrentTaskEntry)";
            _userLookup.Add(entry.User, cte);
        }
        cte.SetTask(entry);
    }
}
