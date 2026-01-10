using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WaterShaderSync : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        _renderer.GetPropertyBlock(_propBlock);

         //--- AMBIENT WAVES ---
        if (AmbientWavesManager.instance != null)
        {
            for (int i = 0; i < 3; i++)
            {
                string suffix = ((char)('A' + i)).ToString();

                float wavelength = 0.0f;
                float steepness = 0.0f;
                Vector2 direction = Vector2.right; // Default direction

                if (i < AmbientWavesManager.instance.waves.Count)
                {
                    var w = AmbientWavesManager.instance.waves[i];
                    wavelength = w.wavelength;
                    steepness = w.steepness;
                    direction = w.direction;

                }
                Vector4 dataA = new Vector4(direction.x, direction.y, wavelength, steepness);

                _propBlock.SetVector("_Ambient_Wave" + suffix + "_DataA", dataA);
            }
        }

        // --- SINGLE WAVES ---
        if (SingleWavesManager.instance != null)
        {
            for (int i = 0; i < 8; i++)
            {
                string suffix = ((char)('A' + i)).ToString();

                
            }
        }
        
        _renderer.SetPropertyBlock(_propBlock);
    }
}