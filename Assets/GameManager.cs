using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    [SerializeField] private TMP_InputField inputField;
    [SerializeField] public Canvas studyCanvas;
    [SerializeField] public Canvas endCanvas;

    private BubbleCursor bubbleCursor;
    private AreaCursor areaCursor;
    public TargetManager targetManager;
    private StudyBehavior studyBehavior;

    private int participantID;
    

    private void Awake()
    {
        GameObject targetSpawner = GameObject.Find("TargetSpawner");
        targetManager = targetSpawner.GetComponent<TargetManager>();

        studyBehavior = FindObjectOfType<StudyBehavior>();

        bubbleCursor = FindObjectOfType<BubbleCursor>();
        areaCursor = FindObjectOfType<AreaCursor>();

        inputField = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        CSVManager.SetFilePath(studyBehavior.StudySettings.cursorType.ToString());

        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        //determines cursor type and sets the start screen to a point cursor.
        SetCursor(studyBehavior.StudySettings.cursorType);
        bubbleCursor.gameObject.SetActive(false);
    }

    public void SetCursor(CursorType cursor)
    {
        bubbleCursor.radius = cursor switch
        {
            CursorType.PointCursor => 0.015f,
            CursorType.BubbleCursor => 10f,
            _ => throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null)
        };
    }

    public void endGame()
    {
        //prints the end game message.
        endCanvas.gameObject.SetActive(true);
    }


    //Start Study is called on GUI Start Study button click.
    //It hides the GUI and shows the center button
    //that must be clicked.
    public void StartStudy()
    {
        //if participant ID was not entered then do nothing.
        if(inputField.text == string.Empty) return;

        participantID = int.Parse(inputField.text);
        studyBehavior.ParticipantID = participantID;

        //hide the canvas (GUI)
        studyCanvas.gameObject.SetActive(false);

        //set cursor type.
        if(studyBehavior.StudySettings.cursorType == CursorType.PointCursor)
        {
            bubbleCursor.gameObject.SetActive(false);
            areaCursor.gameObject.SetActive(true);
        }
        else
        {
            bubbleCursor.gameObject.SetActive(true);
            areaCursor.gameObject.SetActive(false);
        }

        //start the target manager script
        targetManager.initialize();
        Debug.Log("Study Started, Canvas hidden.");
    }
}