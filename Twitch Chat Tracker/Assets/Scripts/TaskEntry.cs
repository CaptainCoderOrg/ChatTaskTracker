using System.Linq;
using UnityEngine;

[System.Serializable]
public class TaskEntry
{
    public const string Seperator = " -- ";
    public const string TaskSetCommand = "!task set";
    public string DateTime;
    public string User;
    public string TaskType;
    public string Description;

    private TaskEntry(string[] split)
    {
        DateTime = split[0];
        TaskType = split[1];
        User = split[2];
        Description = string.Join(Seperator, split.Skip(3)).Substring(TaskSetCommand.Length).Trim();
    }

    public static bool TryFromLog(string input, out TaskEntry entry)
    {
        entry = null;
        string[] elements = input.Trim().Split(Seperator);
        if (elements.Length < 4) 
        { 
            Debug.LogError($"Invalid log entry: '{input.Trim()}'");
            return false; 
        }
        entry = new TaskEntry(elements);
        return true;
    }
}