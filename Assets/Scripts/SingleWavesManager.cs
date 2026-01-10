using UnityEngine;
using System.Collections.Generic;

public class SingleWavesManager : MonoBehaviour
{
    public static SingleWavesManager instance;

    [System.Serializable]
    public class SingleWave
    {
        public float wavelength = 20.0f;
        [Range(0f, 1f)]
        public float steepness = 0.5f;
        public Vector2 direction = new Vector2(1f, 0f);
        public float speed = 1f;
        public float heightScale = 1f; 
    }

    public List<SingleWave> waves = new List<SingleWave>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public Vector3 GetWaveDisplacement(Vector3 basePosition)
    {
        Vector3 totalOffset = Vector3.zero;

        foreach (var w in waves)
        {
            Vector2 d = w.direction.normalized;

            float k = 2 * Mathf.PI / w.wavelength;
            float a = w.steepness / k; 

            float f = k * (Vector2.Dot(d, new Vector2(basePosition.x, basePosition.z)) - w.speed * Time.time);

            totalOffset.x += d.x * (a * Mathf.Cos(f));
            totalOffset.z += d.y * (a * Mathf.Cos(f));
            totalOffset.y += w.heightScale * a * Mathf.Sin(f);
        }

        return totalOffset;
    }
    public float GetWaveHeight(Vector3 basePosition)
    {
        return GetWaveDisplacement(basePosition).y;
    }
}
