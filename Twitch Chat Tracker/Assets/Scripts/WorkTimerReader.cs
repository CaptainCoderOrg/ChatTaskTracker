using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;

public class WorkTimerReader : MonoBehaviour
{
    public const string WorkCommand = "!work";
    public const string BreakCommand = "!break";
    [field: SerializeField]
    public AudioSource BreakTimeSound { get; private set; }
    [field: SerializeField]
    public bool PlayOnTimesUp = false;
    [field: SerializeField]
    public TextMeshProUGUI BreakText { get; private set; }
    [field: SerializeField]
    public float PollingSpeed { get; private set; } = 1;

    [field: SerializeField]
    public string PathToLog { get; private set; }
    [field: SerializeField]
    public int Minutes { get; private set; }
    [field: SerializeField]
    public long StartTime { get; private set; }
    [field: SerializeField]
    public DateTime EndTime { get; private set; }
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
            Minutes = int.Parse(parts[1]);
            StartTime = long.Parse(parts[2]);
            EndTime = DateTimeOffset.FromUnixTimeSeconds(StartTime).LocalDateTime + new TimeSpan(0, Minutes, 0);
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

            yield return delay;
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
