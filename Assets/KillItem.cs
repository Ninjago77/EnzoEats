using UnityEngine;
using Photon.Pun;

public class KillItem : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Pass ANY specific GameObject with a PhotonView here to destroy it safely.
    /// </summary>
    public void DestroyTargetObject(GameObject target)
    {
        PhotonView targetPV = target.GetComponent<PhotonView>();

        if (targetPV == null)
        {
            // Not a network object, just standard Unity destroy
            Destroy(target);
            return;
        }

        // If we own it or are Master Client, delete it directly
        if (targetPV.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(target);
        }
        else
        {
            // Pass the target's unique PhotonView ID to the Master Client
            photonView.RPC("RPC_RequestTargetDestroy", RpcTarget.MasterClient, targetPV.ViewID);
        }
    }

    [PunRPC]
    private void RPC_RequestTargetDestroy(int viewID)
    {
        // Find the specific object across the network using its unique ID
        PhotonView targetPV = PhotonView.Find(viewID);

        if (targetPV != null)
        {
            PhotonNetwork.Destroy(targetPV.gameObject);
        }
    }
}