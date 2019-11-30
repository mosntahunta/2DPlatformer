using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public ItemDatabase database;
    private List<InventoryItem> characterItems = new List<InventoryItem>();

    public void AddItem(int id)
    {
        InventoryItem itemToAdd = database.GetItem(id);
        characterItems.Add(itemToAdd);
        Debug.Log("item added: " + characterItems.Count);
    }

    public void RemoveItem(int id)
    {
        InventoryItem itemToRemove = GetItem(id);
        if (itemToRemove != null)
        {
            characterItems.Remove(itemToRemove);
        }
    }
    
    public InventoryItem GetItem(int id)
    {
        return characterItems.Find(item => item.id == id);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            PickupItem item = collider.gameObject.GetComponent<PickupItem>();
            if (item != null)
            {
                AddItem(item.itemId);
                collider.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Items must have the PickupItem component");
            }
        }
    }
}
