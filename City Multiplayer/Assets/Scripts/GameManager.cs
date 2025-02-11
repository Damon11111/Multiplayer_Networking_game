using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI packagesRemainingText;
    [SerializeField] private TextMeshProUGUI winnerText;
    
    private NetworkVariable<int> score = new NetworkVariable<int>(0);
    private NetworkVariable<int> remainingPackages = new NetworkVariable<int>(13);
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        score.OnValueChanged += UpdateScoreUI;
        UpdateScoreUI(0, score.Value);
        remainingPackages.OnValueChanged += UpdatePackagesUI;
        UpdatePackagesUI(13, remainingPackages.Value);
    }

    private void UpdateScoreUI(int previousValue, int newValue)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Packages Delivered: {newValue}/13";
        }
    }

    private void UpdatePackagesUI(int previousValue, int newValue)
    {
        if (packagesRemainingText != null)
        {
            packagesRemainingText.text = $"Packages Remaining: {newValue}";
        }
    }

    [ServerRpc]
    public void HostWinServerRpc()
    {
        HostWinClientRpc();
    }

    [ClientRpc]
    private void HostWinClientRpc()
    {
        if (IsHost)
        {
            winnerText.text = "You Win!";
        }
        else
        {
            winnerText.text = "You Lose!";
        }
    }

    [ServerRpc]
    public void ClientWinServerRpc()
    {
        ClientWinClientRpc();
    }

    [ClientRpc]
    private void ClientWinClientRpc()
    {
        if (IsHost)
        {
            winnerText.text = "You Lose!";
        }
        else
        {
            winnerText.text = "You Win!";
        }
    }

    public void DecreasePackages()
    {
        if (IsHost)
        {
            remainingPackages.Value--;
            
            if (remainingPackages.Value <= 0)
            {
                HostWinServerRpc();
            }
        }
    }

    public int GetRemainingPackages()
    {
        return remainingPackages.Value;
    }
} 