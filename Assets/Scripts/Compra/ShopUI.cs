using UnityEngine;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private UIDocument shopDocument;
    private VisualElement root;
    private Label currencyLabel;
    private Player player;

    public void Initialize(Player playerReference)
    {
        player = playerReference;
        Debug.Log("Player referenciado com sucesso no ShopUI.");
        UpdateCurrencyDisplay();
    }

    void OnEnable()
    {
        // Carrega a interface sempre que a loja é ativada
        root = shopDocument.rootVisualElement;
        SetupUI();
    }

    private void SetupUI()
    {
        Debug.Log("Configurando a UI da Loja...");

        // Busca o Label de moedas e botões com Query recursiva
        currencyLabel = root.Q<Label>("coin-count");
        Button closeStoreButton = root.Q<Button>("closeStoreButton");

        Button buyButton1 = root.Query<Button>("buyButton1").First();
        Button buyButton2 = root.Query<Button>("buyButton2").First();
        Button buyButton3 = root.Query<Button>("buyButton3").First();

        // Verificação se os elementos foram encontrados
        Debug.Log(currencyLabel != null ? "Label de moedas encontrado!" : "Label de moedas não encontrado!");
        Debug.Log(buyButton1 != null ? "Botão 1 encontrado!" : "Botão 1 não encontrado!");
        Debug.Log(buyButton2 != null ? "Botão 2 encontrado!" : "Botão 2 não encontrado!");
        Debug.Log(buyButton3 != null ? "Botão 3 encontrado!" : "Botão 3 não encontrado!");

        // Configurações dos botões de compra
        if (closeStoreButton != null)
            closeStoreButton.clicked += CloseShop;

        if (buyButton1 != null)
            buyButton1.clicked += () => BuyItem(new Item("Espada Lendária", ItemType.DamageBuff, 20, 120));

        if (buyButton2 != null)
            buyButton2.clicked += () => BuyItem(new Item("Botas Mágicas", ItemType.SpeedBuff, 10, 80));

        if (buyButton3 != null)
            buyButton3.clicked += () => BuyItem(new Item("Elixir de Vida", ItemType.HealthBuff, 30, 100));

        UpdateCurrencyDisplay();
    }

    private void BuyItem(Item item)
    {
        if (player != null && player.gold >= item.price)
        {
            player.GainGold(-item.price);
            player.ApplyPermanentEffect(item);
            UpdateCurrencyDisplay();
            Debug.Log($"{item.name} comprado! Efeito aplicado: {item.effectValue}");
        }
        else
        {
            Debug.Log("Ouro insuficiente ou jogador não definido.");
        }
    }

    private void UpdateCurrencyDisplay()
    {
        if (currencyLabel != null && player != null)
        {
            currencyLabel.text = player.gold.ToString();
        }
    }

    private void CloseShop()
    {
        gameObject.SetActive(false);
        Debug.Log("Loja fechada.");
    }
}
