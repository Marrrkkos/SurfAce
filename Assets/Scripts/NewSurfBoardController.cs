using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class NewSurfBoardController : MonoBehaviour
{
    [Header("Setup")]
    public Transform[] floaters;
    public Rigidbody rigidBody;

    [Header("Auftrieb")]
    public float floatStrength = 15f;
    public float waterDrag = 3f;
    public float waterAngularDrag = 1f;

    [Header("Wellen-Dynamik (Das Neue)")]
    // Wie stark schiebt die Welle das Board horizontal weg?
    public float wavePushStrength = 20f;
    // Wie stark rutscht das Board den Wellenberg runter?
    public float slideFactor = 0.5f;
    // Hilfswert für die Normalenberechnung (je kleiner, desto präziser)
    private float sampleDist = 0.2f;
    void Update()
    {
        HandleInput();
    }
    void FixedUpdate()
    {
        applyPhysics();
    }
    void applyPhysics() {
        int floatersUnderwater = 0;
        // Wir sammeln den durchschnittlichen Normalenvektor des Wassers
        Vector3 combinedWaterNormal = Vector3.zero;

        foreach (Transform floater in floaters)
        {
            float waveHeight = RootWaveManager.instance.GetWaveHeight(floater.position);
            float diff = waveHeight - floater.position.y;

            if (diff > 0)
            {
                floatersUnderwater++;

                // 1. AUFTRIEB (Wie vorher)
                Vector3 buoyantForce = Vector3.up * diff * floatStrength;
                rigidBody.AddForceAtPosition(buoyantForce, floater.position, ForceMode.Acceleration);

                // 2. NORMALEN BERECHNEN (Der Trick)
                // Wir holen uns die Neigung der Welle an genau diesem Punkt
                Vector3 waterNormal = GetNormalAtPosition(floater.position);
                combinedWaterNormal += waterNormal;

                // 3. HORIZONTALER SCHUB (Push)
                // Wenn die Normale nach vorne zeigt, kommt die Welle von hinten -> Schub!
                // Wir projizieren die Normale auf die flache Ebene (x,z)
                Vector3 pushDirection = new Vector3(waterNormal.x, 0, waterNormal.z).normalized;

                // Wir wenden Kraft nur an, wenn die Welle steil genug ist
                rigidBody.AddForceAtPosition(pushDirection * wavePushStrength * diff, floater.position, ForceMode.Acceleration);
            }
        }

        // 4. DRAG & SLIDING (Rutschen)
        if (floatersUnderwater > 0)
        {
            rigidBody.linearDamping = waterDrag;
            rigidBody.angularDamping = waterAngularDrag;

            // Durchschnittliche Neigung des Wassers unter dem Board
            Vector3 avgNormal = (combinedWaterNormal / floatersUnderwater).normalized;

            // Optional: Extra "Rutschen" simulieren (Hangabtriebskraft verstärken)
            // Wir addieren Kraft in die Richtung, in die das Wasser abfällt
            Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, avgNormal).normalized;
            rigidBody.AddForce(slideDir * Physics.gravity.magnitude * slideFactor, ForceMode.Acceleration);
        }
        else
        {
            rigidBody.linearDamping = 0.05f;
            rigidBody.angularDamping = 0.05f;
        }
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
    public float inputMultiplier = 1f;
    void HandleInput()
    {

       
        if (Touch.activeTouches.Count >= 1)
        {
            if (Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                startPos = Touch.activeTouches[0].screenPosition;
            }

            Vector2 abstand = startPos - Touch.activeTouches[0].screenPosition;
            Debug.Log("x: " + abstand.x + " y: " + abstand.y);
            
            if (abstand.y > 0) 
            {
                rigidBody.AddForceAtPosition(abstand.y * Vector3.down * inputMultiplier * 0.01f, floaters[0].position, ForceMode.Acceleration);
            } 
            else 
            {
                rigidBody.AddForceAtPosition(-abstand.y * Vector3.down * inputMultiplier * 0.01f, floaters[1].position, ForceMode.Acceleration);
            }
            if (abstand.x > 0)
            {
                rigidBody.AddForceAtPosition(abstand.x * Vector3.down * inputMultiplier * 0.01f, floaters[2].position, ForceMode.Acceleration);
            }
            else
            {
                rigidBody.AddForceAtPosition(-abstand.x * Vector3.down * inputMultiplier * 0.01f, floaters[3].position, ForceMode.Acceleration);
            }

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
