using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class FoodNetworkController : MonoBehaviourPun
{
    [PunRPC]
    public void NetworkPickUp(int handViewID)
    {
        // Find the specific hand object across the network via its ID
        PhotonView handPV = PhotonView.Find(handViewID);
        if (handPV == null) return;

        // Disable sync components so network updates stop overriding local parenting
        if (TryGetComponent(out PhotonTransformView ptv)) ptv.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false; // Prevents weird physics glitches while held
            rb.Sleep();
        }

        foreach (MeshCollider meshCol in GetComponentsInChildren<MeshCollider>())
        {
            meshCol.enabled = false;
        }

        // Parent the food item directly to the HAND that picked it up
        transform.SetParent(handPV.transform);
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
            rb.detectCollisions = true;
            rb.WakeUp();
        }

        // Re-enable the transform view so it updates position seamlessly again
        if (TryGetComponent(out PhotonTransformView ptv)) ptv.enabled = true;
    }
}