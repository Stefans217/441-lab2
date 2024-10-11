using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        bubbleCursor = FindObjectOfType<BubbleCursor>();
        inputField = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();

        areaCursor = FindObjectOfType<AreaCursor>();
        

        GameObject targetSpawner = GameObject.Find("TargetSpawner");
        targetManager = targetSpawner.GetComponent<TargetManager>();
        
        studyBehavior = FindObjectOfType<StudyBehavior>();
        Debug.Log("Amplitude: " + studyBehavior.StudySettings.targetAmplitudes[0]);

        CSVManager.SetFilePath(studyBehavior.StudySettings.cursorType.ToString());
        DontDestroyOnLoad(this);
    }

    public void Start()
    {

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
        //print end game message here
        endCanvas.gameObject.SetActive(true);
    }


    //Start Study is called on GUI Start Study button click.
    //It needs to hide the GUI, and show the center button
    //that must be clicked.
    public void StartStudy()
    {
        if(inputField.text == string.Empty) return;
        participantID = int.Parse(inputField.text);
        studyBehavior.ParticipantID = participantID;
        studyCanvas.gameObject.SetActive(false);

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


        targetManager.initialize();
        Debug.Log("Study Started, Canvas hidden.");
    }
}