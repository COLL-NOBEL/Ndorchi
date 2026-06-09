using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Scores")]
    public int teamAScore = 0;
    public int teamBScore = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // OPEN YOUR ScoreManager.cs AND ADD THESE TWO METHODS INSIDE THE CLASS CONTAINER:

    /// <summary>
    /// Public getter method to safely expose Team A's total cumulative score to the GameManager.
    /// </summary>
    public int GetTeamAScore()
    {
        // Replace 'teamAScore' with whatever your script calls Team A's score variable
        return teamAScore; 
    }

    /// <summary>
    /// Public getter method to safely expose Team B's total cumulative score to the GameManager.
    /// </summary>
    public int GetTeamBScore()
    {
        // Replace 'teamBScore' with whatever your script calls Team B's score variable
        return teamBScore; 
    }

    // Task 4.3 Implementation: Method to add points to whoever is playing the Dodger role right now
    public void AddScoreToActiveDodgers(int points)
    {
        if (GameManager.Instance == null) return;

        // Find out whether Team A or Team B is currently dodging
        if (GameManager.Instance.teamARole == GameManager.TeamRole.Dodgers)
        {
            teamAScore += points;
            Debug.Log($"🏆 Team A (Dodgers) gets +{points} pts! Total: {teamAScore}");
        }
        else
        {
            teamBScore += points;
            Debug.Log($"🏆 Team B (Dodgers) gets +{points} pts! Total: {teamBScore}");
        }
    }
}