using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private MailboxManager mailboxManager;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"PlayerController spawned. IsHost: {IsHost}");
        FindMailboxManager();
    }

    private void FindMailboxManager()
    {
        mailboxManager = GameObject.FindObjectOfType<MailboxManager>();
        if (mailboxManager == null)
        {
            Debug.LogError("MailboxManager not found in scene!");
        }
        else
        {
            Debug.Log("MailboxManager reference set successfully");
        }
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed in PlayerController");
            if (mailboxManager != null)
            {
                Vector3 currentPos = transform.position;
                Vector3 mailboxPos = mailboxManager.GetCurrentMailboxPosition();
                Debug.Log($"Player position: {currentPos}");
                Debug.Log($"Current mailbox position: {mailboxPos}");
                
                if (mailboxManager.TryDeliverPackage(currentPos))
                {
                    Debug.Log("Delivery successful!");
                }
            }
            else
            {
                Debug.LogError("MailboxManager reference is null! Attempting to find it again...");
                FindMailboxManager();
            }
        }
    }
} 