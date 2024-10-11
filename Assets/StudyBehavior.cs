using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable] public class StudySettings
{
    public List<float> targetSizes;
    public List<float> targetAmplitudes;
    public List<float> EWToW_Ratio;
    public CursorType cursorType;
}

public enum CursorType
{
    PointCursor = 0,
    BubbleCursor = 1
}

public class StudyBehavior : MonoBehaviour
{
    public StudySettings StudySettings => studySettings;

    private CSVManager CSVManager;

    public int ParticipantID
    {
        get => participantID;
        set => participantID = value;
    }

    private int participantID;
    [SerializeField] private StudySettings studySettings;
    [SerializeField] private int repetitions;

    private float timer = 0f;
    private int misClick = 0;
    private int currentTrialIndex = 0;
    private int missedClicks;

    private string[] header =
    {
        "PID",
        "CT",
        "A",
        "W",
        "EWW",
        "MT",
        "MissedClicks"
    };

    private void Start()
    {
        CSVManager = FindObjectOfType<CSVManager>();
        LogHeader();
    }



    private void LogHeader()
    {
        CSVManager.AppendToCSV(header);
    }
}


