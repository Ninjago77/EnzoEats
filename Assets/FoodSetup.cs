using UnityEngine;
using Photon.Pun;

public class FoodSetup : MonoBehaviourPunCallbacks
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
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (transform.position.y < -10f)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}