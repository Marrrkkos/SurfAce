using UnityEngine;
using System.Collections.Generic;

public class AmbientWavesManager : MonoBehaviour
{
    public static AmbientWavesManager instance;

    [System.Serializable]
    public class AmbientWave
    {
        public float wavelength = 20.0f;
        [Range(0f, 1f)]
        public float steepness = 0.5f;
        public Vector2 direction = new Vector2(1f, 0f);
    }

    public List<AmbientWave> waves = new List<AmbientWave>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    public Vector3 GetWaveDisplacement(Vector3 basePosition)
    {
        Vector3 totalOffset = Vector3.zero;

        foreach (var w in waves)
        {
            Vector2 d = w.direction.normalized;

            float k = 2 * Mathf.PI / w.wavelength;
            float c = Mathf.Sqrt(9.81f / k); // Physikalische Geschwindigkeit (Gravitation)
            float a = w.steepness / k;      // Amplitude

            float f = k * (Vector2.Dot(d, new Vector2(basePosition.x, basePosition.z)) - c * Time.time);

            totalOffset.x += d.x * (a * Mathf.Cos(f));
            totalOffset.z += d.y * (a * Mathf.Cos(f));
            totalOffset.y += a * Mathf.Sin(f);
        }

        return totalOffset;
    }
    public float GetWaveHeight(Vector3 worldPosition)
    {
        // Wir starten mit der Annahme, dass die Weltposition 
        // auch unsere Ursprungsposition (Undisplaced) ist.
        Vector3 guess = worldPosition;

        // 3 Iterationen reichen meistens völlig aus (Newton-ähnliche Annäherung)
        for (int i = 0; i < 3; i++)
        {
            Vector3 displacement = GetWaveDisplacement(guess);

            // Der Fehler ist die Differenz zwischen der gewünschten Welt-XZ 
            // und der XZ-Position, an der wir landen würden.
            float errorX = worldPosition.x - (guess.x + displacement.x);
            float errorZ = worldPosition.z - (guess.z + displacement.z);

            // Wir korrigieren unseren Schätzwert um diesen Fehler
            guess.x += errorX;
            guess.z += errorZ;
        }

        // Jetzt, wo wir den korrekten Ursprungspunkt 'guess' gefunden haben,
        // berechnen wir dessen finale vertikale Verschiebung.
        return GetWaveDisplacement(guess).y;
    }
}