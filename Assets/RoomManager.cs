using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    private Transform spawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static RoomManager Instance { get; private set; }
    private void Awake()
    {
        // Singleton pattern to prevent duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy the duplicate
            return;
        }

        Instance = this;

        // Keeps this GameObject between scene loads
        DontDestroyOnLoad(gameObject);
    }

    private void MapLoad()
    {
        spawnPoint = GameObject.FindWithTag("SpawnPointTag").transform;

    }

    void Start()
    {
        MapLoad();
        Debug.Log("P/Connecting...");

        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("P/Connected To Server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("P/Joined Lobby");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;

        PhotonNetwork.JoinOrCreateRoom("test", roomOptions, null, null);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("P/Inside a Room");

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);

        //_player.GetComponent<PlayerSetup>().YesLocalPlayer();
    }
}
