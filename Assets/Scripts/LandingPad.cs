using UnityEngine;

public class LandingPad : MonoBehaviour
{
    [SerializeField] private int ScoreMultiplier;

    public int GetScoreMultiplier()
    {
        return ScoreMultiplier;
    }
  
}
