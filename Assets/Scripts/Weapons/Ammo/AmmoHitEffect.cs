using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }


    //set Ammo Hit Effect from passed in ammoHitEffecso details
    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        
        SetHitEffectColorGradient(ammoHitEffect.colorGradient);
        
        SetHitEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed, 
            ammoHitEffect.startLifetime, ammoHitEffect.effectGravity, ammoHitEffect.maxParticleNumber);
        
        SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);
        
        SetHitEffectParticleSprite(ammoHitEffect.sprite);
        
        SetHitEffectVelocityOverLifeTime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);

    }


    //set the hit effect particle system color gradient
    private void SetHitEffectColorGradient(Gradient gradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }



    //set hit effect particle system starting values
    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, 
        float startLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;
        
        mainModule.duration = duration;
        
        mainModule.startSize = startParticleSize;
        
        mainModule.startSpeed = startParticleSpeed;
        
        mainModule.startLifetime = startLifetime;
        
        mainModule.gravityModifier = effectGravity;
        
        mainModule.maxParticles = maxParticles;
    }


    //set hit effect particle system particle burst particle number
    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;
        
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);
        
        emissionModule.rateOverTime = emissionRate;
    }

  
    
    //set hit effect particle system sprite
    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        //set particle burst number
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }


    //set the hit effect velocity over lifetime
    private void SetHitEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

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