using System;
using System.Security.Cryptography;
using System.Text;
using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
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
        //PhotonNetwork.Instantiate("Food", new Vector3(transform.position.x, 5, transform.position.z), Quaternion.identity);

        // 3. Check if this specific player object belongs to the person running the game
        if (photonView.IsMine)
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
