using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/*
 *TargetManager Object controls all targets created.
 *
 *Start with the centerTarget, and run startGame() when it's clicked.
 *startGame():
 * - Creates the goal target
 * - then the distractor targets using the goal targets position.
 * - then the random targets bounded by the distractor targets positions.
 *
 *Remove all objects when the goal target is clicked
 *Call StartGame() again to begin a new repetition.
 *
*/


public class TargetManager : MonoBehaviour
{
    [SerializeField] public GameObject target;
    [SerializeField] public Target centerTarget;

    [SerializeField] private List<float> targetSizes;
    [SerializeField] private List<float> targetAmplitudes;
    [SerializeField] private int numTargets;

    private Stopwatch stopwatch;
    private Vector3 goal;
    private List<Vector3> distractors;
    private List<Vector3> points;
    private Vector3 ScreenCentreInWorld => mainCamera.ScreenToWorldPoint(screenCentre);
    
    private List<float> randomSizes;
    private List<Target> targetList = new();
    private Vector2 screenCentre;
    private Camera mainCamera;

    private GameManager gameManager;
    private StudyBehavior studyBehavior;


    public int sizeIndex = 0;
    public int ampIndex = 0;
    public int ewIndex = 0;
    [SerializeField] public int repetitions;
    public int repCounter = 0;
    public int misclicks = 0;

