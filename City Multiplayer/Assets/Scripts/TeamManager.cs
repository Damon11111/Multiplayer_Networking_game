using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TeamManager : NetworkBehaviour
{
    public enum Teams {Courier, Robber}
    public static TeamManager Instance {get; private set;}
    private int courierCount = 0;
    private int robberCount = 0;
    [SerializeField] private TMP_Text courierCountText;
    [SerializeField] private TMP_Text robberCountText;

    private void Awake()
    {   //Only single instance of team manager
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public Teams setTeam()
    {   //Assign the teams evenly and returns counting each and updating display
        if (courierCount <= robberCount)
        {
            courierCount++;
            updateText();
            return Teams.Courier;
        }
        else
        {
            robberCount++;
            updateText();
            return Teams.Robber;
        }
    }

    private void updateText()
    {
        // Update display with current player in team
        courierCountText.text = "Couriers: " + courierCount.ToString();
        robberCountText.text = "Robbers: " + robberCount.ToString();
    }
}
