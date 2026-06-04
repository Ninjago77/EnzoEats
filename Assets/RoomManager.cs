using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    private Transform spawnPoint;
    private static bool hasConnectedOnce = false;
    public string roomCodestr;
    public string nicknamestr;
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
        // 1. Tell Photon to automatically sync scenes across all connected players
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        // 2. Subscribe to Unity's scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        // 3. Unsubscribe to clean up memory
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    //private void MapLoad()
    //{
    //    SceneManager.LoadScene("SchoolCafeteriaMap");
    //    spawnPoint = GameObject.FindWithTag("SpawnPointTag").transform;

    //}

    void Start()
    {
        //MapLoad();
        if (!hasConnectedOnce)
        {
            Debug.Log("P/Connecting for the first time...");
            PhotonNetwork.ConnectUsingSettings();
            //hasConnectedOnce = true; // Set to true so it never runs again
            FindAnyObjectByType<Camera>().enabled = false;
        }
        else
        {
            Debug.Log("P/Already connected previously. Skipping connection step.");

            // If you need to join a room automatically on re-entry, 
            // you can call OnJoinedLobby() or your specific room logic here.
        }


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

        if (!hasConnectedOnce)
        {
            FindAnyObjectByType<Camera>().enabled = true;
            hasConnectedOnce = true;
        }


        //RoomOptions roomOptions = new RoomOptions();
        //roomOptions.PublishUserId = true;

        //PhotonNetwork.JoinOrCreateRoom("test", roomOptions, null, null);

    }
    
    public void OnJoinClicked(string roomCodeInputField, string nicknameInputField)
    {
        roomCodestr = new string(roomCodeInputField.Where(char.IsLetterOrDigit).ToArray()).ToLower();
        nicknamestr = new string(nicknameInputField.Where(char.IsLetterOrDigit).ToArray());
        PhotonNetwork.NickName = nicknamestr;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;

        PhotonNetwork.JoinOrCreateRoom(roomCodestr, roomOptions, null, null);
    }

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();
    //    MapLoad();

    //    Debug.Log("P/Inside a Room");


    //    GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);

    //    //_player.GetComponent<PlayerSetup>().YesLocalPlayer();
    //}
    public override void OnJoinedRoom()
    {
        Debug.Log("P/Inside a Room");

        // 4. Use Photon's master client to load the scene safely for everyone
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("SchoolCafeteriaMap");
        }
    }

    // 5. This fires ONLY when the new scene is completely loaded and ready
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SchoolCafeteriaMap")
        {
            Transform spawnPoint = GameObject.FindWithTag("SpawnPointTag")?.transform;

            if (spawnPoint != null)
            {
                GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
                // _player.GetComponent<PlayerSetup>().YesLocalPlayer();
            }
            else
            {
                Debug.LogError("SpawnPointTag not found in the loaded scene!");
            }
        }
    }
}
