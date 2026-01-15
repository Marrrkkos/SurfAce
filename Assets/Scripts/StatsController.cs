using UnityEngine;
using TMPro;
public class StatsController : MonoBehaviour
{
    public NewSurfBoardController newSurfBoardController;

    public TextMeshProUGUI[] texts;
    void LateUpdate()
    {
        texts[0].text = newSurfBoardController.currentSpeed.ToString();
        texts[1].text = newSurfBoardController.currentVelocity.ToString();
    }

}
