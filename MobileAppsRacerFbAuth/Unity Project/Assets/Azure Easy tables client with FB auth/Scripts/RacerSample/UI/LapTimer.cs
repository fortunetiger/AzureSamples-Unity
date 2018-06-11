using System;
using UnityEngine;
using UnityEngine.UI;

public class LapTimer : MonoBehaviour
{
    public static Action<float> AfterMostRecentScoreSet;
    private Text timeText;
    private float lapTime = 0f;
    private bool raceFinished = false;

    private float mostRecentScore;

    public float MostRecentScore
    {
        get
        {
            return mostRecentScore;
        }

        set
        {
            mostRecentScore = value;
            if (AfterMostRecentScoreSet != null)
                AfterMostRecentScoreSet.Invoke(mostRecentScore);
        }
    }

    private void Start()
    {
        timeText = GetComponent<Text>();
    }

    private void Update()
    {
        UpdateLapTime();
    }

    private void UpdateLapTime()
    {
        if (!raceFinished)
        {
            lapTime += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(lapTime);
            timeText.text = timeSpan.Minutes + ":" + timeSpan.Seconds + ":" + timeSpan.Milliseconds;
        }
    }

    private void OnRaceFinished()
    {
        if (!raceFinished)
        {
            raceFinished = true;
            MostRecentScore = lapTime;
        }
    }

    private void OnEnable()
    {
        Checkpoint.RaceFinished += OnRaceFinished;   
    }

    private void OnDisable()
    {
        Checkpoint.RaceFinished -= OnRaceFinished;
    }
}
