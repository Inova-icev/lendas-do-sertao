using UnityEngine;

public enum ItemType
{
    SpeedBuff,
    DamageBuff,
    HealthBuff
}

[System.Serializable]
public class Item
{
    public string name;        
    public ItemType type;     
    public float effectValue; 
    public int price;         

    public Item(string name, ItemType type, float effectValue, int price)
    {
        this.name = name;
        this.type = type;
        this.effectValue = effectValue;
        this.price = price;
    }

}