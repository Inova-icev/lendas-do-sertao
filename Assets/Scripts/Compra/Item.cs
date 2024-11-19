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
    public string name;        // Nome do item
    public ItemType type;      // Tipo do item (ex: buff de velocidade ou dano)
    public float effectValue;  // Valor do efeito (ex: multiplicador de velocidade ou dano)
    public int price;          // Preço do item

    // Construtor opcional para criar itens no código
    public Item(string name, ItemType type, float effectValue, int price)
    {
        this.name = name;
        this.type = type;
        this.effectValue = effectValue;
        this.price = price;
    }

}