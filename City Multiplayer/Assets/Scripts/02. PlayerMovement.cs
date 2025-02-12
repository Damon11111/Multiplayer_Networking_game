using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// adding namespaces
using Unity.Netcode;
using System;
using System.Runtime.Serialization.Json;
using TMPro;

// because we are using the NetworkBehaviour class
// NewtorkBehaviour class is a part of the Unity.Netcode namespace
// extension of MonoBehaviour that has functions related to multiplayer
public class PlayerMovement : NetworkBehaviour
{
    public GameObject courierSpawn;
    public GameObject robberSpawn;

    // getting the reference to the prefab
    [SerializeField]
    private GameObject spawnedPrefab;
    // save the instantiated prefab
    private GameObject instantiatedPrefab;

    public GameObject cannon;
    public GameObject bullet;

    // reference to the camera audio listener
    [SerializeField] public AudioListener audioListener;
    // reference to the camera
    [SerializeField] public Camera playerCamera;

    private string playerTeam;
    private Transform spawn;
    private Renderer playerRenderer;

    //TeamManager.Instance.deliverServerRpc(); - Delivers packages updates counter
    //TeamManager.Instance.stolenServerRpc(); - Delivers packages updates counter

    // Add this near your other private variables
    private bool gameEnded = false;

    // Add this with the other private fields at the top
    private MailboxManager mailboxManager;

    // Start is called before the first frame update
    void Start()
    {
        TeamManager.Instance.setTeamServerRpc();
        if (IsOwner)
        {
            mailboxManager = GameObject.FindObjectOfType<MailboxManager>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        // check if the player is the owner of the object
        // makes sure the script is only executed on the owners 
        // not on the other prefabs 
        if (!IsOwner) return;

        // if I is pressed spawn the object 
        // if J is pressed destroy the object
        if (Input.GetKeyDown(KeyCode.I))
        {
            //instantiate the object
            instantiatedPrefab = Instantiate(spawnedPrefab);
            // spawn it on the scene
            instantiatedPrefab.GetComponent<NetworkObject>().Spawn(true);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            //despawn the object
            instantiatedPrefab.GetComponent<NetworkObject>().Despawn(true);
            // destroy the object
            Destroy(instantiatedPrefab);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            // call the BulletSpawningServerRpc method
            // as client can not spawn objects
            BulletSpawningServerRpc(cannon.transform.position, cannon.transform.rotation);
        }

        // Add delivery check for E key
        if (Input.GetKeyDown(KeyCode.E) && playerTeam == "Courier")
        {
            Debug.Log("E key pressed - attempting delivery");
            if (mailboxManager != null)
            {
                if (mailboxManager.TryDeliverPackage(transform.position))
                {
                    Debug.Log("Delivery successful - returning to spawn");
                    setSpawnServerRpc();
                }
            }
            else
            {
                Debug.LogError("MailboxManager not found!");
                mailboxManager = GameObject.FindObjectOfType<MailboxManager>();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        playerRenderer = GetComponent<Renderer>();

        bool whichTeam = OwnerClientId % 2 == 0 ? true : false; 

        if (whichTeam){
            if (playerRenderer != null)
                playerRenderer.material.color = Color.blue;
            spawn = courierSpawn.transform;
            playerTeam = "Courier";
            Debug.Log(playerTeam + " Joined The Game");
        }
        else{
            if (playerRenderer != null)
                playerRenderer.material.color = Color.red;
            spawn = robberSpawn.transform;
            playerTeam = "Robber";
            Debug.Log(playerTeam + " Joined The Game");
        }


        if (IsServer){
            setSpawnServerRpc();
        }

        if (IsOwner){
            audioListener.enabled = true;
            playerCamera.enabled = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void setColorServerRpc(Color color)
    {
        playerRenderer.material.color = color;
        setColorsClientRpc(color);
    }

    [ClientRpc]
    public void setColorsClientRpc(Color color)
    {
        playerRenderer.material.color = color;
    }



    [ServerRpc(RequireOwnership = false)]
    public void setSpawnServerRpc()
    {
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
        setSpawnClientRpc();
    }
    [ClientRpc]
    public void setSpawnClientRpc()
    {
        if (!IsOwner){
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }
    }


    // need to add the [ServerRPC] attribute
    [ServerRpc]
    // method name must end with ServerRPC
    private void BulletSpawningServerRpc(Vector3 position, Quaternion rotation)
    {
        // call the BulletSpawningClientRpc method to locally create the bullet on all clients
        BulletSpawningClientRpc(position, rotation);
    }

    [ClientRpc]
    private void BulletSpawningClientRpc(Vector3 position, Quaternion rotation)
    {
        GameObject newBullet = Instantiate(bullet, position, rotation);
        newBullet.GetComponent<Rigidbody>().linearVelocity += Vector3.up * 2;
        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * 1500);
        // newBullet.GetComponent<NetworkObject>().Spawn(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer || gameEnded) return;

        // Check if this is a player-player collision
        PlayerMovement otherPlayer = collision.gameObject.GetComponent<PlayerMovement>();
        if (otherPlayer == null) return;

        // Check if it's a Robber catching a Courier
        if (playerTeam == "Robber" && otherPlayer.playerTeam == "Courier")
        {
            EndGameServerRpc(false); // Robber wins
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndGameServerRpc(bool courierWon)
    {
        gameEnded = true;
        EndGameClientRpc(courierWon);
    }

    [ClientRpc]
    private void EndGameClientRpc(bool courierWon)
    {
        string winMessage = courierWon ? "Courier wins!" : "Robber wins!";
        Debug.Log($"Game Over! {winMessage}");
        
        // Disable player movement when game ends
        enabled = false;
    }
}