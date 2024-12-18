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
    public ItemType type;      // Tipo do item
    public float effectValue;  // Valor do efeito aplicado
    public int price;          // Preço do item

    public Item(string name, ItemType type, float effectValue, int price)
    {
        this.name = name;
        this.type = type;
        this.effectValue = effectValue;
        this.price = price;
    }
}
