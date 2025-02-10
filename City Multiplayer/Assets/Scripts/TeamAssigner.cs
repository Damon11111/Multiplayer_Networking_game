using UnityEngine;
using Unity.Netcode;

public class TeamAssigner : NetworkBehaviour
{
    private NetworkVariable<TeamManager.Teams> playerTeam = new NetworkVariable<TeamManager.Teams>(writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {   // Server runs team assignments
        if (IsServer)
            playerTeam.Value = TeamManager.Instance.setTeam();
    }

    public TeamManager.Teams getTeam()
    {   //Returns team so player can use it for abilities, color, spawn, etc
        return playerTeam.Value;
    }
}
