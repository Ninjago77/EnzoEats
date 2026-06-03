using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManagerUIConnector : MonoBehaviour
{
    public TMP_InputField roomCodeInputField;
    public TMP_InputField nicknameInputField;
    public Button joinButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (RoomManager.Instance != null)
        {
            // 2. Set up the button click via code
            joinButton.onClick.AddListener(OnJoinClicked);
        }
        else
        {
            Debug.LogError("RoomManager instance not found in the scene!");
        }
    }

    private void OnJoinClicked()
    {
        // 3. Pass the UI data to the persistent manager
 
        RoomManager.Instance.OnJoinClicked(roomCodeInputField.text,nicknameInputField.text);
    }

    private void OnDestroy()
    {
        // Good practice: clean up listeners when the scene changes/unloads
        if (joinButton != null)
        {
            joinButton.onClick.RemoveListener(OnJoinClicked);
        }
    }
}
