using System.Collections.Generic;
using UnityEngine;

public class SaveLoadExample : MonoBehaviour
{
    public string saveFileName = "slot1.sav";
    public int playerHealth = 100;
    public int playerMaxHealth = 100;
    public int playerLevel = 1;
    public List<string> inventory = new List<string>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            DoSave();
        if (Input.GetKeyDown(KeyCode.F9))
            DoLoad();
    }

    public void DoSave()
    {
        var data = GamedData.FromTransform(transform, playerHealth, playerMaxHealth, playerLevel, inventory, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        SaveSystem.Save(saveFileName, data);
    }

    public void DoLoad()
    {
        var data = SaveSystem.Load(saveFileName);
        if (data == null) return;
        transform.position = data.GetPlayerPosition();
        playerHealth = data.health;
        playerMaxHealth = data.maxHealth;
        playerLevel = data.level;
        inventory = new List<string>(data.inventory);
        Debug.Log($"SaveLoadExample: Loaded save from {new System.DateTime(data.saveTimestamp)}");
    }
}