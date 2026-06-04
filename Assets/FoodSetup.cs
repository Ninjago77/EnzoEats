using UnityEngine;
using Photon.Pun;

public class FoodSetup : MonoBehaviour
{
    public int itemIndex = -1;
    public float itemDamage = 69f;
    //[PunRPC]
    //public void RequestDestroyObject(int viewID)
    //{
    //    // Only the Master Client executes this block
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        PhotonView targetView = PhotonView.Find(viewID);
    //        if (targetView != null)
    //        {
    //            PhotonNetwork.Destroy(targetView.gameObject);
    //        }
    //    }
    //}
}