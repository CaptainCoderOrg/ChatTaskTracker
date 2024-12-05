using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;

public class WorkTimerReader : MonoBehaviour
{
    public const string WorkCommand = "!work";
    public const string BreakCommand = "!break";
    public const string PomoCommand = "!pomo";
    [field: SerializeField]
    public AudioSource BreakTimeSound { get; private set; }
    [field: SerializeField]
    public bool PlayOnTimesUp = false;
    [field: SerializeField]
    public bool IsWorking = false;
    [field: SerializeField]
    public TextMeshProUGUI BreakText { get; private set; }
    [field: SerializeField]
    public float PollingSpeed { get; private set; } = 1;

    [field: SerializeField]
    public string PathToLog { get; private set; }
    [field: SerializeField]
    public int Minutes { get; private set; }
    [field: SerializeField]
    public int WorkMinutes { get; private set; }
    [field: SerializeField]
    public int BreakMinutes { get; private set; }
    [field: SerializeField]
    public long StartTime { get; private set; }
    [field: SerializeField]
    public DateTime EndTime { get; private set; }
    [field: SerializeField]
    public DateTime StartDateTime { get; private set; }
    [field: SerializeField]
    public string Command { get; private set; }

    [field: SerializeField]
    private string _rawTimer;
    public string RawTimer
    {
        get => _rawTimer;
        private set
        {
            if (_rawTimer == value) { return; }
            _rawTimer = value;
            string[] parts = value.Trim().Split(' ');
            Command = parts[0].Trim();
            if (Command is PomoCommand)
            {
                string[] pomoParts = parts[1].Split('/');
                WorkMinutes = Minutes = int.Parse(pomoParts[0]);
                BreakMinutes = int.Parse(pomoParts[1]);
            }
            else
            {
                Minutes = int.Parse(parts[1]);
            }
            
            StartTime = long.Parse(parts[2]);
            StartDateTime = DateTimeOffset.FromUnixTimeSeconds(StartTime).LocalDateTime;
            Debug.Log(StartDateTime);
            EndTime = StartDateTime + new TimeSpan(0, Minutes, 0);
        }
    }

    void Start()
    {
        StartCoroutine(PollLog());
        StartCoroutine(Tick());
    }

    public IEnumerator Tick()
    {
        YieldInstruction delay = new WaitForSeconds(1);
        while (true)
        {
            if (Command is WorkCommand)
            {
                HandleWorkCommand();
            }
            else if (Command is BreakCommand)
            {
                HandleBreakCommand();
            }
            else if (Command is PomoCommand)
            {
                HandlePomoCommand();
            }

            yield return delay;
        }
    }

    private void HandlePomoCommand()
    {
        TimeSpan TimePassed = DateTime.Now - StartDateTime;
        int secondsPassed = (int)TimePassed.TotalSeconds;
        int secondsInWork = WorkMinutes * 60;
        int secondsInBreak = BreakMinutes * 60;
        int secondsInSession = secondsInWork + secondsInBreak;
        int currentSession = secondsPassed % secondsInSession;
        bool InWork = currentSession <= secondsInWork;
        bool InBreak = !InWork;
        if (InWork)
        {
            if (!IsWorking)
            {
                IsWorking = true;
                BreakTimeSound.Play();
            }
            int secondsLeftToWork = (secondsInSession - secondsInBreak) - currentSession;
            int minutes = secondsLeftToWork / 60;
            int seconds = secondsLeftToWork % 60;
            BreakText.text = $"Next Break: <color=green>{minutes}:{seconds:00}</color>";
        }
        else
        {
            if (IsWorking)
            {
                IsWorking = false;
                BreakTimeSound.Play();
            }
            int secondsLeftInBreak = secondsInSession - currentSession;
            int minutes = secondsLeftInBreak / 60;
            int seconds = secondsLeftInBreak % 60;
            BreakText.text = $"<color=yellow>On Break: {minutes}:{seconds:00}</color>";
        }
    }

    private void HandleBreakCommand()
    {
        TimeSpan timeRemaining = EndTime - DateTime.Now;
        if (timeRemaining > TimeSpan.Zero)
        {
            PlayOnTimesUp = true;
            BreakText.text = $"On Break: <color=green>{timeRemaining.Minutes}:{timeRemaining.Seconds:00}</color>";
        }
        else
        {
            if (PlayOnTimesUp)
            {
                PlayOnTimesUp = false;
                BreakTimeSound.Play();
            }
            BreakText.text = $"Break Over <color=red>+{Math.Abs(timeRemaining.Minutes)}:{Math.Abs(timeRemaining.Seconds):00}</color>";
        }
    }

    private void HandleWorkCommand()
    {
        TimeSpan timeRemaining = EndTime - DateTime.Now;
        if (timeRemaining > TimeSpan.Zero)
        {
            PlayOnTimesUp = true;
            BreakText.text = $"Next Break: <color=green>{timeRemaining.Minutes}:{timeRemaining.Seconds:00}</color>";
        }
        else
        {
            if (PlayOnTimesUp)
            {
                PlayOnTimesUp = false;
                BreakTimeSound.Play();
            }
            BreakText.text = $"Times Up <color=red>+{Math.Abs(timeRemaining.Minutes)}:{Math.Abs(timeRemaining.Seconds):00}</color>";
        }
    }

    public IEnumerator PollLog()
    {
        YieldInstruction delay = new WaitForSeconds(PollingSpeed);
        while (true)
        {
            RawTimer = File.ReadAllText(PathToLog);
            yield return delay;
        }
    }
}
