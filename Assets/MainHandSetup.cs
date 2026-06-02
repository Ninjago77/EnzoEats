using NUnit.Framework;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class MainHandSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] itemsList;

    public int itemIndex = -1;
    private GameObject item;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void UpdateItemSkin(int[] newInventory)
    { 
        itemIndex = newInventory[0];
        if (item != null)
        {
            Destroy(item);
            item = null;
        }
        if (itemIndex != -1)
        {
            item = Instantiate(itemsList[itemIndex], transform, false);
        }
    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        // Only run if the property changed belongs to the owner of this script
        if (targetPlayer == photonView.Owner && changedProps.ContainsKey("inventory"))
        {
            int[] newInventory = (int[])changedProps["inventory"];

            // Trigger the linked function
            UpdateItemSkin(newInventory);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
