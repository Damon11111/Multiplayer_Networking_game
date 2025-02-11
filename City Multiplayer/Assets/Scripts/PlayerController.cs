using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"PlayerController spawned. IsHost: {IsHost}");
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
        if (!IsHost)
        {
            return;
        }

        // Check for delivery input
        if (Input.GetKeyDown(KeyCode.E)){
            Debug.Log("E key pressed in PlayerController");
            Vector3 currentPos = transform.position;
            Vector3 mailboxPos = MailboxManager.Instance.GetCurrentMailboxPosition();
            Debug.Log($"Player position: {currentPos}");
            Debug.Log($"Current mailbox position: {mailboxPos}");
                
            if (MailboxManager.Instance.TryDeliverPackage(currentPos)){
                Debug.Log("Delivery successful!");
            }
         }
     }
} 