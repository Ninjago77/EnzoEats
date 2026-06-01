using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class FoodNetworkController : MonoBehaviourPun
{
    [PunRPC]
    public void NetworkPickUp(int playerViewID)
    {
        // Find the player object across the network via its ID
        PhotonView playerPV = PhotonView.Find(playerViewID);
        if (playerPV == null) return;

        // Disable sync components so network updates stop overriding local parenting
        if (TryGetComponent(out PhotonTransformView ptv)) ptv.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.Sleep();
        }

        foreach (MeshCollider meshCol in GetComponentsInChildren<MeshCollider>())
        {
            meshCol.enabled = false;
        }

        // Parent the food item to the specific player who picked it up
        transform.SetParent(playerPV.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    [PunRPC]
    public void NetworkDrop()
    {
        transform.SetParent(null);

        foreach (MeshCollider meshCol in GetComponentsInChildren<MeshCollider>())
        {
            meshCol.enabled = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.WakeUp();
        }

        // Re-enable the transform view so it updates position seamlessly again
        if (TryGetComponent(out PhotonTransformView ptv)) ptv.enabled = true;
    }
}
