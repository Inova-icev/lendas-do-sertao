using UnityEngine;

public class Shop : MonoBehaviour
{
    private bool playerInRange = false; // Se o jogador está na loja
    [SerializeField] private GameObject shopUI; // Referência ao GameObject do ShopUI (UI Toolkit)

    void Start()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false); // A loja começa desativada
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.B))
        {
            ToggleShop();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player playerComponent = other.GetComponent<Player>(); // Declara e atribui o Player
        if (playerComponent != null)
        {
            playerInRange = true;
            ShopUI shopScript = shopUI.GetComponent<ShopUI>();
            if (shopScript != null)
            {
                shopScript.Initialize(playerComponent); // Passa o Player corretamente
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerInRange = false;
            if (shopUI != null)
            {
                shopUI.SetActive(false);
                Debug.Log("Você saiu da loja.");
            }
        }
    }

    private void ToggleShop()
    {
        if (shopUI != null)
        {
            bool isActive = shopUI.activeSelf;
            shopUI.SetActive(!isActive);
            Debug.Log(isActive ? "Loja fechada" : "Loja aberta");
        }
    }
}
