using System;
using System.Security.Cryptography;
using System.Text;
using Photon.Pun;
using UnityEngine;

public class SkinSetup : MonoBehaviour
{


    public GameObject[] skinsList;
    public PhotonView photonView;

    private int skinIndex;
    private GameObject skin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skinSetup();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void skinSetup()
    {
        UnityEngine.Random.InitState(convertToNumericSeed(photonView.Owner.UserId));
        skinIndex = UnityEngine.Random.Range(0, skinsList.Length);
        skin = Instantiate(skinsList[skinIndex], transform, false);
        skin.transform.localScale = new Vector3(2f, 2f, 2f);
        //skin.transform.localPosition = new Vector3(0, -1, 0);
    }

    private int convertToNumericSeed(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        // Instantiate the SHA256 cryptographic engine safely using a statement
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            return BitConverter.ToInt32(hashBytes, 0);
        }

    }
}
