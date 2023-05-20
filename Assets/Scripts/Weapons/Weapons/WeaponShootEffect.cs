using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake()
    {
        
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }


    //set the Shoot Effect from the passed in WeaponShootEffectSO and aimAngle
    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        
        SetShootEffectColorGradient(shootEffect.colorGradient);
        
        SetShootEffectParticleStartingValues(shootEffect.duration, shootEffect.startParticleSize, shootEffect.startParticleSpeed, 
            shootEffect.startLifetime, shootEffect.effectGravity, shootEffect.maxParticleNumber);
        
        SetShootEffectParticleEmission(shootEffect.emissionRate, shootEffect.burstParticleNumber);
        
        SetEmmitterRotation(aimAngle);
        
        SetShootEffectParticleSprite(shootEffect.sprite);
        
        SetShootEffectVelocityOverLifeTime(shootEffect.velocityOverLifetimeMin, shootEffect.velocityOverLifetimeMax);

    }


    
    //set the shoot effect particle system color gradient
    private void SetShootEffectColorGradient(Gradient gradient)
    {
   
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }


    
    //set shoot effect particle system starting values
    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, 
        float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;
        
        mainModule.duration = duration;
        
        mainModule.startSize = startParticleSize;
        
        mainModule.startSpeed = startParticleSpeed;
        
        mainModule.startLifetime = startLifetime;
        
        mainModule.gravityModifier = effectGravity;
        
        mainModule.maxParticles = maxParticles;

    }


    
    //set shoot effect particle system particle burst particle number
    private void SetShootEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);
        
        emissionModule.rateOverTime = emissionRate;
    }


    
    
    //set shoot effect particle system sprite
    private void SetShootEffectParticleSprite(Sprite sprite)
    {
        //set particle burst number
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);

    }

  
    
    //set the rotation of the emmitter to match the aim angle
    private void SetEmmitterRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }


    
    //set the shoot effect velocity over lifetime
    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = shootEffectParticleSystem.velocityOverLifetime;

        //define min max X velocity
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = minVelocity.x;
        minMaxCurveX.constantMax = maxVelocity.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        //define min max Y velocity
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = minVelocity.y;
        minMaxCurveY.constantMax = maxVelocity.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        //define min max Z velocity
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = minVelocity.z;
        minMaxCurveZ.constantMax = maxVelocity.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;

    }

}