using UnityEngine;

public class Shop : MonoBehaviour
{
    public Item[] itemsForSale;  
    public Player player;        

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();  
        }
    }

    public void ShowItemsForSale()
    {
        Debug.Log("Itens disponíveis para venda:");
        for (int i = 0; i < itemsForSale.Length; i++)
        {
            Item item = itemsForSale[i];
            Debug.Log($"{i + 1}. {item.name} - Preço: {item.price} Ouro - Tipo: {item.type}");
        }
    }

    public void BuyItem(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < itemsForSale.Length)
        {
            Item item = itemsForSale[itemIndex];
            if (player.gold >= item.price)
            {
                player.GainGold(-item.price); 
                player.ApplyItemEffect(item);  
                Debug.Log($"Você comprou {item.name}! Ouro restante: {player.gold}");
            }
            else
            {
                Debug.Log("Você não tem ouro suficiente para comprar este item.");
            }
        }
        else
        {
            Debug.Log("Índice de item inválido.");
        }
    }

    // Exemplo de como o jogador pode acessar a loja (isso pode ser chamado em algum evento, como interação com a loja no jogo)
    public void InteractWithShop()
    {
        // Exibe os itens na loja
        ShowItemsForSale();

        // O jogador escolhe um item para comprar (por exemplo, índice 0 para o primeiro item)
        int itemIndex = 0; // Isso seria provavelmente configurado por alguma interação no jogo (ex: botão de compra)
        BuyItem(itemIndex); // Simulando a compra do primeiro item
    }

}