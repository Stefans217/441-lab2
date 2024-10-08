using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleCursor : MonoBehaviour
{
    public float baseRadius = 0.5f;
    public float maxRadius = 2f;
    public float radius;
    public bool bubbleCursorActive;

    [SerializeField] private ContactFilter2D contactFilter;
    [SerializeField] private Transform bubbleVisual;

    private Camera mainCam;
    private List<Collider2D> results = new();
    private Collider2D previousDetectedCollider = new();
    private Collider2D closestTargetCollider;


    private void Awake()
    {
        bubbleCursorActive = false;
        mainCam = Camera.main;
        bubbleVisual = transform.Find("BubbleVisual");
        bubbleVisual.localScale = Vector2.zero;
  
    }

    void Update()
    {
            
        //Get Mouse Position on screen, and get the corresponding position in a Vector3 World Co-Ordinate
        Vector3 mousePosition = Input.mousePosition;
        //Change the z position so that cursor does not get occluded by the camera
        mousePosition.z += 9f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);


        //Collider2D detectedCollider = null;

        Physics2D.OverlapCircle(transform.position, maxRadius, contactFilter, results);

        closestTargetCollider = FindClosestTarget(results);

        

        if(closestTargetCollider != previousDetectedCollider)
        {
            UnHoverPreviousTarget();
        }
        if (closestTargetCollider != null)
        {
            AdjustBubbleRadius(closestTargetCollider);
            HoverTarget(closestTargetCollider);
        }
        else
        {
            ResetBubbleRadius();
            UnHoverPreviousTarget();
        }


        //On Mouse Click, select the closest target
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            if(closestTargetCollider != null)
            {
                SelectTarget(closestTargetCollider);
            }
            
        }


        previousDetectedCollider = closestTargetCollider;
    }

    private Collider2D FindClosestTarget(List<Collider2D> colliders)
    {
        Collider2D closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            float distance = Vector2.Distance(transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = col;
            }
        }
        return closest;
    }

    private void AdjustBubbleRadius(Collider2D targetCollider)
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetCollider.transform.position);
        float effectiveWidth = CalculateEffectiveWidth(distanceToTarget);
        radius = Mathf.Clamp(effectiveWidth, baseRadius, maxRadius);

        
        bubbleVisual.localScale = Vector3.one * radius * 2;
    }

    private void ResetBubbleRadius()
    {
        radius = baseRadius;


        bubbleVisual.localScale = Vector3.one * radius * 2;
    }

    private float CalculateEffectiveWidth(float distanceToTarget)
    {
        return Mathf.Max(baseRadius, distanceToTarget / 2);
    }



    private void HoverTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnHoverEnter();
        }
        else
        {
            Debug.LogWarning("Not a valid Target?");
        }
    }

    private void UnHoverPreviousTarget()
    {
         if (previousDetectedCollider != null)
        {
            if (previousDetectedCollider.TryGetComponent(out Target t))
            {
                t.OnHoverExit();
            }
        }
    }

    //private void UnHoverPreviousTarget(Collider2D collider)
    //{
    //    //Checking if the target detected in previous and current frame are the same
    //    //If target changes, change the previous target back to default colour
    //     if (previousDetectedCollider != null &&  collider != previousDetectedCollider)
    //    {
    //        if (previousDetectedCollider.TryGetComponent(out Target t))
    //        {
    //            t.OnHoverExit();
    //        }
    //    }
    //}

    private void ExpandBubble(Collider2D collider, float expandedBubbleRadius)
    {
        if(collider.TryGetComponent(out Target target))
        {
            target.OnHoverEnter();
            float bubbleScale = expandedBubbleRadius * 2 + 1f;
            bubbleScale = Mathf.Clamp(bubbleScale, 0, radius);
            
        }
    }

    void SelectTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnSelect();
        }
    }


    //Debug code
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
