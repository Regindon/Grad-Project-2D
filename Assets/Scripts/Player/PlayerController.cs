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
    private const string BACK_RIGHT = "BackRight";
    private const string BACK_LEFT = "BackLeft";
    private const string SKILL_LEFT = "SkillLeft";
    private const string SKILL_RIGHT = "SkillRight";
    private bool isDisabled = false;

    private bool isDashButtonDown = false;
    private bool canDash = true;
    private float timer;
    private float dashCooldown = 3f;
    private Vector3 moveDir;
    [SerializeField] private LayerMask dashLayerMask;
    
    public Vector3 mouseWorldPosition;

    void Deneme()
    {
        Debug.Log("im working");
    }

    private void Awake()
    {
        
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
        moveDir = new Vector3(inputHorizontal, inputVertical).normalized;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mouseWorldPosition = GetMouseWorldPosition();
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

        if (!canDash)
        {
            timer += Time.deltaTime;
            if (timer>dashCooldown)
            {
                canDash = true;
                timer = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)&& canDash)
        {
            isDashButtonDown = true;
        }
    }

   

    private void FixedUpdate()
    {
        Debug.Log(inputHorizontal);
        
        
        if (inputHorizontal != 0 || inputVertical!=0)
        {
            if (inputHorizontal != 0 && inputVertical!=0)
            {
                inputHorizontal *= speedLimiter;
                inputVertical *= speedLimiter;
            }
            
            rb.velocity = new Vector2(inputHorizontal*walkSpeed,inputVertical*walkSpeed);

            if (RightHalf()&&inputHorizontal>0)
            {
                ChangeAnimationState(WALK_RIGHT);
            }
            else if (!RightHalf()&&inputHorizontal>0)
            {
                ChangeAnimationState(BACK_RIGHT);
                Debug.Log("worked2");
            }
            else if (!RightHalf()&&inputHorizontal<0)
            {
                ChangeAnimationState(WALK_LEFT);
            }
            else if (RightHalf()&&inputHorizontal<0)
            {
                ChangeAnimationState(BACK_LEFT);
                Debug.Log("worked1");
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

        if (isDashButtonDown)
        {
            canDash = false;
            float dashAmount = 2f;
            Vector3 dashPosition = transform.position + moveDir * dashAmount;
            
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, moveDir, dashAmount,dashLayerMask);
            if (raycastHit2D.collider !=null)
            {
                dashPosition = raycastHit2D.point;
            }
            rb.MovePosition(dashPosition);
            isDashButtonDown = false;
        }
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    
    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    
    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
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
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
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
