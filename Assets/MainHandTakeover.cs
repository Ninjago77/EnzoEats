using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MainHandTakeover : MonoBehaviourPun, IPunOwnershipCallbacks
{
    public SphereCollider sphereCollider;
    private List<GameObject> objectsInRange = new List<GameObject>();
    public string[] prefabList;
    //public ContactTakeover contactTakeover;
    public MainHandSetup mainHandSetup;
    public GameObject pickedUpObject;
    //public KillItem killItem;
    // Register callbacks so PUN tells this script when ownership shifts
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
    private void Start()
    {
        resetInv();
    }

    private void resetInv()
    {
        Hashtable props = new Hashtable();
        props["inventory"] = new int[] { -1, -1, -1 };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

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
                if (((int[])photonView.Owner.CustomProperties["inventory"])[0] != -1)
                {
                    drop();
                }
                pickUp();
            }
        }

        if (Input.GetButtonDown("Drop"))
        {
            if (((int[])photonView.Owner.CustomProperties["inventory"])[0] != -1)
            {
                drop();
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            ; // HUMAN ME LISTEN TO ME FIRE OR LEFT CLICK HERE
        }
    }

    void drop()

    {
        PhotonNetwork.Instantiate(prefabList[((int[])photonView.Owner.CustomProperties["inventory"])[0]], transform.position, transform.rotation);
        resetInv();
        
        //pickedUpObject.transform.SetParent(null);



        //foreach (MeshCollider meshCol in pickedUpObject.GetComponentsInChildren<MeshCollider>())
        //{
        //    meshCol.enabled = true;
        //}

        //pickedUpObject.GetComponent<PhotonTransformView>().enabled = true;
        //pickedUpObject.GetComponent<Rigidbody>().isKinematic = false;
        //pickedUpObject.GetComponent<Rigidbody>().WakeUp();
        //pickedUpObject = null;
    }
    //private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    //private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
    void pickUp()
    {
        pickedUpObject = objectsInRange[0];
        objectsInRange.RemoveAt(0);

        PhotonView foodView = pickedUpObject.GetComponent<PhotonView>();

        // 2. Write data to custom properties immediately
        Hashtable props = new Hashtable();
        props["inventory"] = new int[] { pickedUpObject.GetComponent<FoodSetup>().itemIndex, -1, -1 };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // 3. Request ownership. We DO NOT destroy it yet. 
        // We wait for OnOwnershipTransfered to trigger.
        if (foodView != null && !foodView.IsMine)
        {
            foodView.RequestOwnership();
        }
        else if (foodView != null && foodView.IsMine)
        {
            // If we already happen to own it, destroy it immediately safely
            PhotonNetwork.Destroy(pickedUpObject);
            pickedUpObject = null;
        }
        //ContactTakeover contactTakeover = pickedUpObject.GetComponent<ContactTakeover>();
        //contactTakeover.CheckAndTakeover(transform.parent.gameObject,"death");



        //    StartCoroutine(pickUpWait());
        //}

        //System.Collections.IEnumerator pickUpWait()
        //{
        //    float timeout = 3f;
        //    float elapsed = 0f;

        //    while (pickedUpObject != null &&
        //           pickedUpObject.GetComponent<PhotonView>()?.Owner?.UserId !=
        //           photonView?.Owner?.UserId)
        //    {
        //        elapsed += Time.deltaTime;
        //        if (elapsed >= timeout)
        //        {
        //            Debug.LogWarning("Pickup timed out - ownership transfer failed");
        //            pickedUpObject = null;
        //            yield break;
        //        }
        //        yield return null;
        //    }

        //    if (pickedUpObject == null) yield break;

        //Hashtable props = new Hashtable();
        //props["inventory"] = new int[] { pickedUpObject.GetComponent<FoodSetup>().itemIndex, -1, -1 };
        //PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        //PhotonNetwork.Destroy(pickedUpObject);
        //pickedUpObject = null;
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


    //public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer) { }

    //public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    //{
    //    // If this script's player just successfully gained ownership of the picked up item
    //    if (pickedUpObject != null && targetView == pickedUpObject.GetComponent<PhotonView>() && targetView.IsMine)
    //    {
    //        PhotonNetwork.Destroy(pickedUpObject);
    //    }
    //}

    //public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest) { }

    // --- IPunOwnershipCallbacks Implementation ---

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer) { }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        // This checks if the item we are currently trying to pick up 
        // successfully became ours on the network.
        if (pickedUpObject != null && targetView == pickedUpObject.GetComponent<PhotonView>() && targetView.IsMine)
        {
            PhotonNetwork.Destroy(pickedUpObject);
            pickedUpObject = null; // Clean up local pointer reference
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest) { }
}
