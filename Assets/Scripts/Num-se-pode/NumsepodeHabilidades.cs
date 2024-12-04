using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumsepodeHabilidade : MonoBehaviour
{
     private StatusBase statusBase; // Para acessar os atributos da classe StatusBase
    public Transform curaPoint; // Ponto de invocação do Poste Protetor
    public Transform ataquePoint; // Ponto de origem do ataque (braço de Num-se-pode)
    public Transform fumacentoPoint; // Ponto de origem do Domínio Fumacento
    public GameObject areaFumacaPrefab; // Prefab para representar a área de fumaça
    public float tempoEfeito = 5f; // Tempo de duração do efeito de cura e velocidade de ataque
    public float alcance = 5f; // Alcance das habilidades
    public float duracaoAtordoamento = 1f; // Duração do atordoamento em segundos
    public float cooldown = 15f; // Cooldown inicial das habilidades
    private float cooldownRestante = 0f; // Tempo restante para usar a habilidade novamente
    public float alcanceAssobio = 6f; // Alcance do Assobio Assustador
    public float reducaoVelocidade = 0.2f; // Redução base da velocidade de movimento
    public float duracaoMedo = 1.5f; // Duração do medo
    public float duracaoReducaoVelocidade = 1.5f; // Duração da redução de velocidade
    public float resistenciaBonus = 10f; // Resistência bônus durante o efeito
    public LayerMask inimigoLayers; // Camada dos inimigos que podem ser atingidos
    public LayerMask aliadoLayers; // Camada dos aliados que podem ser curados
    public float alcanceFumaca = 5f; // Alcance da fumaça
    public float reducaoPrecisao = 0.3f; // Redução na precisão dos inimigos
    public float aumentoEsquiva = 0.2f; // Aumento na esquiva do personagem
    public float duracaoFumaca = 8f; // Duração do Domínio Fumacento

    void Start()
    {
        statusBase = GetComponent<StatusBase>();
        if (statusBase == null)
        {
            Debug.LogError("StatusBase não encontrado no personagem!");
        }
    }

    void Update()
    {
        // Reduz o cooldown ao longo do tempo
        if (cooldownRestante > 0)
        {
            cooldownRestante -= Time.deltaTime;
        }

        // Habilidade Abraço Tenebroso
        if (Input.GetKeyDown(KeyCode.J) && cooldownRestante <= 0)
        {
            UsarHabilidadeAbraçoTenebroso();
        }

        // Habilidade Poste Protetor
        if (Input.GetKeyDown(KeyCode.K) && cooldownRestante <= 0)
        {
            UsarHabilidadePosteProtetor();
        }

        // Habilidade Assobio Assustador
        if (Input.GetKeyDown(KeyCode.L) && cooldownRestante <= 0)
        {
            UsarAssobioAssustador();
        }
         if (Input.GetKeyDown(KeyCode.H) && cooldownRestante <= 0)
        {
            UsarDominioFumacento();
        }
    }

    // Abraço Tenebroso
    void UsarHabilidadeAbraçoTenebroso()
    {
        cooldownRestante = cooldown - (statusBase.level - 1) * 2f; // Escala o cooldown com o nível
        Collider2D[] inimigos = Physics2D.OverlapCircleAll(ataquePoint.position, alcance, inimigoLayers);

        if (inimigos.Length > 0)
        {
            Collider2D inimigo = inimigos[0]; // Ataca o primeiro inimigo encontrado
            StatusBase statusInimigo = inimigo.GetComponent<StatusBase>();

            if (statusInimigo != null)
            {
                statusInimigo.Atordoar(duracaoAtordoamento); // Aplica o atordoamento
                Debug.Log("Inimigo atordoado!");
                StartCoroutine(PuxarInimigo(inimigo.transform)); // Puxa o inimigo para o personagem
            }
        }
        else
        {
            Debug.Log("Nenhum inimigo encontrado no alcance.");
        }
    }

    private IEnumerator PuxarInimigo(Transform inimigoTransform)
    {
        Vector3 posicaoInicial = inimigoTransform.position;
        Vector3 posicaoFinal = transform.position; // Posição de Num-se-pode
        float tempoDeDeslocamento = 1f;
        float tempoDecorrido = 0f;

        while (tempoDecorrido < tempoDeDeslocamento)
        {
            inimigoTransform.position = Vector3.Lerp(posicaoInicial, posicaoFinal, tempoDecorrido / tempoDeDeslocamento);
            tempoDecorrido += Time.deltaTime;
            yield return null;
        }

        inimigoTransform.position = posicaoFinal; // Garante o reposicionamento final
        Debug.Log("Inimigo puxado para a posição de Num-se-pode.");
    }

    // Poste Protetor
    void UsarHabilidadePosteProtetor()
    {
        cooldownRestante = cooldown - (statusBase.level - 1) * 2f; // Escala o cooldown com o nível
        Collider2D[] aliados = Physics2D.OverlapCircleAll(curaPoint.position, alcance, aliadoLayers);

        if (aliados.Length > 0)
        {
            foreach (Collider2D aliado in aliados)
            {
                StatusBase statusAliado = aliado.GetComponent<StatusBase>();
                if (statusAliado != null)
                {
                    float cura = statusBase.danoMagico * 0.5f; // Calcula a cura baseada no dano mágico
                    statusAliado.vidaAtual = Mathf.Min(statusAliado.vidaAtual + cura, statusAliado.vidaMaxima);
                    Debug.Log($"Aliado curado por {cura} de vida!");

                    StartCoroutine(AumentarVelocidadeAtaque(statusAliado));
                }
            }
        }
        else
        {
            Debug.Log("Nenhum aliado encontrado no alcance.");
        }
    }

    private IEnumerator AumentarVelocidadeAtaque(StatusBase aliadoStatus)
    {
        float velocidadeOriginal = aliadoStatus.velocidadeAtaque;
        aliadoStatus.velocidadeAtaque *= 1 + (statusBase.level * 0.05f); // Aumenta velocidade proporcional ao nível

        Debug.Log($"Velocidade de ataque aumentada para {aliadoStatus.velocidadeAtaque}");
        yield return new WaitForSeconds(tempoEfeito);

        aliadoStatus.velocidadeAtaque = velocidadeOriginal; // Restaura a velocidade original
        Debug.Log("Velocidade de ataque retornada ao valor original.");
    }

    // Assobio Assustador
    void UsarAssobioAssustador()
    {
        cooldownRestante = 12f - (statusBase.level - 1); // Cooldown escala com o nível
        Collider2D[] inimigos = Physics2D.OverlapCircleAll(transform.position, alcanceAssobio, inimigoLayers);

        if (inimigos.Length > 0)
        {
            foreach (Collider2D inimigo in inimigos)
            {
                StatusBase statusInimigo = inimigo.GetComponent<StatusBase>();
                if (statusInimigo != null)
                {
                    StartCoroutine(AplicarMedo(statusInimigo));
                    StartCoroutine(AplicarReducaoVelocidade(statusInimigo));
                }
            }
            StartCoroutine(AplicarResistencia());
        }
        else
        {
            Debug.Log("Nenhum inimigo encontrado no alcance do Assobio Assustador.");
        }
    }

    private IEnumerator AplicarMedo(StatusBase inimigoStatus)
    {
        Vector3 direcaoFuga = (inimigoStatus.transform.position - transform.position).normalized;
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoMedo)
        {
            inimigoStatus.transform.position += direcaoFuga * inimigoStatus.velocidadeMovimento * Time.deltaTime;
            tempoDecorrido += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator AplicarReducaoVelocidade(StatusBase inimigoStatus)
    {
        float velocidadeOriginal = inimigoStatus.velocidadeMovimento;
        inimigoStatus.velocidadeMovimento *= 1 - (reducaoVelocidade + (statusBase.level * 0.1f)); // Escala com nível

        yield return new WaitForSeconds(duracaoReducaoVelocidade);
        inimigoStatus.velocidadeMovimento = velocidadeOriginal;
    }

    private IEnumerator AplicarResistencia()
    {
        statusBase.armadura += resistenciaBonus;
        statusBase.resistenciaMagica += resistenciaBonus;

        yield return new WaitForSeconds(duracaoMedo);

        statusBase.armadura -= resistenciaBonus;
        statusBase.resistenciaMagica -= resistenciaBonus;
    }

    // Visualizar alcance no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ataquePoint.position, alcance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(curaPoint.position, alcance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, alcanceAssobio);
         Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(fumacentoPoint.position, alcanceFumaca);
    }
    
     void UsarDominioFumacento()
    {
        cooldownRestante = 20f - (statusBase.level * 1f); // Escala o cooldown com o nível

        // Criar a área de fumaça no ponto especificado
        GameObject areaFumaca = Instantiate(areaFumacaPrefab, fumacentoPoint.position, Quaternion.identity);
        Destroy(areaFumaca, duracaoFumaca); // Remove a fumaça após o tempo de duração

        // Aumenta a esquiva do personagem temporariamente
        StartCoroutine(AumentarEsquiva());

        // Afeta inimigos dentro da fumaça
        Collider2D[] inimigos = Physics2D.OverlapCircleAll(fumacentoPoint.position, alcanceFumaca, inimigoLayers);
        foreach (Collider2D inimigo in inimigos)
        {
            StatusBase statusInimigo = inimigo.GetComponent<StatusBase>();
            if (statusInimigo != null)
            {
                StartCoroutine(ReduzirPrecisao(statusInimigo));
            }
        }

        Debug.Log("Domínio Fumacento ativado!");
    }

    private IEnumerator AumentarEsquiva()
    {
        float esquivaOriginal = statusBase.precisao;
        statusBase.precisao += aumentoEsquiva;

        Debug.Log($"Esquiva aumentada para {statusBase.precisao}.");
        yield return new WaitForSeconds(duracaoFumaca);

        statusBase.precisao = esquivaOriginal;
        Debug.Log("Esquiva retornada ao normal.");
    }

    private IEnumerator ReduzirPrecisao(StatusBase inimigoStatus)
    {
        float precisaoOriginal = inimigoStatus.precisao;
        inimigoStatus.precisao *= 1 - reducaoPrecisao;

        Debug.Log($"Precisão do inimigo {inimigoStatus.name} reduzida para {inimigoStatus.precisao}.");
        yield return new WaitForSeconds(duracaoFumaca);

        inimigoStatus.precisao = precisaoOriginal;
        Debug.Log($"Precisão do inimigo {inimigoStatus.name} retornada ao normal.");
    }
}
