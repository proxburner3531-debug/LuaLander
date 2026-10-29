using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI StatsUITextMesh;
    [SerializeField] private GameObject SpeedUpArrowGameObject;
    [SerializeField] private GameObject SpeedDownArrowGameObject;
    [SerializeField] private GameObject SpeedLeftArrowGameObject;
    [SerializeField] private GameObject SpeedRightArrowGameObject;
    [SerializeField] private Image FuelImage;


    private void Update()
    {
        UpdateStatsTextMesh();
    }

    private void UpdateStatsTextMesh()
    {
        SpeedUpArrowGameObject.SetActive(Lander.Instance.GetSpeedY() >= 0);
        SpeedDownArrowGameObject.SetActive(Lander.Instance.GetSpeedY() < 0);
        SpeedLeftArrowGameObject.SetActive(Lander.Instance.GetSpeedX() < 0);
        SpeedRightArrowGameObject.SetActive(Lander.Instance.GetSpeedX() >= 0);

        FuelImage.fillAmount = Lander.Instance.GetFuelNormalized();

        StatsUITextMesh.text =
            GameManager.Instance.GetLevelNumber() + "\n" +
            GameManager.Instance.GetScore() + "\n"
            + Mathf.Round(GameManager.Instance.GetTime()) + "\n"
            + Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedX() * 10f)) + "\n"
            + Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedY() * 10f)) + "\n";
    }
}
