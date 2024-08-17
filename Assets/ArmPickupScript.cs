using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmPickupScript : MonoBehaviour
{
    public GameObject Inventory;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player Arm Collision");
        
        if (collision.gameObject.tag == "Player")
        {
            Inventory.GetComponent<InventoryScript>().addPickup(this.gameObject);
        }
    }
}
