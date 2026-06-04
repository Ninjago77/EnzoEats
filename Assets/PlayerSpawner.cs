using System.Collections; // Needed for Coroutines
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        // Start a Coroutine to wait until the network is fully unpaused
        StartCoroutine(SpawnPlayerCoroutine());
    }

    private IEnumerator SpawnPlayerCoroutine()
    {
        // THE FIX: Wait until the client is fully in the room AND Photon has unpaused the network queue.
        // If we spawn while the queue is paused, other players will never receive the spawn message!
        while (!PhotonNetwork.InRoom || !PhotonNetwork.IsMessageQueueRunning)
        {
            yield return null; // Wait for the next frame and check again
        }

        // Now that the network is fully unpaused, we can safely spawn!
        GameObject[] possibleSpawns = GameObject.FindGameObjectsWithTag("SpawnPointTag");
        Transform spawnPoint = possibleSpawns[UnityEngine.Random.Range(0, possibleSpawns.Length)]?.transform;

        if (spawnPoint != null)
        {
            GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("SpawnPointTag not found in the loaded scene!");
        }
    }
}