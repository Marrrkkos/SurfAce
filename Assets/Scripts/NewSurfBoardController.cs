using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class NewSurfBoardController : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform oceanTF;

    [Header("General")]
    public float rotationStrength = 1f;
    public float lerpSpeed = 1f;

    [Header("Physics")]
    public float velocityStrength = 1f;
    public float startSpeed = 10f;


    public Vector3 currentVelocity;
    public float currentSpeed;
    private bool gameOver = false;

    private float currentTurnInput;

    void OnEnable() => EnhancedTouchSupport.Enable();
    void OnDisable() => EnhancedTouchSupport.Disable();

    void Start()
    {
        currentSpeed = startSpeed;
    }

    void Update()
    {
        if (gameOver) return;
        HandleInput();
    }

    void FixedUpdate()
    {
        if (gameOver) return;

        Vector3 waterSlopeNormal = GetWaterSlopeNormal(transform.position, 0.5f);
        float waveHeight = RootWaveManager.instance.GetWaveHeight(transform.position);
        float currentHeight = transform.position.y;
        float depth = waveHeight - currentHeight;



        HandlePosition(waveHeight, depth, waterSlopeNormal);
        HandleRotation(waterSlopeNormal, depth > 0);
    }
    private void HandlePosition(float waveHeight, float depth, Vector3 surfaceNormal)
    {
        Vector3 slope = Vector3.ProjectOnPlane(Vector3.down, surfaceNormal).normalized;
        currentVelocity += slope * velocityStrength * 0.1f;
        currentSpeed = (currentSpeed + currentSpeed * Vector3.Dot(slope, transform.forward) * 0.01f);


        Vector3 nextPosition = transform.position + ((transform.forward * currentSpeed) + currentVelocity) * 0.02f; // 0.02 FixedUpdate
        Vector3 finalPosition = new Vector3(nextPosition.x, waveHeight + 0.1f, nextPosition.z);
        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * lerpSpeed);
    }
    
    private void HandleRotation(Vector3 surfaceNormal, bool isSurfing)
    {
            Vector3 targetUp = surfaceNormal;
            Vector3 targetForward = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(targetForward, targetUp);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.fixedDeltaTime);

            transform.Rotate(Vector3.up, currentTurnInput * rotationStrength, Space.Self);

    }

    private Vector3 GetWaterSlopeNormal(Vector3 pos, float sampleDist)
    {
        float h0 = RootWaveManager.instance.GetWaveHeight(pos);
        float hX = RootWaveManager.instance.GetWaveHeight(pos + new Vector3(sampleDist, 0, 0));
        float hZ = RootWaveManager.instance.GetWaveHeight(pos + new Vector3(0, 0, sampleDist));

        Vector3 v1 = new Vector3(sampleDist, hX - h0, 0);
        Vector3 v2 = new Vector3(0, hZ - h0, sampleDist);

        return Vector3.Cross(v2, v1).normalized;
    }

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

            float normalizedTurn = Mathf.Clamp(offset.x / Screen.width, -1f, 1f);

            currentTurnInput = normalizedTurn;
        }
        else
        {
            currentTurnInput = Mathf.Lerp(currentTurnInput, 0, Time.deltaTime * 5f);
        }
    }
    
}
