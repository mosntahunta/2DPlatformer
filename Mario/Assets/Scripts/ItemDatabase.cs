using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<InventoryItem> items { get; set; }
    private void Awake()
    {
        BuildDatabase();
    }

    public InventoryItem GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }

    void BuildDatabase()
    {
        items = new List<InventoryItem>()
        {
            new InventoryItem(0, "sword", "basic sword",
            new Dictionary<string, int>()
            {
                {"Power", 10 },
                {"Durability", 50 }
            }),
            new InventoryItem(1, "gun", "basic gun",
            new Dictionary<string, int>()
            {
                {"Power", 5 },
                {"Range", 10 }
            })
        };

    }
}
