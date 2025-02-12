using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private MailboxManager mailboxManager;
    
    public override void OnNetworkSpawn()
    {
        mailboxManager = FindObjectOfType<MailboxManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsSpawned) return;

        PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
        
        if (otherPlayer != null)
        {
            if (IsHost && !otherPlayer.IsHost)
            {
                // Client caught the host
                GameManager.Instance.ClientWinServerRpc();
            }
        }
    }

    private void Update()
    {
        if (!IsHost) return;

        // Check for delivery input (e.g., E key)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (mailboxManager.TryDeliverPackage(transform.position))
            {
                // Package delivered successfully
                // Add visual/audio feedback here
            }
        }
    }
} 