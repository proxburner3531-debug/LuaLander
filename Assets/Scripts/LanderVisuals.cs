using UnityEngine;

public class LanderVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem LeftThrusterParticleSystem;
    [SerializeField] private ParticleSystem RightThrusterParticleSystem;
    [SerializeField] private ParticleSystem MiddleThrusterParticleSystem;
    [SerializeField] private GameObject landerExplosionVFX;

    private Lander lander;

    

    private void Awake()
    {
        lander = GetComponent<Lander>();

        lander.OnUpForce += Lander_OnUpForce;
        lander.OnLeftForce += Lander_OnLeftForce;
        lander.OnRightForce += Lander_OnRightForce;
        lander.OnBeforeForce += Lander_OnBeforeForce;

        SetEnabledThrusterParticleSystem(LeftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(RightThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(MiddleThrusterParticleSystem,false);
    }

    private void Start()
    { 
        lander.OnLanded += Lander_OnLanded;
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.WrongLandingArea :
            case Lander.LandingType.TooFastLanding:
            case Lander.LandingType.TooSteepAngle:

                Instantiate(landerExplosionVFX,transform.position,Quaternion.identity);
                gameObject.SetActive(false);
                break;
        }
        
    }

    private void Lander_OnBeforeForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(LeftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(RightThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(MiddleThrusterParticleSystem, false);
    }

    private void Lander_OnRightForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(LeftThrusterParticleSystem, true);

    }

    private void Lander_OnLeftForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(RightThrusterParticleSystem, true);
       
    }

    private void Lander_OnUpForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(LeftThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(RightThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(MiddleThrusterParticleSystem, true);

    }

    private void SetEnabledThrusterParticleSystem(ParticleSystem particleSystem,bool enabled)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = enabled;
    }
}
