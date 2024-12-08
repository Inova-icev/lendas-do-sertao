using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Habilidades : MonoBehaviour
{
    // Atributos do personagem
    public float vidaBase = 600f;
    public float danoBase = 60f;
    public float vidaAtual;

    // Configuração do Mapinguari - Habilidade Q (Garrada)
    private int nivelGarrada = 1; 
    private float[] danoGarradaPorNivel = { 0, 50, 60, 70, 80, 90 };
    private float[] cooldownGarradaPorNivel = { 0, 6, 5, 4, 3, 2 }; 
    private bool habilidadePronta = true;

    // Configuração do Mapinguari - Habilidade W (Pelagem Resistente)
    private int nivelEscudo = 1; 
    private float[] escudoPorNivel = { 0, 100, 200, 300, 400, 500 };
    private float[] cooldownEscudoPorNivel = { 0, 14, 13, 11, 11, 10 };
    private bool escudoPronto = true;
    private float valorEscudoAtual = 0;

    // Configuração do Mapinguari - Habilidade E (Muro de Lama)
    public float duracao = 5f; 
    public int nivelLentidao = 1; 
    public float[] porcentagemLentidaoPorNivel = { 0, 40f, 42f, 44f, 46f, 48f }; 
    // Configuração do Mapinguari - Habilidade R (Fúria)
    private int nivelFuria = 1; 
    public float velocidadeFuria = 10f;
    private float[] danoFuriaPorNivel = { 0, 450, 550, 650 }; 
    private float[] duracaoAtordoamentoPorNivel = { 0, 1f, 1.5f, 2f };
    private bool furiaAtivada = false;
    private List<Inimigo> inimigosArrastados = new List<Inimigo>();
    private void Start()
    {
        vidaAtual = vidaBase; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MapinguariQ();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            MapinguariW();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            MapinguariE(); 
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            MapinguariR();
        }
    }

    public void MapinguariQ()
    {
        if (nivelGarrada > 0 && nivelGarrada <= 5 && habilidadePronta)
        {
            float danoHabilidade = danoGarradaPorNivel[nivelGarrada]
                                   + (danoBase * 1.1f)
                                   + (vidaBase * 0.03f);

            StartCoroutine(CooldownGarrada());
        }
    }

    private IEnumerator CooldownGarrada()
    {
        habilidadePronta = false;
        yield return new WaitForSeconds(cooldownGarradaPorNivel[nivelGarrada]);
        habilidadePronta = true;
    }

    private void AplicarDano(GameObject alvo, float dano)
    {
        Inimigo inimigo = alvo.GetComponent<Inimigo>();
        if (inimigo != null)
        {
            inimigo.ReceberDano(dano);
        }
    }

    public void MapinguariW()
    {
        if (nivelEscudo > 0 && nivelEscudo <= 5 && escudoPronto) 
        {
            float valorEscudo = escudoPorNivel[nivelEscudo] + (vidaBase * 0.05f); 
            AplicarEscudo(valorEscudo);
            StartCoroutine(CooldownEscudo());
        }
    }

    private IEnumerator CooldownEscudo()

    {
        Debug.Log($"Cooldown da habilidade W iniciado: {cooldownEscudoPorNivel[nivelEscudo]} segundos.");
        escudoPronto = false;
        yield return new WaitForSeconds(cooldownEscudoPorNivel[nivelEscudo]);
        escudoPronto = true;
    }

    private void AplicarEscudo(float valorEscudo)
    {
        valorEscudoAtual = valorEscudo; 
    }

    public void MapinguariE() 
    {
        StartCoroutine(AtivarLentidao()); 
    }

    private IEnumerator AtivarLentidao()
    {
        Inimigo[] inimigos = FindObjectsOfType<Inimigo>();
        
        foreach (Inimigo inimigo in inimigos)
        {
            AplicarLentidao(inimigo);
        }

        
        yield return new WaitForSeconds(duracao);

        foreach (Inimigo inimigo in inimigos)
        {
            RemoverLentidao(inimigo);
        }
    }

    private void AplicarLentidao(Inimigo inimigo)
    {
        if (inimigo != null)
        {
            float porcentagemLentidao = porcentagemLentidaoPorNivel[nivelLentidao];
            inimigo.Velocidade *= (1 - porcentagemLentidao / 100);
        }
          Debug.Log("Lentidão aplicada nos inimigos.");
    }

    private void RemoverLentidao(Inimigo inimigo)
    {
        if (inimigo != null)
        {
            float porcentagemLentidao = porcentagemLentidaoPorNivel[nivelLentidao];
            inimigo.Velocidade /= (1 - porcentagemLentidao / 100);
        }
         Debug.Log("Lentidão removida dos inimigos.");
    }

    public void MapinguariR(){
        Debug.Log("MapinguariR ativada. Avançando com Fúria.");
        StartCoroutine(AvancarComFuria());
    }
    private IEnumerator AvancarComFuria()
    {
         Debug.Log("Iniciando avanço com Fúria...");
        List<Inimigo> inimigosArrastados = new List<Inimigo>();
        float distanciaMaxima = 10f;
        Vector3 direcao = transform.forward;
        while (distanciaMaxima > 0)
        {
            transform.position += direcao * velocidadeFuria * Time.deltaTime;
            distanciaMaxima -= velocidadeFuria * Time.deltaTime;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
            foreach (Collider collider in colliders)
            {
                Inimigo inimigo = collider.GetComponent<Inimigo>();
                if (inimigo != null && !inimigosArrastados.Contains(inimigo))
                {
                    inimigosArrastados.Add(inimigo);
                    inimigo.Atordoar(duracaoAtordoamentoPorNivel[nivelFuria]);
                      Debug.Log($"Inimigo {inimigo.name} atordoado.");
                }
            }
            yield return null;
        }
        foreach (Inimigo inimigo in inimigosArrastados)
        {
            inimigo.ReceberDano(danoFuriaPorNivel[nivelFuria]);
            Debug.Log($"Inimigo {inimigo.name} recebeu {danoFuriaPorNivel[nivelFuria]} de dano.");
        }
        
    }

    private bool ColisaoComSuperficie()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            return hit.collider.CompareTag("Superficie"); 
        }
        return false;
    }
    private void AplicarDanoEAtordoamento()
    {   
        foreach (Inimigo inimigo in inimigosArrastados)
        {
            float dano = danoFuriaPorNivel[nivelFuria];
            float duracaoAtordoamento = duracaoAtordoamentoPorNivel[nivelFuria];
            inimigo.ReceberDano(dano);
            inimigo.Atordoar(duracaoAtordoamento);
        }
        inimigosArrastados.Clear();
    }
}

