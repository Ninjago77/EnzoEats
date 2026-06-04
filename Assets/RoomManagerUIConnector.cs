using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // Added so we can check if we are already connected

public class RoomManagerUIConnector : MonoBehaviour
{
    public TMP_InputField roomCodeInputField;
    public TMP_InputField nicknameInputField;
    public Button joinButton;

    //[Header("UI Panels")]
    public GameObject loadingPanel; // Drag your new LoadingPanel here in the inspector
    //public Image loadingPanelImage;

    void Start()
    {
        if (RoomManager.Instance != null)
        {
            joinButton.onClick.AddListener(OnJoinClicked);

            // Check if we are already connected to the lobby
            if (!PhotonNetwork.InLobby)
            {
                // Not connected yet: show loading screen and wait for the event
                ShowLoadingScreen();
                RoomManager.Instance.OnJoinedLobbyEvent += HideLoadingScreen;
            }
            else
            {
                // Already connected (e.g., returning to main menu from a game): hide it
                loadingPanel.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("RoomManager instance not found in the scene!");
        }
    }

    //private void HideLoadingScreen()
    //{
    //    loadingPanel.SetActive(false);
    //    //loadingPanelImage.enabled = false;
    //}

    //private void ShowLoadingScreen()
    //{
    //    loadingPanel.SetActive(true);
    //    //loadingPanelImage.enabled = true;
    //}

    private void HideLoadingScreen()
    {
        // THE FIX: Check if Unity has destroyed this object before trying to touch the UI
        if (this == null || loadingPanel == null) return;

        loadingPanel.SetActive(false);
    }

    private void ShowLoadingScreen()
    {
        // THE FIX: Check if Unity has destroyed this object before trying to touch the UI
        if (this == null || loadingPanel == null) return;

        loadingPanel.SetActive(true);
    }


    private void OnJoinClicked()
    {
        RoomManager.Instance.OnJoinClicked(roomCodeInputField.text, nicknameInputField.text);
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (joinButton != null)
        {
            joinButton.onClick.RemoveListener(OnJoinClicked);
        }

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.OnJoinedLobbyEvent -= HideLoadingScreen;
        }
    }
}