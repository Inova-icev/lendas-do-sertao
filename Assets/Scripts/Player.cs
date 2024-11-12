using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim;
    public int gold = 0;
    public float speed = 5f; // Velocidade base do jogador
    private float currentSpeed;
    private Vida vidaComponent; // Referência ao componente Vida
    private float damageMultiplier = 1f; // Multiplicador de dano inicial

    public int baseDamage = 10; // Dano base do jogador

    void Start()
    {
        anim = GetComponent<Animator>();
        currentSpeed = speed; // Inicialize com a velocidade base
        vidaComponent = GetComponent<Vida>(); // Obtém o componente Vida
    }

    void Update()
    {
        // Movimento simples de exemplo
        float move = Input.GetAxis("Horizontal") * currentSpeed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("Correndo", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetBool("Correndo", false);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetBool("Atacando", true);
            Attack(); // Exemplo de ataque
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            anim.SetBool("Atacando", false);
        }
    }

    // Método para calcular o dano causado com o buff
    private void Attack()
    {
        int damageToDeal = Mathf.RoundToInt(baseDamage * damageMultiplier);
        Debug.Log($"Dano causado: {damageToDeal}");
        // Aqui você pode implementar lógica para encontrar inimigos próximos e aplicar dano a eles
    }

    public void ApplyBuff(float multiplier)
    {
        currentSpeed *= multiplier;
        Debug.Log($"Buff de velocidade aplicado! Nova velocidade: {currentSpeed}");
    }

    public void RemoveBuff(float multiplier)
    {
        currentSpeed /= multiplier;
        Debug.Log($"Buff de velocidade removido. Velocidade de volta ao normal: {currentSpeed}");
    }

    // Aplicar um buff de dano
    public void ApplyDamageBuff(float multiplier)
    {
        damageMultiplier *= multiplier;
        Debug.Log($"Buff de dano aplicado! Multiplicador de dano atual: {damageMultiplier}");
    }

    // Remover o buff de dano
    public void RemoveDamageBuff(float multiplier)
    {
        damageMultiplier /= multiplier;
        Debug.Log($"Buff de dano removido. Multiplicador de dano de volta ao normal: {damageMultiplier}");
    }

    public void TakeDamage(int damageAmount)
    {
        if (vidaComponent != null)
        {
            vidaComponent.TakeDamage(damageAmount); // Passa o dano para o componente Vida
        }
    }

    public void GainGold(int amount)
    {
        gold += amount;
        Debug.Log($"Jogador ganhou {amount} de ouro. Total: {gold}");
    }
}
