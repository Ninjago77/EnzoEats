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
        // Wait until the client is fully in the room AND Photon has unpaused the network queue.
        while (!PhotonNetwork.InRoom || !PhotonNetwork.IsMessageQueueRunning)
        {
            yield return null;
        }

        GameObject[] possibleSpawns = GameObject.FindGameObjectsWithTag("SpawnPointTag");

        if (possibleSpawns.Length > 0)
        {
            // FIX: Seed the random number generator using the current system time ticks
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

            // Pick a random index now that the seed is randomized
            int randomIndex = UnityEngine.Random.Range(0, possibleSpawns.Length);
            Transform spawnPoint = possibleSpawns[randomIndex].transform;

            PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("SpawnPointTag not found in the loaded scene!");
        }
    }
}