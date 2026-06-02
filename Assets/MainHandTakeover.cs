using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class MainHandTakeover : MonoBehaviour
{
    public SphereCollider sphereCollider;
    private List<GameObject> objectsInRange = new List<GameObject>();
    public ContactTakeover contactTakeover;
    public GameObject pickedUpObject;
    private bool isPickingUp;

    void Start()
    {
        //contactTakeover = GetComponent<ContactTakeover>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == pickedUpObject)
        {
            return;
        }

        // Check if this collider belongs to a Food parent
        if (FindFoodParent(other.gameObject) != null)
        {
            UpdateObjectsInRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == pickedUpObject)
        {
            return;
        }

        // Check if this collider belonged to a Food parent
        if (FindFoodParent(other.gameObject) != null)
        {
            UpdateObjectsInRange();
        }
    }

    void Update()
    {
        // Detect input and instantly execute on the exact frame it's pressed
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

    // Remove the pickup logic completely from FixedUpdate
    private void FixedUpdate()
    {
        // Leave empty or delete entirely if not tracking other physics mechanics
    }

    void drop()
    {
        if (pickedUpObject == null) return;

        pickedUpObject.transform.SetParent(null);

        foreach (MeshCollider meshCol in pickedUpObject.GetComponentsInChildren<MeshCollider>())
        {
            meshCol.enabled = true;
        }

        pickedUpObject.GetComponent<PhotonTransformView>().enabled = true;
        pickedUpObject.GetComponent<Rigidbody>().isKinematic = false;
        pickedUpObject.GetComponent<Rigidbody>().WakeUp();

        pickedUpObject = null;
    }

    void pickUp()
    {
        pickedUpObject = objectsInRange[0];
        objectsInRange.RemoveAt(0);

        contactTakeover.CheckAndTakeover(pickedUpObject);


        foreach (MeshCollider meshCol in pickedUpObject.GetComponentsInChildren<MeshCollider>())
        {
            meshCol.enabled = false;
        }

        pickedUpObject.GetComponent<PhotonTransformView>().enabled = false;
        pickedUpObject.GetComponent<Rigidbody>().isKinematic = true;
        pickedUpObject.GetComponent<Rigidbody>().Sleep();

        pickedUpObject.transform.SetParent(transform);
        pickedUpObject.transform.localPosition = Vector3.zero;
        pickedUpObject.transform.localRotation = Quaternion.identity;
    }

    private void UpdateObjectsInRange()
    {
        if (sphereCollider == null)
        {
            return;
        }

        Vector3 center = sphereCollider.transform.TransformPoint(sphereCollider.center);
        float radius = sphereCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        objectsInRange = hitColliders
            .Select(col => FindFoodParent(col.gameObject)) // Find the tagged parent for every hit collider
            .Where(foodParent => foodParent != null && foodParent != pickedUpObject) // Filter out nulls and already held items
            .Distinct() // Ensure we don't add the same parent multiple times if it has multiple child colliders
            .OrderBy(foodParent => Vector3.Distance(center, foodParent.transform.position)) // Distance to the actual Food object
            .ToList();
    }

    /// <summary>
    /// Helper method to travel up the hierarchy and find the GameObject tagged "Food".
    /// </summary>
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
        return null; // No parent with the "Food" tag found
    }
}