using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class TaskEntry
{
    public const string CompleteTaskTag = "CompleteTask";
    public const string SetTaskTag = "SetTask";
    public const string Seperator = " -- ";
    public const string TaskSetCommand = "!task set";
    public const string TaskCompleteCommand = "!task done";
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

    internal bool IsComplete() => TaskType == CompleteTaskTag;
    internal bool IsSetTask() => TaskType == SetTaskTag;
}

public static class TaskEntryExtensions
{
    public const string DaySuffixRegex = "st|nd|rd|th";
    public static DateTime StartTime(this TaskEntry entry)
    {
        if(!long.TryParse(entry.DateTime, out long unixTimeSeconds))
        {
            Debug.LogError($"Invalid date format: '{entry.DateTime}'");
            return default;
        }
        
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds).LocalDateTime;
    }

}