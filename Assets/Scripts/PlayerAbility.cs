using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public Ability[] abilities;
    public Passive[] passives;
    private float[] cooldowns;

    void Start()
    {
        cooldowns = new float[abilities.Length];

        // Aplicando passivas no personagem
        foreach (Passive passive in passives)
        {
            passive.ApplyPassive(gameObject);
        }
    }

    void Update()
    {
        // verificar inputs do jogador para ativar habilidades
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseAbility(0); // Habilidade no slot Q
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            UseAbility(1); // Habilidade no slot W
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseAbility(2); // Habilidade no slot E
        }

        // Atualizando o tempo de recarga das habilidades
        for (int i = 0; i < cooldowns.Length; i++)
        {
            if (cooldowns[i] > 0)
                cooldowns[i] -= Time.FixedDeltaTime;
        }
    }

    public void UseAbility(int index)
    {
        if (cooldowns[index] <= 0)
        {
            abilities[index].Activate(gameObject);
            cooldowns[index] = abilities[index].cooldown;
        }
        else
        {
            Debug.Log("Habilidade em recarga. ");
        }
    }
}
