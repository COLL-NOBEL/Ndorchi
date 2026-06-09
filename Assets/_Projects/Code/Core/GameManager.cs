using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class TeamData
    {
        public string teamName;
        [Tooltip("Assign references to the 5 player prefabs")]
        public GameObject[] playerPrefabs = new GameObject[5];
        
        [HideInInspector] public List<bool> playerAliveStatus = new List<bool>();
        [HideInInspector] public int currentActivePlayerIndex = 0;
    }

    [Header("Teams Configuration")]
    public TeamData teamA;
    public TeamData teamB;

    public enum TeamRole { Shooters, Dodgers }
    
    [Header("Current Round State")]
    public TeamRole teamARole = TeamRole.Shooters;
    public TeamRole teamBRole = TeamRole.Dodgers;

    [Header("Match Progression (Task 4.5 & 4.6)")]
    public int currentRoundNumber = 1;
    public const int MAX_ROUNDS = 2;

    [Header("Round Timer Settings (Task 4.5.1)")]
    public float roundDurationSeconds = 300f; 
    private float timeRemaining;
    private bool isRoundActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeTeams();
    }

    private void Start()
    {
        StartNewRoundTimer();
    }

    private void Update()
    {
        if (!isRoundActive) return;

        // Task 4.5.1: Process Round Countdowns
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
            isRoundActive = false;
            Debug.Log("⏰ TIME'S UP! The round duration has completely run out.");
            TriggerTeamSwap();
        }
    }

    private void InitializeTeams()
    {
        teamA.playerAliveStatus.Clear();
        for (int i = 0; i < teamA.playerPrefabs.Length; i++) teamA.playerAliveStatus.Add(true);
        teamA.currentActivePlayerIndex = 0;

        teamB.playerAliveStatus.Clear();
        for (int i = 0; i < teamB.playerPrefabs.Length; i++) teamB.playerAliveStatus.Add(true);
        teamB.currentActivePlayerIndex = 0;
    }

    private void StartNewRoundTimer()
    {
        timeRemaining = roundDurationSeconds;
        isRoundActive = true;
    }

    public TeamData GetTeamByRole(TeamRole role)
    {
        if (teamARole == role) return teamA;
        return teamB;
    }

    public void EliminateActiveDodger()
    {
        TeamData dodgerTeam = GetTeamByRole(TeamRole.Dodgers);
        int activeIdx = dodgerTeam.currentActivePlayerIndex;

        if (activeIdx < dodgerTeam.playerAliveStatus.Count)
        {
            dodgerTeam.playerAliveStatus[activeIdx] = false;
            Debug.Log($"💀 {dodgerTeam.teamName} Player {activeIdx + 1} has been eliminated!");
            dodgerTeam.currentActivePlayerIndex++;
        }

        // Check if all 5 players are gone out of play
        if (!HasRemainingDodgers())
        {
            Debug.Log("🎯 All Dodgers have been wiped out!");
            TriggerTeamSwap();
        }
        else
        {
            if (SpawnManager.Instance != null) SpawnManager.Instance.RespawnOnlyDodger();
        }
    }

    public bool HasRemainingDodgers()
    {
        TeamData dodgerTeam = GetTeamByRole(TeamRole.Dodgers);
        return dodgerTeam.currentActivePlayerIndex < dodgerTeam.playerPrefabs.Length;
    }

    // Task 4.5.2: Handle Field Resets and Inverting Team Positions/Scripts
    public void TriggerTeamSwap()
    {
        isRoundActive = false;

        if (currentRoundNumber >= MAX_ROUNDS)
        {
            DetermineMatchWinner();
            return;
        }

        Debug.Log("🔄 ROUND OVER: Inverting Roles...");

        teamARole = (teamARole == TeamRole.Shooters) ? TeamRole.Dodgers : TeamRole.Shooters;
        teamBRole = (teamBRole == TeamRole.Shooters) ? TeamRole.Dodgers : TeamRole.Shooters;

        teamA.currentActivePlayerIndex = 0;
        teamB.currentActivePlayerIndex = 0;

        for (int i = 0; i < teamA.playerAliveStatus.Count; i++) teamA.playerAliveStatus[i] = true;
        for (int i = 0; i < teamB.playerAliveStatus.Count; i++) teamB.playerAliveStatus[i] = true;

        currentRoundNumber++;

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.SpawnCurrentRoundPlayers();
        }

        ResetBallOnField();
        StartNewRoundTimer();
    }

    private void ResetBallOnField()
    {
        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball != null)
        {
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
                ballRb.isKinematic = false;
            }
            BallCollision bc = ball.GetComponent<BallCollision>();
            if (bc != null) bc.ballShotByShooter = false;

            ball.transform.position = new Vector3(0, 1.0f, 0); 
        }
    }

    // Task 4.6: Calculate scores at Match Conclusion
    private void DetermineMatchWinner()
    {
        Debug.Log("🏁 MATCH COMPLETE!");
        int scoreA = 0;
        int scoreB = 0;

        if (ScoreManager.Instance != null)
        {
            scoreA = ScoreManager.Instance.GetTeamAScore();
            scoreB = ScoreManager.Instance.GetTeamBScore();
        }

        Debug.Log($"📊 Final Scores -> {teamA.teamName}: {scoreA} | {teamB.teamName}: {scoreB}");

        if (scoreA > scoreB) Debug.Log($"🏆 WINNER: {teamA.teamName}!");
        else if (scoreB > scoreA) Debug.Log($"🏆 WINNER: {teamB.teamName}!");
        else Debug.Log("🤝 IT'S A DRAW!");

        Time.timeScale = 0f; 
    }
}