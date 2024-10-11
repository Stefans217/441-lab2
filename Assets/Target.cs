using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private SpriteRenderer sprite;
    private bool onSelect;
    public bool isGoalTarget;
    public bool isCenterTarget;
    public bool isDistractorTarget;

    private TargetManager targetManager;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        targetManager = FindObjectOfType<TargetManager>();
    }
    void Update()
    {
        
    }

    public void SetColor(Color color)
    {
        sprite.color = color;
    }

    public void OnHighlight()
    {

    }

    public void OnHoverEnter()
    {
        if(onSelect)return;
        sprite.color = Color.yellow;
    }

    public void OnHoverExit()
    {
        //if(onSelect)return;
        if(isCenterTarget)sprite.color = Color.red;
        else if(isGoalTarget) sprite.color = Color.red;
        else sprite.color = Color.white;
    }

    public void OnSelect()
    {
        if (!isGoalTarget) targetManager.misclicks++;
        
        if (isCenterTarget)
        {
            onSelect = true;
            sprite.color = Color.green;
            StartCoroutine(DestroyGameObject(0.1f));
            targetManager.startGame();
            Debug.Log("Center target clicked. Starting script...");
            
        }
        else if (isGoalTarget)
        {
            onSelect = true;
            sprite.color = Color.green;
            StartCoroutine(DestroyGameObject(0.1f));
            targetManager.repCounter++;
            targetManager.removeTargets();
            Debug.Log("Goal target clicked. removing targets...");
        }
        //sprite.color = Color.green;
        //StartCoroutine(DestroyGameObject(0.1f));
    }

    public IEnumerator DestroyGameObject(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    
}
