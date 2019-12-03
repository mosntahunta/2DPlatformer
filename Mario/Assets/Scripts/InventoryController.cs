using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public ItemDatabase database;
    private List<InventoryItem> characterItems;

    private int equippedWeaponIndex;

    // temporary initialization
    private void Start()
    {
        characterItems = new List<InventoryItem>();
        AddItem(0); // initialize with sword
        equippedWeaponIndex = 0;
    }

    public void AddItem(int id)
    {
        InventoryItem itemToAdd = database.GetItem(id);
        characterItems.Add(itemToAdd);
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
        InventoryItem foundItem = characterItems.Find(item => item.id == id);
        return foundItem;
    }

    public InventoryItem GetCurrentWeapon()
    {
        return characterItems[equippedWeaponIndex];
    }

    public void ChangeWeapon()
    {
        if (characterItems.Count > 1)
        {
            if (equippedWeaponIndex < characterItems.Count - 1)
            {
                equippedWeaponIndex++;
            }
            else
            {
                equippedWeaponIndex = 0;
            }
        }
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
