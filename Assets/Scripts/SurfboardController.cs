using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SurfBoardController : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform oceanTF; // Reference if needed later

    [Header("General")]
    public float sampleDist = 0.2f;
    public float snapHeight = 1f;

    [Header("Movement Settings")]
    public float baseForwardSpeed = 10f;
    public float rotationSensitivity = 1.5f;
    public float velocityDamping = 0.98f; // Air resistance/Water drag
    public float steeringGrip = 1f;

    [Header("Physics Settings")]
    public float gravity = 9.81f;
    public float buoyancyForce = 15f; // Needs to be higher than gravity to float
    public float slopeSlideSpeed = 5f;
    public float waterBreakPoint = 3f; // Max depth before game over
    public float tooDeepPoint = 5f;
    public float baseHoverHeight = 1f;
    // State
    private Vector3 currentVelocity;
    private bool gameOver = false;
    private float currentTurnInput;

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Start()
    {
        currentVelocity = transform.forward * baseForwardSpeed;
    }

    void Update()
    {
        if (gameOver) return;
        HandleInput();
    }

    void FixedUpdate()
    {
        if (gameOver) return;

        // 1. Get Environment Data
        Vector3 waterSlopeNormal = GetWaterSlopeNormal(transform.position);
        float waveHeight = RootWaveManager.instance.GetWaveHeight(transform.position);
        float currentHeight = transform.position.y;
        float depth = waveHeight - currentHeight;

        if (depth + snapHeight > 0)
        {
            //UNDER WATER
            HandleWaterPhysics(depth, waterSlopeNormal);
            
        }
        else
        {
            //ABOVE WATER
            HandleAirPhysics();
        }
        HandlePosition(waveHeight, depth);
        // 3. Integrate Velocity (Move)
        //transform.position += currentVelocity * Time.fixedDeltaTime;

        // 4. Handle Rotation
        HandleRotation(waterSlopeNormal, depth > 0);
    }
    private void HandlePosition(float waveHeight, float depth) {
        Vector3 normalizedVelocity = currentVelocity * Time.fixedDeltaTime;

        if (currentVelocity.y < snapHeight && currentVelocity.y > -snapHeight/2 && depth < snapHeight && depth > -snapHeight/2)    // SNAP
        {
            Vector3 finalPos = new Vector3(normalizedVelocity.x + transform.position.x, (waveHeight + baseHoverHeight), normalizedVelocity.z + transform.position.z);
            transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime*15f);
            //transform.position = finalPos;
        }
        else
        {
             Vector3 finalPos = transform.position + currentVelocity * Time.fixedDeltaTime + new Vector3(0,baseHoverHeight,0);
             transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * 15f);
            //transform.position = finalPos;
        }
    }
    private void HandleWaterPhysics(float depth, Vector3 surfaceNormal)
    {
        if (depth > tooDeepPoint) { /* ... */ return; }

        Vector3 buoyancy = Vector3.up * (depth * buoyancyForce * Time.fixedDeltaTime);
        currentVelocity += buoyancy;

        Vector3 forwardForce = transform.forward * baseForwardSpeed * Time.fixedDeltaTime;
        currentVelocity += forwardForce;

        Vector3 downhillDirection = Vector3.ProjectOnPlane(Vector3.down, surfaceNormal).normalized;
        currentVelocity += downhillDirection * slopeSlideSpeed * Time.fixedDeltaTime;



        Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);

        localVelocity.x = Mathf.Lerp(localVelocity.x, 0, Time.fixedDeltaTime * steeringGrip);

        currentVelocity = transform.TransformDirection(localVelocity);



        currentVelocity *= velocityDamping;
    }

    private void HandleAirPhysics()
    {
        // Apply Gravity
        currentVelocity += Vector3.down * gravity * Time.fixedDeltaTime;
    }

    private void HandleRotation(Vector3 surfaceNormal, bool isSurfing)
    {
        // 1. Align with Water Surface
        if (isSurfing)
        {
            Vector3 targetUp = surfaceNormal;
            Vector3 targetForward = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;

            // Create a rotation that looks forward but keeps the "Up" vector aligned with the wave normal
            Quaternion targetRotation = Quaternion.LookRotation(targetForward, targetUp);

            // Smoothly interpolate to that rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.fixedDeltaTime);
        }

        // 2. Apply Player Turn Input
        // We rotate around the board's Up axis (Y)
        transform.Rotate(Vector3.up, currentTurnInput * rotationSensitivity, Space.Self);
    }

    private Vector3 GetWaterSlopeNormal(Vector3 pos)
    {
        // Calculate the Normal using the Cross Product of two tangent vectors
        float h0 = RootWaveManager.instance.GetWaveHeight(pos);
        float hX = RootWaveManager.instance.GetWaveHeight(pos + new Vector3(sampleDist, 0, 0));
        float hZ = RootWaveManager.instance.GetWaveHeight(pos + new Vector3(0, 0, sampleDist));

        Vector3 v1 = new Vector3(sampleDist, hX - h0, 0);
        Vector3 v2 = new Vector3(0, hZ - h0, sampleDist);

        // The Normal is perpendicular to the surface
        return Vector3.Cross(v2, v1).normalized;
    }

    // --- Input Handling ---
    private Vector2 touchStartPos;

    void HandleInput()
    {
        if (Touch.activeTouches.Count > 0)
        {
            var touch = Touch.activeTouches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                touchStartPos = touch.screenPosition;
            }

            Vector2 offset = touch.screenPosition - touchStartPos;

            // Normalize input to be frame-rate independent
            // You might want to clamp this so the turn doesn't get infinitely fast
            float normalizedTurn = Mathf.Clamp(offset.x / Screen.width, -1f, 1f);

            currentTurnInput = normalizedTurn; // Scale up for rotation speed
        }
        else
        {
            // Damping the input back to 0 feels better than snapping
            currentTurnInput = Mathf.Lerp(currentTurnInput, 0, Time.deltaTime * 5f);
        }
    }
}
