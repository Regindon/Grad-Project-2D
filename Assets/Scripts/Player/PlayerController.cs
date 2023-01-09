using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float walkSpeed = 4f;
    private float speedLimiter = .7f;
    private float inputHorizontal;
    private float inputVertical;
    private Animator animator;
    private string currentState;
    private string lastState;
    private const string IDLE_LEFT = "IdleLeft";
    private const string IDLE_RIGHT = "IdleRight";
    private const string WALK_LEFT = "WalkLeft";
    private const string WALK_RIGHT = "WalkRight";
    private const string SKILL_LEFT = "SkillLeft";
    private const string SKILL_RIGHT = "SkillRight";
    private bool isDisabled = false;

    private Vector3 mouseWorldPosition;

    void Deneme()
    {
        Debug.Log("im working");
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.Log(isDisabled);
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("q key working");
            if (currentState== WALK_LEFT || currentState==IDLE_LEFT)
            {
                ChangeAnimationState(SKILL_LEFT);
            }
            else
            {
                ChangeAnimationState(SKILL_RIGHT);
            }
        }
    }

   

    private void FixedUpdate()
    {
        
        
        if (inputHorizontal != 0 || inputVertical!=0)
        {
            if (inputHorizontal != 0 && inputVertical!=0)
            {
                inputHorizontal *= speedLimiter;
                inputVertical *= speedLimiter;
            }
            
            rb.velocity = new Vector2(inputHorizontal*walkSpeed,inputVertical*walkSpeed);

            if (RightHalf())
            {
                ChangeAnimationState(WALK_RIGHT);
            }
            else if (!RightHalf())
            {
                ChangeAnimationState(WALK_LEFT);
            }
        }
        else
        {
            rb.velocity = new Vector2(0f,0f);
            if (RightHalf())
            {
                ChangeAnimationState(IDLE_RIGHT);
            }
            else
            {
                ChangeAnimationState(IDLE_LEFT);
            }
        }
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState)
        {
            return;
        }

        if (!isDisabled)
        {
            animator.Play(newState);
            lastState = currentState;
            currentState = newState;
        }
        
    }
    
    bool RightHalf()
    {
        return mouseWorldPosition.x > transform.position.x;
    }

    private void skillStarted()
    {
        Debug.Log("started");
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        
        isDisabled = true;
        


    }
    private void skillFinished()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        
        Debug.Log("finished");
        StartCoroutine(RageBuff(1.5f, 5f));
        isDisabled = false;
        
    }

    IEnumerator RageBuff(float buffAmount, float duration)
    {
        walkSpeed += buffAmount;
        yield return new WaitForSeconds(duration);
        walkSpeed -= buffAmount;
    }
}
