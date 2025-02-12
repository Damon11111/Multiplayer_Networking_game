using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class TeamManager : NetworkBehaviour
{
    public enum Teams {Courier, Robber}
    public static TeamManager Instance {get; private set;}
    public NetworkVariable<int> courierCount = new NetworkVariable<int>(0);
    public NetworkVariable<int> robberCount = new NetworkVariable<int>(0);
    public NetworkVariable<int> totalPackages = new NetworkVariable<int>(13);
    public NetworkVariable<int> whoWon = new NetworkVariable<int>(0);
    [SerializeField] private int setPackages;

    [SerializeField] private TMP_Text courierText;
    [SerializeField] private TMP_Text robberText;
    [SerializeField] private TMP_Text remainingText;
    [SerializeField] private TMP_Text winnerText;

    private void Awake()
    {   //Only single instance of team manager
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        totalPackages.Value = setPackages;
        setPackagesServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void setPackagesServerRpc()
    {
        totalPackages.Value = setPackages;
    }

    [ServerRpc (RequireOwnership = false)]
    public void deliverServerRpc(){
        totalPackages.Value--;
        if (totalPackages.Value <= 0){
            whoWon.Value = 1;
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void stolenServerRpc(){
        whoWon.Value = 2;
    }

    [ServerRpc (RequireOwnership = false)]
    public void setTeamServerRpc(){   //Assign the teams evenly and returns counting each and updating display
        if (courierCount.Value <= robberCount.Value){
            courierCount.Value++;
        }
        else{
            robberCount.Value++;
        }
    }


    void Update()
    {
        //Updates UI text appropiatly
        if (courierCount.Value > 0){
            courierText.text = "Couriers: Online";
        }
        else{
            courierText.text = "Couriers: Offline";
        }
        if (robberCount.Value > 0){
            robberText.text = "Robbers: Online";
        }else{
            robberText.text = "Robbers: Offline";
        }
        remainingText.text = "Packages Remaining: " + totalPackages.Value.ToString();

        if (whoWon.Value == 1){
            winnerText.color = Color.blue;
            winnerText.text = "Courier<br>Wins";
        }
        else if (whoWon.Value == 2){
            winnerText.color = Color.red;
            winnerText.text = "Robber<br>Wins";
        }
    }
}
