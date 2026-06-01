using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Changed inheritance to MonoBehaviourPun so we can access photonView easily
public class MainHandTakeover : MonoBehaviourPun
{
    public SphereCollider sphereCollider;
    private List<GameObject> objectsInRange = new List<GameObject>();
    public ContactTakeover contactTakeover;
    public GameObject pickedUpObject;
    private bool isPickingUp;

    private void OnTriggerEnter(Collider other)
    {
        // Network safeguard: Only the local owner of this player should track items in range
        if (!photonView.IsMine) return;

        if (other.gameObject == pickedUpObject)
        {
            return;
        }

        if (FindFoodParent(other.gameObject) != null)
        {
            UpdateObjectsInRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Network safeguard
        if (!photonView.IsMine) return;

        if (other.gameObject == pickedUpObject)
        {
            return;
        }

        if (FindFoodParent(other.gameObject) != null)
        {
            UpdateObjectsInRange();
        }
    }

    void Update()
    {
        // CRITICAL FIX: Stop non-local clones of this player from executing input, 
        // which was stealing ownership or double-triggering pickups
        if (!photonView.IsMine) return;

        if (Input.GetButtonDown("PickUp"))
        {
            if (objectsInRange.Count > 0)
            {
                if (pickedUpObject != null)
                {
                    drop();
                }
                pickUp();
            }
        }
    }

    void drop()
    {
        if (pickedUpObject == null) return;

        // Get the PhotonView of the object we want to drop
        PhotonView targetPV = pickedUpObject.GetComponent<PhotonView>();
        if (targetPV != null)
        {
            // Request all clients to execute the physics and unparenting logic simultaneously
            targetPV.RPC("NetworkDrop", RpcTarget.AllBuffered);
        }

        pickedUpObject = null;
    }

    void pickUp()
    {
        pickedUpObject = objectsInRange[0];
        objectsInRange.RemoveAt(0);

        // Take ownership of the food item before modifying its transform state
        contactTakeover.CheckAndTakeover(pickedUpObject);

        PhotonView targetPV = pickedUpObject.GetComponent<PhotonView>();
        if (targetPV != null)
        {
            // Pass this player's PhotonView ID so everyone knows who to parent it to
            targetPV.RPC("NetworkPickUp", RpcTarget.AllBuffered, photonView.ViewID);
        }
    }

    // --- PUN RPCs TO EXECUTE TRASFORM CHANGES SYNCHRONOUSLY ACROSS ALL CLIENTS ---

    [PunRPC]
    public void NetworkPickUp(int playerViewID, PhotonMessageInfo info)
    {
        // Find the player object across the network via its ID
        PhotonView playerPV = PhotonView.Find(playerViewID);
        if (playerPV == null) return;

        // Disable sync components so network updates stop overriding the local parenting positioning
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

        // Parent it to the specific player who picked it up
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


    private void UpdateObjectsInRange()
    {
        if (sphereCollider == null) return;

        Vector3 center = sphereCollider.transform.TransformPoint(sphereCollider.center);
        float radius = sphereCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        objectsInRange = hitColliders
            .Select(col => FindFoodParent(col.gameObject))
            .Where(foodParent => foodParent != null && foodParent != pickedUpObject)
            .Distinct()
            .OrderBy(foodParent => Vector3.Distance(center, foodParent.transform.position))
            .ToList();
    }

    private GameObject FindFoodParent(GameObject child)
    {
        Transform current = child.transform;
        while (current != null)
        {
            if (current.CompareTag("Food"))
            {
                return current.gameObject;
            }
            current = current.parent;
        }
        return null;
    }
}
