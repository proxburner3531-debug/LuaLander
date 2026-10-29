using TMPro;
using UnityEngine;

public class LandingPadVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreMultiplierTextMesh;

    private LandingPad landingPad;
    private void Awake()
    {
        landingPad = GetComponent<LandingPad>();
        scoreMultiplierTextMesh.text = "x" + landingPad.GetScoreMultiplier();
    }
}
