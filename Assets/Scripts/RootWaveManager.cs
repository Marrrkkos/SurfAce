using UnityEngine;

public class RootWaveManager : MonoBehaviour
{
    public static RootWaveManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    public Vector3 GetWaveDisplacement(Vector3 basePosition)
    {
         //Vector3 singleWavesPos = SingleWavesManager.instance.GetWaveDisplacement(basePosition);
         Vector3 ambientWavesPos = AmbientWavesManager.instance.GetWaveDisplacement(basePosition);

        
        return ambientWavesPos;
    }
    public float GetWaveHeight(Vector3 basePosition)
    {
        return AmbientWavesManager.instance.GetWaveHeight(basePosition);
    }
}
