using System;
using System.Security.Cryptography;
using System.Text;
using Photon.Pun;
using UnityEngine;
using TMPro; // 1. Added TMPro namespace

public class PlayerSetup : MonoBehaviourPun
{
    public PlayerMovement playerMovement;
    public GameObject mainCamera;
    public MouseXPlay mouseXPlay;
    public MouseYCam mouseYCam;
    public MainHandTakeover mainHandTakeover;

    [Header("UI / Name Tag")]
    public TextMeshPro nameTagText; // 2. Reference to the 3D TextMeshPro component

    void Start()
    {
        // Set the nickname text
        if (nameTagText != null)
        {
            if (photonView.Owner != null && !string.IsNullOrEmpty(photonView.Owner.NickName))
            {
                nameTagText.text = photonView.Owner.NickName;
            }
            else
            {
                // Fallback in case a nickname isn't set yet
                nameTagText.text = "Player " + photonView.OwnerActorNr;
            }
        }

        if (photonView.IsMine)
        {
            // This is YOU. Enable controls.
            playerMovement.enabled = true;
            mouseXPlay.enabled = true;
            mouseYCam.enabled = true;
            mainHandTakeover.enabled = true;
            mainCamera.SetActive(true);

            // Hide your own name tag so it doesn't float in front of your camera view
            if (nameTagText != null)
            {
                nameTagText.gameObject.SetActive(false);
            }
        }
        else
        {
            // This is a REMOTE PLAYER clone. Explicitly disable controls and cameras.
            playerMovement.enabled = false;
            mouseXPlay.enabled = false;
            mouseYCam.enabled = false;
            mainHandTakeover.enabled = false;
            mainCamera.SetActive(false);

            // Ensure remote players have their name tags visible
            if (nameTagText != null)
            {
                nameTagText.gameObject.SetActive(true);
            }
        }
    }

    // 3. Keep remote name tags facing the active local camera
    void LateUpdate()
    {
        // Only run this for other players, using the active Main Camera
        if (!photonView.IsMine && nameTagText != null && Camera.main != null)
        {
            nameTagText.transform.forward = Camera.main.transform.forward;
        }
    }
}