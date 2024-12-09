using UnityEngine;

public class Shop : MonoBehaviour
{
    // Itens disponíveis na loja
    public Item[] itemsForSale = new Item[]
    {
        new Item("Speed Boost", ItemType.SpeedBuff, 1.5f, 50),   // Item 1: Speed Buff
        new Item("Damage Boost", ItemType.DamageBuff, 1.2f, 60),  // Item 2: Damage Buff
        new Item("Health Potion", ItemType.HealthBuff, 50f, 30)   // Item 3: Health Potion
    };
    public Player player;   // Referência ao jogador
    private bool playerInRange = false;  // Indica se o jogador está na área da loja

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();  // Encontra o jogador na cena, caso não tenha sido atribuído
        }
    }

    void Update()
    {
        // Verifica se o jogador está na área da loja e se a tecla "B" foi pressionada
        if (playerInRange && Input.GetKeyDown(KeyCode.B))
        {
            InteractWithShop();  // Exibe os itens da loja
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Quando o jogador entra na área da loja (detectado por colisão de trigger)
        if (other.CompareTag("Player"))
        {
            playerInRange = true;  // Marca que o jogador está na área da loja
            Debug.Log("Pressione 'B' para acessar a loja.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Quando o jogador sai da área da loja
        if (other.CompareTag("Player"))
        {
            playerInRange = false;  // Marca que o jogador saiu da área
            Debug.Log("Você saiu da área da loja.");
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
                player.GainGold(-item.price);  // Subtrai o valor do ouro do jogador
                player.ApplyItemEffect(item);  // Aplica o efeito do item
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

    public void InteractWithShop()
    {
        ShowItemsForSale();
    }

}