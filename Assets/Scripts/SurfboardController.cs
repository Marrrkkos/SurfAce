using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SurfBoardController : MonoBehaviour
{
    public Transform oceanTF;
    [Header("general")]
    public float sampleDist = 0.2f;
    public float startSpeed = 10f;

    [Header("worldSettings")]
    private float gravitiy = 10f;
    public float waterForce = 2f;
    public float waterBreakPoint = 10f;
    public float velocityMultilpier = 0.2f;

    private Vector3 currentVelocity;
    void Start() {
        currentVelocity = startSpeed * transform.forward;
    }
    void Update()
    {
        // Vector3 currentSpeed;
        // float startSpeed;

        // Calculate Estimated Speed Position, (Positiona fter no resisistance at all)
        // if under Water and speed > waterBreakPoint-> Go under Water, else set Transform at waterheight
        // If not under Water ->set Transform to Estimated Speed position
        // Apply Gravitiy
        // Apply Rotation


        Vector3 waterSlope = GetWaterSlope(transform.position);
        calculateVelocity(waterSlope);
        if (RootWaveManager.instance.GetWaveHeight(transform.position) < transform.position.y)
        {
            doAboveWater();
        }
        else {
            doUnderWater();
        }
        applyRotation(waterSlope);




    }
    void calculateVelocity(Vector3 waterSlope) {
        Vector3 downhill = Vector3.ProjectOnPlane(Vector3.down, waterSlope).normalized;
        Vector3 gravityVec = new Vector3(0, gravitiy, 0);

        transform.position += currentVelocity.normalized * velocityMultilpier * 0.01f;
        currentVelocity += ((downhill - gravityVec) * velocityMultilpier * 0.01f);
        Debug.Log("x: " + currentVelocity.x + " y: " + currentVelocity.y + " z: " + currentVelocity.z);
    }
    private void doUnderWater() {
        if (-currentVelocity.y > waterBreakPoint) {
            Debug.Log("GAME END -> You got under Water!!");
        }
        else {
            float y = RootWaveManager.instance.GetWaveHeight(transform.position);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
    private void doAboveWater() { 

    }
    private void applyRotation(Vector3 waterSlope) { 
    }

    private Vector3 GetWaterSlope(Vector3 pos)
    {
        float h0 = RootWaveManager.instance.GetWaveHeight(pos);
        float hX = RootWaveManager.instance.GetWaveHeight(pos + new Vector3(sampleDist, 0, 0));
        float hZ = RootWaveManager.instance.GetWaveHeight(pos + new Vector3(0, 0, sampleDist));
        Vector3 v1 = new Vector3(sampleDist, hX - h0, 0);
        Vector3 v2 = new Vector3(0, hZ - h0, sampleDist);
        return Vector3.Cross(v2, v1).normalized;
    }
}
