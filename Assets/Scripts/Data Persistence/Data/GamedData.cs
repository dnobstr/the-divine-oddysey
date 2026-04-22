using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GamedData
{
    // store Vector3 as float array for compatibility with JsonUtility
    public float[] playerPosition = new float[3];
    public int health;
    public int maxHealth;
    public int level;
    public List<string> inventory = new List<string>();
    public string sceneName;
    public long saveTimestamp;

    public GamedData() { }

    public static GamedData FromTransform(Transform t, int health, int maxHealth, int level, List<string> inventory, string sceneName)
    {
        var d = new GamedData();
        d.playerPosition[0] = t.position.x;
        d.playerPosition[1] = t.position.y;
        d.playerPosition[2] = t.position.z;
        d.health = health;
        d.maxHealth = maxHealth;
        d.level = level;
        d.inventory = new List<string>(inventory ?? new List<string>());
        d.sceneName = sceneName;
        d.saveTimestamp = DateTime.UtcNow.Ticks;
        return d;
    }

    public Vector3 GetPlayerPosition()
    {
        return new Vector3(playerPosition[0], playerPosition[1], playerPosition[2]);
    }
}
