using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InventoryModel : PersistableObject
{
    public List<int> characterItems; // id references to items in inventory
    [SerializeField] ItemDatabase database;

    public int equippedWeaponIndex;

    private void Start()
    {
        characterItems = new List<int>();

        characterItems.Add(0); // temporary initialization

        equippedWeaponIndex = 0;
    }

    public InventoryItem GetItem(int id)
    {
        return database.GetItem(id);
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(equippedWeaponIndex);

        string ids = String.Join(",", characterItems.ToArray());
        writer.Write(ids);
    }

    public override void Load(GameDataReader reader)
    {
        equippedWeaponIndex = reader.ReadInt();

        characterItems.Clear();
        string[] ids = reader.ReadString().Split(',');
        foreach(string id in ids)
        {
            characterItems.Add(int.Parse(id));
        }
    }
}
