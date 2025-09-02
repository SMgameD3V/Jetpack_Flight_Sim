using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class JetpackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private FuelManager fuelManager;
    [SerializeField] private OverheatManager overheatManager;
    [SerializeField] private Animator animator;

    [Header("Jetpack Settings")]
    [Tooltip("Target upward velocity (units/second) while holding jetpack")]
    [SerializeField] private float jetpackThrust = 2f;
    [Tooltip("How fast the vertical speed moves toward target (units/sec^2). Higher = snappier")]
    [SerializeField] private float jetpackAcceleration = 40f;
    [Tooltip("Base gravity (negative)")]
    [SerializeField] private float gravity = -9.81f;
    [Tooltip("Gravity multiplier when falling downwards")]
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [Tooltip("Fuel refill per second while grounded (idle OR moving)")]
    [SerializeField] private float groundRefillRate = 25f;
    [Tooltip("Minimum vertical velocity when grounded (keeps CC grounded)")]
    [SerializeField] private float groundedVertical = -2f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem[] jetpackParticles;
    [SerializeField] private AudioSource jetpackSound;

    private float verticalSpeed = 0f;
    private bool wasJetpacking = false;
    private ThirdPersonController tpc;

    private int animIDJetpack;

    private void Awake()
    {
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (fuelManager == null) fuelManager = GetComponent<FuelManager>();
        if (overheatManager == null) overheatManager = GetComponent<OverheatManager>();
        tpc = GetComponent<ThirdPersonController>();

        if (animator == null) animator = GetComponent<Animator>();
        animIDJetpack = Animator.StringToHash("IsJetpacking");
    }

    private void Update()
    {

        float dt = Time.deltaTime;

        // combined grounded check (use TPC's ground logic if available, otherwise CC.isGrounded)
        bool groundedTPC = (tpc != null) && tpc.Grounded;
        bool groundedCC  = characterController.isGrounded;
        bool grounded    = groundedTPC || groundedCC;

        // keep a small downward velocity when grounded to ensure CC stays on ground
        if (grounded && verticalSpeed < groundedVertical) verticalSpeed = groundedVertical;

        // input + conditions
        bool jetKey = IsJetpackHeld();
        bool hasFuel = fuelManager.HasFuel();
        bool isOverheated = overheatManager.IsOverheated();

        // Overheat and Fuel Chck Logic
        bool canJetpack = jetKey && hasFuel && !isOverheated;

        if (canJetpack)
        {
            float target = jetpackThrust;
            verticalSpeed = Mathf.MoveTowards(verticalSpeed, target, jetpackAcceleration * dt);

            // consume fuel & heat
            fuelManager.ConsumeFuel(dt);
            overheatManager.IncreaseHeat(dt);

            PlayJetpackFXIfNeeded();
            if (animator) animator.SetBool(animIDJetpack, true);
        }
        else
        {
            // Not jetpacking, either grounded (refill) or apply gravity mid-air
            if (grounded)
            {
                if (groundRefillRate > 0f)
                    fuelManager.RefillFuel(groundRefillRate * dt);

                // when grounded verticalSpeed doesn't accumulate upward
                if (verticalSpeed > groundedVertical) verticalSpeed = groundedVertical;

                overheatManager.CoolDown(dt);
                StopJetpackFXIfNeeded();
            }
            else
            {
                // Apply gravity. Make falling fall faster.
                float mult = verticalSpeed < 0f ? fallGravityMultiplier : 1f;
                verticalSpeed += gravity * mult * dt;
                overheatManager.CoolDown(dt);
                StopJetpackFXIfNeeded();
            }
            if (animator) animator.SetBool(animIDJetpack, false);
        }

        // clamp vertical speed
        verticalSpeed = Mathf.Clamp(verticalSpeed, -100f, 100f);

        // Apply vertical velocity: pass to ThirdPersonController if present (so only it moves the controller),
        // otherwise fallback to moving the CharacterController directly.
        if (tpc != null)
        {
            tpc.SetExternalVerticalVelocity(verticalSpeed);
        }
        else
        {
            
            characterController.Move(Vector3.up * verticalSpeed * dt);
        }

    }

    private bool IsJetpackHeld()
    {
#if ENABLE_INPUT_SYSTEM
        
        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) return true;
        if (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed) return true; // A / Cross
#endif
        
        return Input.GetButton("Jump");
    }

    private void PlayJetpackFXIfNeeded()
    {
        if (wasJetpacking) return;
        if (jetpackParticles != null)
        {
            foreach (var ps in jetpackParticles)
                if (ps && !ps.isPlaying) ps.Play();
        }
        if (jetpackSound && !jetpackSound.isPlaying) jetpackSound.Play();
        wasJetpacking = true;
    }

    private void StopJetpackFXIfNeeded()
    {
        if (!wasJetpacking) return;
        if (jetpackParticles != null)
        {
            foreach (var ps in jetpackParticles)
                if (ps && ps.isPlaying) ps.Stop();
        }
        if (jetpackSound && jetpackSound.isPlaying) jetpackSound.Stop();
        wasJetpacking = false;
    }
}
