using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHp : MonoBehaviour
{
    public float hp;
    public float invulnerableWindow;

    public void takeDmg(float dmg)
    {
        hp -= dmg;
    }
}
