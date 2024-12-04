using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class TaskDatabase : ScriptableObject
{
    [field: SerializeField]
    public string FilePath { get; private set; }
    [field: SerializeField]
    public List<TaskEntry> Entries { get; private set; }
    [field: SerializeField]
    public UnityEvent<TaskEntry> OnTaskAdded { get; private set; }

    public void AddEntry(TaskEntry task)
    {
        Entries.Add(task);
        OnTaskAdded.Invoke(task);
    }

    [Button("Clear")]
    public void Clear()
    {
        Entries.Clear();
    }

    [Button("Refresh")]
    public void Refresh()
    {
        // To do, we can probably read a file but not close it so we don't have to
        // read all of the lines each time.
        string[] lines = File.ReadAllLines(FilePath);

        // Skip past entries we've already loaded
        for (int i = Entries.Count; i < lines.Length; i++)
        {
            if (TaskEntry.TryFromLog(lines[i], out TaskEntry entry))
            {
                AddEntry(entry);
            }
            else
            {
                AddEntry(null);
            }

        }
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            OnTaskAdded.RemoveAllListeners();
        }
    }
#endif

}

[System.Serializable]
public class UserEntry
{
    public string UserName;
    public List<TaskEntry> TaskHistory;
}