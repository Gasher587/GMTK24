using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public GameObject Slots;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateUI()
    {
        Debug.Log("Updating UI");
    }

    public void addPickup(GameObject pickupObject)
    {
        SlotScript[] SlotScripts = Slots.GetComponentsInChildren<SlotScript>();
        for (int i = 0; i < SlotScripts.Length; i++)
        {
            if (SlotScripts[i].occuipied == false)
            {
                SlotScripts[i].inventoryItem = pickupObject;
                break;
            }
        }
    }
}
