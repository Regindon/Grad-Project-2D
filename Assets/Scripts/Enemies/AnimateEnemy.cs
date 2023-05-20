using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        //subscribe to events
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        
        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;

        
        enemy.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        //unsubscribes from events
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        
        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;

        
        enemy.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }


    
    
    
    //on weapon aim event handler
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitialiseAimAnimationParameters();
        SetAimWeaponAnimationParameters(aimWeaponEventArgs.aimDirection);
    }


    
    
    
    //on movement event handler
    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        SetMovementAnimationParameters();
    }


    
    
    //on idle event handler
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
    }


    
    
    
    //initialise aim animation parameters
    private void InitialiseAimAnimationParameters()
    {
        enemy.animator.SetBool(Settings.aimUp, false);
        enemy.animator.SetBool(Settings.aimUpRight, false);
        enemy.animator.SetBool(Settings.aimUpLeft, false);
        enemy.animator.SetBool(Settings.aimRight, false);
        enemy.animator.SetBool(Settings.aimLeft, false);
        enemy.animator.SetBool(Settings.aimDown, false);
    }


    
    
    
    //set movement animation parameters
    private void SetMovementAnimationParameters()
    {
        
        enemy.animator.SetBool(Settings.isIdle, false);
        enemy.animator.SetBool(Settings.isMoving, true);
    }



    
    
    //set idle animation parameters
    private void SetIdleAnimationParameters()
    {
        
        enemy.animator.SetBool(Settings.isMoving, false);
        enemy.animator.SetBool(Settings.isIdle, true);
    }

    
    
    
    
    //Set aim animation parameters
    private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
    {
       
        switch (aimDirection)
        {
            case AimDirection.Up:
                enemy.animator.SetBool(Settings.aimUp, true);
                break;

            case AimDirection.UpRight:
                enemy.animator.SetBool(Settings.aimUpRight, true);
                break;

            case AimDirection.UpLeft:
                enemy.animator.SetBool(Settings.aimUpLeft, true);
                break;

            case AimDirection.Right:
                enemy.animator.SetBool(Settings.aimRight, true);
                break;

            case AimDirection.Left:
                enemy.animator.SetBool(Settings.aimLeft, true);
                break;

            case AimDirection.Down:
                enemy.animator.SetBool(Settings.aimDown, true);
                break;
        }
    }
}