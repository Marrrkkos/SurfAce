using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SurfBoardController : MonoBehaviour
{
    public Transform oceanTransform;
    [Header("Brett-Einstellungen")]
    public float boardLength = 3.0f;
    public float boardWidth = 1.0f;

    public float speed;

    private float currentYawX;
    private float currentYawY;

    void Update() {
        //MoveForward();
        //ApplyRotationOnWater();
    }
    void MoveForward() {
        Vector3 forwardFlat = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

        transform.position += forwardFlat * speed * Time.deltaTime;
    }

    void ApplyRotationOnWater()
    {
        Vector3 basePos = new Vector3(50f, 0f, 50f);
        Vector3 displacement = RootWaveManager.instance.GetWaveDisplacement(basePos);
        transform.position = basePos + displacement;

        transform.position = new Vector3(50f, RootWaveManager.instance.GetWaveHeight(basePos),50f);

        /*Vector3 oceanPos = oceanTransform.position;
        Vector3 oceanSize = new Vector3(GameManager.instance.oceanSize, 0, GameManager.instance.oceanSize);

        Quaternion yawRot = Quaternion.Euler(currentYawY, currentYawX, 0);
        float halfLen = boardLength / 2.0f;
        float halfWid = boardWidth / 2.0f;

        Vector3 pNose = transform.position + yawRot * new Vector3(0, 0, halfLen);
        Vector3 pTailL = transform.position + yawRot * new Vector3(-halfWid, 0, -halfLen);
        Vector3 pTailR = transform.position + yawRot * new Vector3(halfWid, 0, -halfLen);


        pNose.y = RootWaveManager.instance.GetWaveHeight(pNose);
        pTailL.y = RootWaveManager.instance.GetWaveHeight(pTailL);
        pTailR.y = RootWaveManager.instance.GetWaveHeight(pTailR);

        Debug.DrawLine(pNose, pTailL, Color.red);
        Debug.DrawLine(pTailL, pTailR, Color.red);
        Debug.DrawLine(pTailR, pNose, Color.red);

        Debug.DrawRay(transform.position, transform.forward * 200, Color.green);

        Vector3 centerTail = (pTailL + pTailR) / 2f;

        Vector3 newForward = (pNose - centerTail).normalized;

        Vector3 newRight = (pTailR - pTailL).normalized;

        Vector3 newUp = Vector3.Cross(newForward, newRight);

        Quaternion targetRotation = Quaternion.LookRotation(newForward, newUp);

        float smoothSpeed = 10f;
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        transform.rotation = targetRotation;
        float avgHeight = (pNose.y + pTailL.y + pTailR.y) / 3f;

        transform.position = new Vector3(transform.position.x, avgHeight, transform.position.z);*/
    }

    private Vector2 startPos = Vector2.zero;
    public float rotationSpeed = 1f;
    void HandleInput() {
        if (Touch.activeTouches.Count > 0) {
            Debug.Log("tesdt2");
            if (Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                startPos = Touch.activeTouches[0].screenPosition;
            }

            Vector2 abstand = startPos - Touch.activeTouches[0].screenPosition;
            float rotationAmountX = abstand.x * rotationSpeed * Time.deltaTime;
            float rotationAmountY = abstand.y * rotationSpeed * Time.deltaTime;
            currentYawX -= rotationAmountX;
            currentYawY -= rotationAmountY;
            Debug.Log(abstand.x + " y: " + abstand.y);
        }
    }
    
}
