using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // Dein PlayerSurfer

    [Header("Position Einstellungen")]
    public float fixedHeight = 8.0f;  // Die feste Höhe der Kamera (Y)
    public float distance = 12.0f;     // Wie weit ist die Kamera HINTER dem Spieler?
    public float followSpeed = 5.0f;   // Wie schnell zieht sie nach?

    [Header("Rotation Einstellungen")]
    public float tiltAngle = 20.0f;    // Der feste Blickwinkel nach unten (X-Achse)

    void LateUpdate()
    {
        if (target == null) return;

        // --- 1. POSITION BERECHNEN ---
        // Wir holen uns die aktuelle Drehung des Spielers (nur um die Y-Achse)
        float currentYAngle = target.eulerAngles.y;

        // Wir berechnen den Punkt hinter dem Spieler basierend auf seiner Drehung
        // Quaternion.Euler(0, currentYAngle, 0) * Vector3.back erstellt einen Vektor, der "nach hinten" zeigt, relativ zum Spieler
        Vector3 offsetPosition = Quaternion.Euler(0, currentYAngle, 0) * Vector3.back * distance;

        // Die Zielposition ist Spieler-Position + der Abstand nach hinten
        Vector3 desiredPosition = target.position + offsetPosition;

        // WICHTIG: Wir überschreiben die Y-Höhe hart!
        // Egal ob der Surfer auf einer 20m Welle ist, die Kamera bleibt auf 'fixedHeight'
        desiredPosition.y = fixedHeight + target.position.y;

        // Weiche Bewegung
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);


        // --- 2. ROTATION BERECHNEN ---
        // Wir wollen NICHT LookAt benutzen, weil das wackelt, wenn der Spieler springt.
        // Wir setzen die Rotation hart:
        // X = tiltAngle (Fester Blick nach unten)
        // Y = currentYAngle (Dreht sich mit dem Spieler mit)
        // Z = 0 (Kein seitliches Kippen)
        transform.rotation = Quaternion.Euler(tiltAngle, currentYAngle, 0);
    }
}
