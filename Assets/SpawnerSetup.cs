using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class SpawnerSetup : MonoBehaviour
{


    public string[] prefabList;
    public float timing = 7f;
    private float timer;
    private int index;
    private bool isbeingTriggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        index = UnityEngine.Random.Range(0,prefabList.Length);

        timer = timing;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (other.gameObject.CompareTag("Player"))
        {
            isbeingTriggered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        timer -= Time.deltaTime;

        if (timer < 0 && isbeingTriggered)
        {
            PhotonNetwork.Instantiate(prefabList[index], new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Random.rotation);
            timer = timing;
            isbeingTriggered = false;

        }

    }
}
