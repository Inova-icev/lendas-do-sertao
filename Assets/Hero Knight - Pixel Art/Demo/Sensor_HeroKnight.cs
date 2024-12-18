using UnityEngine;
using System.Collections;

public class Sensor_HeroKnight : MonoBehaviour
{

    private int m_ColCount = 0;

    private float m_DisableTimer;

    public LayerMask layerPlayer;
    public LayerMask layerPlataforma;

    private void OnEnable()
    {
        m_ColCount = 0;
    }

    void Start()
    {
        // Ignora colisões entre o jogador e minions
        Physics2D.IgnoreLayerCollision(layerPlayer, layerPlayer, true);
    }

    public bool State()
    {
        if (m_DisableTimer > 0)
            return false;
        return m_ColCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Detecta apenas camadas específicas
        if (((1 << other.gameObject.layer) & layerPlataforma) != 0)
        {
            m_ColCount++;

            // Verifica se o objeto é uma plataforma flutuante
            if (other.gameObject.layer == LayerMask.NameToLayer("Plataforma"))
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, false);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Detecta apenas camadas específicas
        if (((1 << other.gameObject.layer) & layerPlataforma) != 0)
        {
            m_ColCount--;

            // Permite atravessar a plataforma novamente
            if (other.gameObject.layer == LayerMask.NameToLayer("Plataforma"))
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, true);
            }
        }
    }


    void Update()
    {
        m_DisableTimer -= Time.deltaTime;
    }

    public void Disable(float duration)
    {
        m_DisableTimer = duration;
    }
}
