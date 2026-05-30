using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject mainCamera;
    public MouseXPlay mouseXPlay;
    public MouseYCam mouseYCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public void YesLocalPlayer()
    //{
    //    playerMovement.enabled = true;
    //    mouseXPlay.enabled = true;
    //    mouseYCam.enabled = true;
    //    mainCamera.SetActive(true);
    //}

    void Start()
    {
        // 3. Check if this specific player object belongs to the person running the game
        if (GetComponent<PhotonView>().IsMine)
        {
            // This is YOU. Enable controls.
            playerMovement.enabled = true;
            mouseXPlay.enabled = true;
            mouseYCam.enabled = true;
            mainCamera.SetActive(true);
        }
        else
        {
            // This is a REMOTE PLAYER clone. Explicitly disable controls and cameras.
            playerMovement.enabled = false;
            mouseXPlay.enabled = false;
            mouseYCam.enabled = false;
            mainCamera.SetActive(false);
        }
    }
}
