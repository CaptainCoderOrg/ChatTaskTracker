using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class LogReader : MonoBehaviour
{
    [field: SerializeField]
    public float PollingSpeed { get; private set; } = 1;
    [field: SerializeField]
    public TaskDatabase Tasks { get; private set; }
    
    void Start()
    {
        StartCoroutine(PollLog());
    }

    public IEnumerator PollLog()
    {
        YieldInstruction delay = new WaitForSeconds(PollingSpeed);
        while (true)
        {
            Tasks.Refresh();
            yield return delay;
        }
    }
}
