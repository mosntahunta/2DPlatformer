using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : PersistableObject
{
    public int maxLives;
    public int currentLives;
    public Vector2 startingPosition;

    void Start()
    {
        currentLives = maxLives;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(currentLives);
        writer.Write(maxLives);
        writer.Write(startingPosition);
    }

    public override void Load(GameDataReader reader)
    {
        currentLives = reader.ReadInt();
        maxLives = reader.ReadInt();
        startingPosition = reader.ReadVector2();
    }
}
