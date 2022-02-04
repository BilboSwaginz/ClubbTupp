using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Clothes : MonoBehaviour
{
    public Sprite displaySprite;
    public int price;
    public enum Type { Shirt, Pants, Hat };
    public Type type;
    
}
