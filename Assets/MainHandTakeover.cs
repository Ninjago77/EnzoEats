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
    public MainHandSetup mainHandSetup;
    public Camera playerCamera;
    public GameObject pickedUpObject;
    public float clickForceMagnitude = 1.0f;
    private float clickForce = 1.0f;

    // Instantly track inventory locally to avoid E & Q delay and duplicate drops
    private int currentInventoryItem = -1;
    // Safely track objects waiting to be destroyed on server confirmation
    private List<GameObject> pendingDestroys = new List<GameObject>();

    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    private void Start()
    {
        // Add a check so joining players don't reset the host's inventory
        if (photonView.IsMine)
        {
            resetInv();
        }
    }

    private void resetInv()
    {
        if (!photonView.IsMine) return;

        currentInventoryItem = -1;
        Hashtable props = new Hashtable();
        int[] inv = new int[] { -1, -1, -1 };
        props["inventory"] = inv;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // Instantly update our visual skin without waiting for server return trip
        if (mainHandSetup != null) mainHandSetup.UpdateItemSkin(inv);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        GameObject foodParent = FindFoodParent(other.gameObject);
        if (foodParent != null)
        {
            // Ignore objects we just picked up or are waiting to be destroyed
            if (foodParent == pickedUpObject || pendingDestroys.Contains(foodParent)) return;
            UpdateObjectsInRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;

        GameObject foodParent = FindFoodParent(other.gameObject);
        if (foodParent != null)
        {
            if (foodParent == pickedUpObject || pendingDestroys.Contains(foodParent)) return;
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
                // Rely on local instant state rather than network-delayed custom properties
                if (currentInventoryItem != -1)
                {
                    drop();
                }
                pickUp();
            }
        }

        if (Input.GetButtonDown("Drop"))
        {
            if (currentInventoryItem != -1)
            {
                drop();
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            ; // HUMAN ME LISTEN TO ME FIRE OR LEFT CLICK HERE
            // clickForce from click on to click off later here
            if (currentInventoryItem != -1)
            {
                MainFire();
            }
        }
    }

    void drop()
    {
        PhotonNetwork.Instantiate(prefabList[currentInventoryItem], transform.position, transform.rotation);
        resetInv();
    }

    void MainFire()
    {
        GameObject obj = PhotonNetwork.Instantiate(prefabList[currentInventoryItem], transform.position, transform.rotation);
        resetInv();
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.AddForce(playerCamera.transform.forward.normalized*clickForce*clickForceMagnitude,ForceMode.Impulse);
    }

    void pickUp()
    {
        pickedUpObject = objectsInRange[0];
        objectsInRange.RemoveAt(0);

        PhotonView foodView = pickedUpObject.GetComponent<PhotonView>();
        currentInventoryItem = pickedUpObject.GetComponent<FoodSetup>().itemIndex;

        // 2. Write data to custom properties immediately
        Hashtable props = new Hashtable();
        int[] inv = new int[] { currentInventoryItem, -1, -1 };
        props["inventory"] = inv;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // Instant visual update locally
        if (mainHandSetup != null) mainHandSetup.UpdateItemSkin(inv);

        // 3. Request ownership. We DO NOT destroy it yet. 
        if (foodView != null && !foodView.IsMine)
        {
            pendingDestroys.Add(pickedUpObject);
            foodView.RequestOwnership();
        }
        else if (foodView != null && foodView.IsMine)
        {
            // If we already happen to own it, destroy it immediately safely
            PhotonNetwork.Destroy(pickedUpObject);
            // We leave pickedUpObject assigned so it gets skipped in subsequent UpdateObjectsInRange calls
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
            // Filter out items that are currently pending network destruction
            .Where(foodParent => foodParent != null && foodParent != pickedUpObject && !pendingDestroys.Contains(foodParent))
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

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer) { }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView == null || targetView.gameObject == null) return;

        // Verify if any object we spammed pick up on finally transferred ownership
        if (pendingDestroys.Contains(targetView.gameObject) && targetView.IsMine)
        {
            pendingDestroys.Remove(targetView.gameObject);
            PhotonNetwork.Destroy(targetView.gameObject);
            if (pickedUpObject == targetView.gameObject)
            {
                pickedUpObject = null;
            }
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest) { }
}