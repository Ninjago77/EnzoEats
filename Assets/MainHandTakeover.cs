using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class MainHandTakeover : MonoBehaviourPun
{
    public SphereCollider sphereCollider;
    private List<GameObject> objectsInRange = new List<GameObject>();
    public ContactTakeover contactTakeover;
    public GameObject pickedUpObject;

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.gameObject == pickedUpObject) return;

        if (FindFoodParent(other.gameObject) != null)
        {
            UpdateObjectsInRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.gameObject == pickedUpObject) return;

        if (FindFoodParent(other.gameObject) != null)
        {
            UpdateObjectsInRange();
        }
    }

    void Update()
    {
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

        PhotonView targetPV = pickedUpObject.GetComponent<PhotonView>();
        if (targetPV != null)
        {
            // This calls NetworkDrop on the Food item's PhotonView script
            targetPV.RPC("NetworkDrop", RpcTarget.AllBuffered);
        }

        pickedUpObject = null;
    }

    void pickUp()
    {
        pickedUpObject = objectsInRange[0];
        objectsInRange.RemoveAt(0);

        contactTakeover.CheckAndTakeover(pickedUpObject);

        PhotonView targetPV = pickedUpObject.GetComponent<PhotonView>();
        if (targetPV != null)
        {
            // This calls NetworkPickUp on the Food item's PhotonView script
            targetPV.RPC("NetworkPickUp", RpcTarget.AllBuffered, photonView.ViewID);
        }
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
