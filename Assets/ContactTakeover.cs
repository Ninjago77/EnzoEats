using UnityEngine;
using Photon.Pun;
using Photon.Realtime; // Required for the Player class

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class ContactTakeover : MonoBehaviour, IPunOwnershipCallbacks
{
    private PhotonView photonView;
    private Rigidbody rb;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    // CRITICAL: You must manually register ownership callbacks in PUN 2
    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckAndTakeover(collision.gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    CheckAndTakeover(other.gameObject);
    //}

    public void CheckAndTakeover(GameObject hittingObject)
    {
        PhotonView playerView = hittingObject.GetComponent<PhotonView>();

        if (playerView != null && playerView.IsMine)
        {
            if (!photonView.IsMine)
            {
                photonView.RequestOwnership();
            }
        }
    }

    // --- IPunOwnershipCallbacks Implementation ---

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        // Not needed for 'Takeover' mode, but required by the interface
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        // Ensure this callback is reacting to THIS specific object's ownership change
        if (targetView == photonView)
        {
            // Enable physics simulation for the new owner, disable it for everyone else
            rb.isKinematic = !photonView.IsMine;
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        // Optional fallback logic if a takeover request fails
    }
}