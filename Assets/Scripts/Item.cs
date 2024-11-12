using UnityEngine;

public enum ItemType
{
    SpeedBuff,
    DamageBuff
}

[System.Serializable]
public class Item
{
    public string name;        
    public int price;          
    public ItemType type;     
    public float effectValue;  

}