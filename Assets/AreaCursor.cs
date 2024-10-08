using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AreaCursor : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private ContactFilter2D contactFilter;

    private Camera mainCam;
    private List<Collider2D> results = new();
    private Collider2D previousDetectedCollider = new();

    private void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Collider2D detectedCollider = null;

        Physics2D.OverlapCircle(transform.position, radius, contactFilter, results);



        //Get Mouse Position on screen, and get the corresponding position in a Vector3 World Co-Ordinate
        Vector3 mousePosition = Input.mousePosition;

        //Change the z position so that cursor does not get occluded by the camera
        mousePosition.z += 9f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);

        //Detect how many targets
        //Change previous target back to default colour
        if (results.Count < 1)
        {
            UnHoverPreviousTarget();
            return;
        }
        else if (results.Count > 1)
        {
            UnHoverPreviousTarget();
            Debug.LogWarning("Too many targets in area");
            return;
        }
        else
        {
            detectedCollider = results[0];
            UnHoverPreviousTarget(detectedCollider);
            HoverTarget(detectedCollider);
        }

        //On Mouse Click, select the closest target
        if (Input.GetMouseButtonDown(0))
        {
            SelectTarget(detectedCollider);
        }

        previousDetectedCollider = detectedCollider;
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

    private void UnHoverPreviousTarget(Collider2D collider)
    {
        //Checking if the target detected in previous and current frame are the same
        //If target changes, change the previous target back to default colour
         if (previousDetectedCollider != null &&  collider != previousDetectedCollider)
        {
            if (previousDetectedCollider.TryGetComponent(out Target t))
            {
                t.OnHoverExit();
            }
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
