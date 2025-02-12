using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class MailboxManager : NetworkBehaviour
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float deliveryRange = 2f;
    
    private List<GameObject> mailboxes = new List<GameObject>();
    private List<GameObject> unusedMailboxes = new List<GameObject>();
    private NetworkVariable<int> currentMailboxIndex = new NetworkVariable<int>(-1);
    private Material defaultMaterial;
    private MeshRenderer currentHighlightedMailbox;

    [SerializeField] private GameObject selectionIndicatorPrefab;
    private GameObject currentIndicator;

    [SerializeField] private TMPro.TextMeshProUGUI packagesRemainingText;
    private bool hasInitialized = false;
    private const int TOTAL_MAILBOXES = 13;


    public static MailboxManager Instance { get; private set; }

    private void Awake()
    {   //Only single instance of mailbox manager
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost && !hasInitialized)
        {
            GameObject[] foundMailboxes = GameObject.FindGameObjectsWithTag("Mailbox");
            Debug.Log($"Found {foundMailboxes.Length} mailboxes");
            mailboxes.AddRange(foundMailboxes.Take(TOTAL_MAILBOXES));
            ResetUnusedMailboxes();
            SelectNewRandomMailboxServerRpc();
            hasInitialized = true;
        }
    }


    [ServerRpc]
    private void SelectNewRandomMailboxServerRpc()
    {
        if (unusedMailboxes.Count == 0)
        {
            Debug.Log("All mailboxes used, resetting pool");
            ResetUnusedMailboxes();
        }

        if (unusedMailboxes.Count == 0)
        {
            Debug.Log("All mailboxes used, resetting pool");
            ResetUnusedMailboxes();
        }

        int randomIndex = Random.Range(0, unusedMailboxes.Count);
        GameObject selectedMailbox = unusedMailboxes[randomIndex];
        int newIndex = mailboxes.IndexOf(selectedMailbox);
        
        unusedMailboxes.RemoveAt(randomIndex);
        
        Debug.Log($"Selected mailbox index: {newIndex}. {unusedMailboxes.Count} mailboxes remaining");
        currentMailboxIndex.Value = newIndex;
        UpdateMailboxHighlightClientRpc(newIndex);
    }

    [ClientRpc]
    private void UpdateMailboxHighlightClientRpc(int newIndex)
    {
        if (currentHighlightedMailbox != null && defaultMaterial != null){
            currentHighlightedMailbox.material = defaultMaterial;
        }

        if (currentIndicator != null){
            Destroy(currentIndicator);
        }

        if (newIndex >= 0 && newIndex < mailboxes.Count){
            Debug.Log($"Selected Mailbox Index: {newIndex}");
            currentHighlightedMailbox = mailboxes[newIndex].GetComponent<MeshRenderer>();
            if (currentHighlightedMailbox != null)
            {
                defaultMaterial = currentHighlightedMailbox.material;
                currentHighlightedMailbox.material = highlightMaterial;
            }

            Vector3 mailboxPosition = mailboxes[newIndex].transform.position;
            mailboxPosition.y += 15f;
            currentIndicator = Instantiate(selectionIndicatorPrefab, mailboxPosition, Quaternion.identity);

            // Add light component
            Light indicatorLight = currentIndicator.AddComponent<Light>();
            indicatorLight.type = LightType.Point;
            indicatorLight.intensity = 5f;
            indicatorLight.range = 10f;
            indicatorLight.color = Color.yellow;

            // Get renderer and set emission
            Renderer renderer = currentIndicator.GetComponent<Renderer>();
            Material material = renderer.material;
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", Color.yellow * 2f);
        }
    }

    public Vector3 GetCurrentMailboxPosition()
    {
        if (currentMailboxIndex.Value >= 0 && currentMailboxIndex.Value < mailboxes.Count)
        {
            return mailboxes[currentMailboxIndex.Value].transform.position;
        }
        Debug.LogWarning($"Invalid mailbox index: {currentMailboxIndex.Value}, Count: {mailboxes.Count}");
        return Vector3.zero;
    }

    public bool TryDeliverPackage(Vector3 playerPosition)
    {
        if (!IsHost)
        {
            Debug.Log("Delivery attempted but player is not host");
            return false;
        }

        if (currentMailboxIndex.Value < 0 || currentMailboxIndex.Value >= mailboxes.Count)
        {
            Debug.LogError($"Invalid mailbox index: {currentMailboxIndex.Value}");
            return false;
        }

        Vector3 mailboxPos = mailboxes[currentMailboxIndex.Value].transform.position;
        float distance = Vector3.Distance(playerPosition, mailboxPos);

        if (distance <= deliveryRange)
        {
            // First decrement the count
            TeamManager.Instance.deliverServerRpc();
            
            // If game isn't over, continue with mailbox selection
            if (TeamManager.Instance.totalPackages.Value > 0)
            {
                int remaining = unusedMailboxes.Count - 1;
                Debug.Log($"Remaining mailboxes: {remaining}");
                
                if (remaining <= 0)
                {
                    Debug.Log("Resetting mailbox pool");
                    ResetUnusedMailboxes();
                }
                SelectNewRandomMailboxServerRpc();
            }
            return true;
        }
        
        return false;
    }

    private void ResetUnusedMailboxes()
    {
        unusedMailboxes.Clear();
        unusedMailboxes.AddRange(mailboxes);
        Debug.Log($"Reset mailbox pool. Now have {unusedMailboxes.Count} mailboxes");
    }
}
