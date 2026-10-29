using System;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines.Interpolators;
using UnityEngine.UI;
public class Lander : MonoBehaviour {

    private const float Gravity_Normal = 0.7f;

    public static Lander Instance { get; private set; }

    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnUpForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler OnFuelPickup;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
    }

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }


    public enum LandingType
    {
        Successful,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,
    }

    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }


    private Rigidbody2D LanderRigidBody2D;
    private float fuel;
    private float fuelMax = 10f;
    private State state;
    private void Awake() {

        Instance = this;

        fuel = fuelMax;
        state = State.WaitingToStart;

        LanderRigidBody2D = GetComponent<Rigidbody2D>();
        LanderRigidBody2D.gravityScale = 0f;
    }

    private void FixedUpdate() {

        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch(state)
        {
            default:
            case State.WaitingToStart:
                if (GameInput.Instance.IsUpArrowPressed() || GameInput.Instance.IsRightArrowPressed() || GameInput.Instance.IsLeftArrowPressed())
                {
                    LanderRigidBody2D.gravityScale = Gravity_Normal;
                    SetState(State.Normal);
                }

                break;
            case State.Normal:

                if (fuel <= 0f)
                {


                    return;
                }

                if (GameInput.Instance.IsUpArrowPressed() || GameInput.Instance.IsRightArrowPressed() || GameInput.Instance.IsLeftArrowPressed() || GameInput.Instance.GetMovementInputVector2() != Vector2.zero)
                {
                    LanderRigidBody2D.gravityScale = Gravity_Normal;
                    fuelConsumption();
                }


                float gamePadDeadZone = 0.4f;
                if (GameInput.Instance.IsUpArrowPressed() || GameInput.Instance.GetMovementInputVector2().y > gamePadDeadZone)
                {

                    float force = +700f;
                    LanderRigidBody2D.AddForce(force * transform.up * Time.fixedDeltaTime);
                    OnUpForce?.Invoke(this, EventArgs.Empty);

                }

                if (GameInput.Instance.IsLeftArrowPressed() || GameInput.Instance.GetMovementInputVector2().x < -gamePadDeadZone)
                {

                    float turnspeed = +100f;
                    LanderRigidBody2D.AddTorque(turnspeed * Time.fixedDeltaTime);
                    OnLeftForce?.Invoke(this, EventArgs.Empty);


                }

                if (GameInput.Instance.IsRightArrowPressed() || GameInput.Instance.GetMovementInputVector2().x > gamePadDeadZone)
                {

                    float turnspeed = -100f;
                    LanderRigidBody2D.AddTorque(turnspeed * Time.fixedDeltaTime);
                    OnRightForce?.Invoke(this, EventArgs.Empty);


                }
                break;

            case State.GameOver :

                break;
        }


        
    }

    private void OnCollisionEnter2D(Collision2D collision2D) {

        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingpad)) {

            Debug.Log("Crashed on Terrain!");

            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                dotVector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0f,
                score = 0
            });

            SetState(State.GameOver);
            return;

        }
        float minVelocitySoftLanding = 4f;
        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minTurnSoftLanding = 0.9f;

        if (collision2D.relativeVelocity.magnitude > minVelocitySoftLanding)
        {
            Debug.Log("Hard landing!");

            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLanding,
                dotVector = dotVector,
                landingSpeed = collision2D.relativeVelocity.magnitude,
                scoreMultiplier = 0f,
                score = 0
            });

            SetState(State.GameOver);
            return;
        }

        if (dotVector < minTurnSoftLanding)
        {
            Debug.Log("Landed too steep!");

            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepAngle,
                dotVector = dotVector,
                landingSpeed = collision2D.relativeVelocity.magnitude,
                scoreMultiplier = 0f,
                score = 0
            });

            SetState(State.GameOver);
            return;
        }

        Debug.Log("Successful Landing!");

        float maxLandingSpeedScore = 100f;
        float LandingSpeedScore = (minVelocitySoftLanding - collision2D.relativeVelocity.magnitude) * maxLandingSpeedScore;

        float maxLandingAngleScore = 100f;
        float dotVectorMax = 1f;
        float dotVectorScoreMultiplier = 10f;
        float LandingAngleScore = maxLandingAngleScore - (dotVectorMax - dotVector) * maxLandingAngleScore * dotVectorScoreMultiplier;

        Debug.Log("Speed Score : " + LandingSpeedScore);
        Debug.Log("Angle Score : " + LandingAngleScore);

        int score = Mathf.RoundToInt((LandingAngleScore + LandingSpeedScore) * landingpad.GetScoreMultiplier());

        Debug.Log("Score: " + score);

        OnLanded?.Invoke(this, new OnLandedEventArgs {
            landingType = LandingType.Successful,
            dotVector = dotVector,
            landingSpeed = collision2D.relativeVelocity.magnitude,
            scoreMultiplier = landingpad.GetScoreMultiplier(),
            score = score
        });

        SetState(State.GameOver);
        return;
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            float addFuelOnPickup = 10f;
            fuel += addFuelOnPickup;

            if (fuel > fuelMax)
            {
                fuel = fuelMax;
            }
            OnFuelPickup?.Invoke(this,EventArgs.Empty);
            fuelPickup.DestroySelf();
        }

        if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state
        });
    }
    private float fuelConsumption()
    {

        float consumption = 1f;
        fuel -= consumption * Time.deltaTime;
        return fuel;

    }

    public float GetFuel()
    { 
        return fuel;
    }

    public float GetFuelNormalized()
    {
        return fuel / fuelMax;
    }

    public float GetSpeedX()
    {
        return LanderRigidBody2D.linearVelocityX;
    }

    public float GetSpeedY()
    {
        return LanderRigidBody2D.linearVelocityY;
    }
}
