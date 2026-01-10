using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public void Awake() {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }
    }
    public float oceanSize;

}
