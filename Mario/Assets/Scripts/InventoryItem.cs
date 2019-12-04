using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public int id;
    public string title;
    public string description;
    public Dictionary<string, int> stats = new Dictionary<string, int>();

    // Constructor
    public InventoryItem(int id, string title, string description, Dictionary<string, int> stats)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.stats = stats;
    }

    // Copy constructor
    public InventoryItem(InventoryItem item)
    {
        id = item.id;
        title = item.title;
        description = item.description;
        stats = item.stats;
    }
}