    private void Awake()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        gameManager = GameObject.FindObjectOfType<GameManager>();
        studyBehavior = GameObject.FindObjectOfType<StudyBehavior>();
        stopwatch = new Stopwatch();
    }

    private void TargetManagerLogData(long timer)
    {
        string[] data =
        {
            studyBehavior.ParticipantID.ToString(),
            studyBehavior.StudySettings.cursorType.ToString(),
            studyBehavior.StudySettings.targetAmplitudes[ampIndex].ToString(),
            studyBehavior.StudySettings.targetSizes[sizeIndex].ToString(),
            studyBehavior.StudySettings.EWToW_Ratio[ewIndex].ToString(),
            timer.ToString(),
            misclicks.ToString()
        };
        CSVManager.AppendToCSV(data);


    }

    //referenced from GameManager to create the center target.
    public void initialize()
    {

        stopwatch.Stop();
        long timeElapsed = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("MT: " + timeElapsed);
        TargetManagerLogData(timeElapsed);
        

        bool gameEnded = false;


        //variable control:
        UnityEngine.Debug.Log("Rep " + repCounter + ", amp: " + studyBehavior.StudySettings.targetAmplitudes[ampIndex] + ", size: " + studyBehavior.StudySettings.targetSizes[sizeIndex] + ", EWtoW: " + studyBehavior.StudySettings.EWToW_Ratio[ewIndex]);
        if(repCounter >= repetitions)
        {
            repCounter = 0;
            ampIndex++;
            if(ampIndex >= studyBehavior.StudySettings.targetAmplitudes.Count)
            {
                UnityEngine.Debug.Log("ampIndex reset, incrementing sizeIndex.");
                ampIndex = 0;
                sizeIndex++;
                if(sizeIndex >= studyBehavior.StudySettings.targetSizes.Count)
                {
                    UnityEngine.Debug.Log("sizeIndex reset, incrementing EWIndex.");
                    sizeIndex = 0;
                    ewIndex++;
                    if(ewIndex >= studyBehavior.StudySettings.EWToW_Ratio.Count)
                    {
                        UnityEngine.Debug.Log("ewIndex reset");
                        UnityEngine.Debug.Log("Study complete.");
                        gameEnded = true;
                    }
                }
            }
        }
        

        if (!gameEnded)
        {
            SpawnStartTarget();
        }
        else
        {
            gameManager.endGame();
        }

    }


    //referenced from the centerTarget click event to start the game.
    public void startGame()
    {
        misclicks = 0;
        stopwatch.Reset();
        stopwatch.Start();
        SpawnTargets();
        UnityEngine.Debug.Log("Center of screen: " + ScreenCentreInWorld);
    }



    //The start target is a circle in the center of the
    //screen that the user must click to begin the test.
    //This function is called after the begin study button
    //is clicked on the gui.
    private void SpawnStartTarget()
    {
        float size = 1f;
        GameObject centerTargetObject = Instantiate(target, new Vector3(0,0,0), Quaternion.identity, transform);
        centerTargetObject.transform.localScale = Vector3.one * size;
        centerTarget = centerTargetObject.GetComponent<Target>();
        centerTarget.isCenterTarget = true;
        
        SpriteRenderer spriteRenderer = centerTarget.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
    }



    private void SpawnTargets()
    {
        goal = GenerateGoalTarget();
        distractors = GenerateDistractorTargets(goal, studyBehavior.StudySettings.EWToW_Ratio[ewIndex]);
        points = GenerateRandomPoints();

        List<float> randomSizes = GenerateRandomSizes();
        int randomIndex = UnityEngine.Random.Range(0, points.Count);

        //find goal size.
        float goalTargetSize = studyBehavior.StudySettings.targetSizes[sizeIndex];
        GameObject goalTargetObject = Instantiate(target, goal, Quaternion.identity, transform);
        goalTargetObject.transform.localScale = Vector3.one * goalTargetSize;
        goalTargetObject.name = "Goal";

        Target goalTarget = goalTargetObject.GetComponent<Target>();
        goalTarget.isGoalTarget = true;
        
        goalTargetObject.GetComponent<SpriteRenderer>().color = Color.red;

        for (int i = 0; i < 4; i++)
        {
            GameObject distractorObject = Instantiate(target, distractors[i], Quaternion.identity, transform);
            distractorObject.transform.localScale = Vector3.one * goalTargetSize;
            distractorObject.name = "distractor " + i;
            distractorObject.tag = "Target";
            distractorObject.GetComponent<SpriteRenderer>().color = Color.gray;
            Target distractorTarget = distractorObject.GetComponent<Target>();
            distractorTarget.isDistractorTarget = true;
        }

        for (int i = 0; i < numTargets; i++)
        {
            
            GameObject targetObject = Instantiate(target, points[i], Quaternion.identity, transform);
            targetObject.name = "target";
            targetObject.tag = "Target";

            targetObject.transform.localScale = Vector3.one * randomSizes[i];


        }

    }

    public void removeTargets()
    {
        GameObject[] targetsToDelete = GameObject.FindGameObjectsWithTag("Target");
        foreach(GameObject targetToDelete in targetsToDelete)
        {
            Destroy(targetToDelete);
            
        }
        UnityEngine.Debug.Log("All targets destroyed...");
        initialize();
        //if rep < studyBehavior.repetitions then intialize study again
        //else show canvas displaying thank you screen

    }

    

    List<Vector3> GenerateRandomPoints()
    {
        List<Vector3> pointList = new();
        
        for (int i = 0; i < numTargets; i++)
        {

            bool isValidPoint = false;
            Vector3 randomWorldPoint;

            
            do{
                float randomX = UnityEngine.Random.Range(0, Screen.width);
                float randomY = UnityEngine.Random.Range(0, Screen.height);
                float z = 10f;
               
                Vector3 randomScreenPoint = new(randomX, randomY, z);
                randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);

                isValidPoint = true;
                //check position validity

                float minX = Mathf.Min(distractors[0].x, distractors[1].x, distractors[2].x, distractors[3].x);
                float maxX = Mathf.Max(distractors[0].x, distractors[1].x, distractors[2].x, distractors[3].x);
                float minY = Mathf.Min(distractors[0].y, distractors[1].y, distractors[2].y, distractors[3].y);
                float maxY = Mathf.Max(distractors[0].y, distractors[1].y, distractors[2].y, distractors[3].y);
                if (randomWorldPoint.x >= minX && randomWorldPoint.x <= maxX && randomWorldPoint.y >= minY && randomWorldPoint.y <= maxY)
                {
                    isValidPoint = false;  // inside distractor area.
                }

            } while (!isValidPoint) ;

            pointList.Add(randomWorldPoint);
        }
        return pointList;
    }


    Vector3 GenerateGoalTarget()
    {

        float amplitude = studyBehavior.StudySettings.targetAmplitudes[ampIndex];//studyBehavior.StudySettings.targetAmplitudes[0];
        Vector3 direction = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 goalTargetPosition = ScreenCentreInWorld + direction * amplitude;
        goalTargetPosition.z = 10f;
        //goalTarget's positon relies on the amplitude of the study settings

        return goalTargetPosition;
    }


    List<Vector3> GenerateDistractorTargets(Vector3 goalTargetPosition, float distanceFromTarget)
    {
        List<Vector3> distractorList = new();

        Vector3[] offsets = new Vector3[]
        {
            new Vector3(distanceFromTarget, distanceFromTarget, 11f),
            new Vector3(-distanceFromTarget, distanceFromTarget, 11f),
            new Vector3(distanceFromTarget, -distanceFromTarget, 11f),
            new Vector3(-distanceFromTarget, -distanceFromTarget, 11f)
        };

        foreach (var offset in offsets)
        {
            Vector3 screenPoint = goalTargetPosition + offset;
            //Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
            distractorList.Add(screenPoint);
        }

        return distractorList;
    }

    List<float> GenerateRandomSizes()
    {
        List<float> sizes = new();
        for (int i = 0; i < numTargets; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, targetSizes.Count);
            sizes.Add(targetSizes[randomIndex]);
        }

        return sizes;
    }
}
