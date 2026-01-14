using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class NewSurfBoardController : MonoBehaviour
{
    public Transform[] floaters; // Brauchen wir nur noch für die Normale
    public Rigidbody rigidBody;

    [Header("Settings")]
    public float hoverHeight = 0.5f;   // Wie hoch über dem Wasser schweben wir?
    public float hoverForce = 50f;     // Wie stark drückt es hoch?
    public float hoverDamper = 5f;     // Anti-Wippen
    public float alignSpeed = 5f;      // Wie schnell passt sich Rotation der Welle an?
    public float sampleDist = 0.2f;
    public float acceleration = 0.2f;

    public float sideResistanceMultiplier = 0.2f;
    public float frontResistanceMultiplier = 1f;
    public float rotationMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float startSpeed = 10f;
    public float startSpeedDuration = 5f;
    public float waterForce = 10f;
    public float gravityForce = 1f;
    public float waterBreakPoint = 10f;

    private Vector3 currentRotationInput;
    void Start()
    {
        // WICHTIG: Wir verbieten der Physik, das Board zu drehen!
        //rigidBody.freezeRotation = true;

        //CalculateStartVelocity();
    }
    void Update() {
        //HandleInput();
    }
    void FixedUpdate()
    {
        // Vector3 currentSpeed;
        // float startSpeed;

        // Calculate Estimated Speed Position, (Positiona fter no resisistance at all)
        // if under Water and speed > waterBreakPoint-> Go under Water, else set Transform at waterheight
        // If not under Water ->set Transform to Estimated Speed position
        // Apply Gravitiy
        // Apply Rotation

        //CalculateHeight();
        //Vector3 waterSlope = GetNormalAtPosition(transform.position);
        //CalculateWaterVelocity(waterSlope);




        //CalculateRotation();
    }
    void CalculateWaterVelocity(Vector3 waterSlope) { 
        Vector3 currentVelocity = rigidBody.linearVelocity;
        Vector3 desiredVelocity = waterSlope * speedMultiplier;

        // Board WiderStand
        float angle = Vector3.Angle(desiredVelocity, transform.forward);
        float foldedAngle = angle > 90f ? 180f - angle : angle;
        float finalResistanceMultiplier = (((foldedAngle / 90) * frontResistanceMultiplier) * ((1 - (foldedAngle / 90)) * sideResistanceMultiplier)) / 2;
        desiredVelocity = desiredVelocity * finalResistanceMultiplier;

        Vector3 finalVelocity = new Vector3(desiredVelocity.x, currentVelocity.y, desiredVelocity.z);
        rigidBody.linearVelocity = Vector3.Lerp(currentVelocity, finalVelocity, acceleration * Time.fixedDeltaTime);
    }

    void CalculateStartVelocity() {
        rigidBody.AddForce(transform.forward * startSpeed, ForceMode.Acceleration);
    }
    void CalculateHeight()
    {
        
        float waveHeight = RootWaveManager.instance.GetWaveHeight(transform.position);
        float currentHeight = transform.position.y;

        float diff = waveHeight - currentHeight;
        float velocityY = rigidBody.linearVelocity.y;


        float force = (diff * hoverForce) - (velocityY * hoverDamper);
        if (diff > 0 && velocityY > waterBreakPoint)
        {
            rigidBody.AddForce(Vector3.up * force, ForceMode.Acceleration);
            //Apply WaterForce
        }
        else { 
            
        }


        

    }

    void CalculateRotation()
    {
        Vector3 waterSlope = GetNormalAtPosition(transform.position);

        Vector3 currentForward = transform.forward;

        Vector3 targetUp = waterSlope;
        Vector3 targetForward = Vector3.ProjectOnPlane(currentForward, targetUp).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(targetForward, targetUp);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * alignSpeed);
        transform.Rotate(currentRotationInput);
    }
    Vector3 GetNormalAtPosition(Vector3 pos)
    {
        float h0 = RootWaveManager.instance.GetWaveHeight(pos);
        float hX = RootWaveManager.instance.GetWaveHeight(pos + new Vector3(sampleDist, 0, 0));
        float hZ = RootWaveManager.instance.GetWaveHeight(pos + new Vector3(0, 0, sampleDist));
        Vector3 v1 = new Vector3(sampleDist, hX - h0, 0);
        Vector3 v2 = new Vector3(0, hZ - h0, sampleDist);
        return Vector3.Cross(v2, v1).normalized;
    }












    private Vector2 startPos = Vector2.zero;

    void HandleInput()
    {

        if (Touch.activeTouches.Count >= 1)
        {
            if (Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                startPos = Touch.activeTouches[0].screenPosition;
            }

            Vector2 abstand = startPos - Touch.activeTouches[0].screenPosition;

            if (abstand.y > 0)
            {
            }
            else
            {
            }
            currentRotationInput = Vector3.down * 0.01f * abstand.x;

        }
        else {
            currentRotationInput = Vector3.zero;
        }
    }
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
}
