using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public InventoryModel model;
    
    public InventoryItem GetCurrentWeapon()
    {
        return model.GetItem(model.equippedWeaponIndex);
    }

    public void ChangeWeapon()
    {
        if (model.characterItems.Count > 1)
        {
            if (model.equippedWeaponIndex < model.characterItems.Count - 1)
            {
                model.equippedWeaponIndex++;
            }
            else
            {
                model.equippedWeaponIndex = 0;
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
                model.characterItems.Add(item.itemId);
                collider.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Items must have the PickupItem component");
            }
        }
    }
}
