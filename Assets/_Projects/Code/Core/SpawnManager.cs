using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Spawn Points")]
    public Transform dodgerSpawnPoint;
    public Transform shooterLeftSpawnPoint;
    public Transform shooterRightSpawnPoint;

    [Header("Zone Collider References")]
    public BoxCollider dodgerZoneCollider;
    public BoxCollider shooterLeftZoneCollider;
    public BoxCollider shooterRightZoneCollider;

    private GameObject currentLiveDodger;
    private GameObject currentLiveShooterLeft;
    private GameObject currentLiveShooterRight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Fully populates everything at match launch
        SpawnCurrentRoundPlayers();
    }

    // Add these methods to your SpawnManager class

    /// <summary>
    /// Returns the currently active left shooter GameObject
    /// </summary>
    public GameObject GetLeftShooter()
    {
        return currentLiveShooterLeft;
    }

    /// <summary>
    /// Returns the currently active right shooter GameObject
    /// </summary>
    public GameObject GetRightShooter()
    {
        return currentLiveShooterRight;
    }

    public void SpawnCurrentRoundPlayers()
    {
        ClearActivePlayers();

        if (GameManager.Instance == null) return;

        GameManager.TeamData shooterTeam = GameManager.Instance.GetTeamByRole(GameManager.TeamRole.Shooters);
        
        // 1. Initial Dodger Spawn
        RespawnOnlyDodger();

        // 2. Spawn Shooters (Only called at match start, not during elimination cycles)
        if (shooterTeam.playerPrefabs.Length >= 2)
        {
            GameObject shooterLeftPrefab = shooterTeam.playerPrefabs[0];
            GameObject shooterRightPrefab = shooterTeam.playerPrefabs[1];

            if (shooterLeftPrefab != null && shooterLeftSpawnPoint != null)
            {
                currentLiveShooterLeft = Instantiate(shooterLeftPrefab, shooterLeftSpawnPoint.position, shooterLeftSpawnPoint.rotation);
                ConfigureZoneRestrictor(currentLiveShooterLeft, shooterLeftZoneCollider);
                
                // Force input map binding for left shooter
                PlayerController leftShooterCtrl = currentLiveShooterLeft.GetComponent<PlayerController>();
                if (leftShooterCtrl != null) leftShooterCtrl.AssignRole(PlayerController.MovementRole.Shooter);
            }

            if (shooterRightPrefab != null && shooterRightSpawnPoint != null)
            {
                currentLiveShooterRight = Instantiate(shooterRightPrefab, shooterRightSpawnPoint.position, shooterRightSpawnPoint.rotation);
                ConfigureZoneRestrictor(currentLiveShooterRight, shooterRightZoneCollider);
                
                // Force input map binding for right shooter
                PlayerController rightShooterCtrl = currentLiveShooterRight.GetComponent<PlayerController>();
                if (rightShooterCtrl != null) rightShooterCtrl.AssignRole(PlayerController.MovementRole.Shooter);
            }
        }
    }

    // NEW ISOLATED RESPAWN: Destroys and replaces ONLY the Dodger object
    public void RespawnOnlyDodger()
    {
        ClearDodgerOnly();

        if (GameManager.Instance == null) return;
        GameManager.TeamData dodgerTeam = GameManager.Instance.GetTeamByRole(GameManager.TeamRole.Dodgers);

        int activeDodgerIdx = dodgerTeam.currentActivePlayerIndex;
        if (activeDodgerIdx < dodgerTeam.playerPrefabs.Length)
        {
            GameObject dodgerPrefab = dodgerTeam.playerPrefabs[activeDodgerIdx];
            if (dodgerPrefab != null && dodgerSpawnPoint != null)
            {
                currentLiveDodger = Instantiate(dodgerPrefab, dodgerSpawnPoint.position, dodgerSpawnPoint.rotation);
                ConfigureZoneRestrictor(currentLiveDodger, dodgerZoneCollider);
                
                // Force input map binding for the Dodger
                PlayerController dodgerController = currentLiveDodger.GetComponent<PlayerController>();
                if (dodgerController != null) dodgerController.AssignRole(PlayerController.MovementRole.Dodger);
            }
        }
    }

    public void ClearActivePlayers()
    {
        ClearDodgerOnly();
        if (currentLiveShooterLeft != null) Destroy(currentLiveShooterLeft);
        if (currentLiveShooterRight != null) Destroy(currentLiveShooterRight);
    }

    public void ClearDodgerOnly()
    {
        if (currentLiveDodger != null) Destroy(currentLiveDodger);
    }

    private void ConfigureZoneRestrictor(GameObject player, BoxCollider targetZone)
    {
        ZoneRestrictor restrictor = player.GetComponent<ZoneRestrictor>();
        if (restrictor != null && targetZone != null)
        {
            restrictor.assignedZone = targetZone;
        }
    }
}