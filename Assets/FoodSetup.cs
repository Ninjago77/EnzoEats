using Photon.Pun;
using UnityEngine;

public class FoodSetup : MonoBehaviour
{

    public GameObject[] itemsList;

    public int itemIndex;
    private GameObject item;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foodSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void foodSetup()
    {
        item = Instantiate(itemsList[itemIndex], transform, false);
        item.transform.localScale = new Vector3(.5f, .5f, .5f);
    }
}
