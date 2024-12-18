using UnityEngine;
using System.Collections.Generic;
using TMPro;
using ManagmentScripts;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public LayerMask groundLayer;
    public LayerMask platformLayer; // Nova camada para as plataformas que podem ser atravessadas
    private Rigidbody2D rb;
    private bool isGrounded;

    public int gold = 0;

    private bool controlEnabled = true;

    private float currentSpeed;
    private float damageMultiplier = 1f;

    public int baseDamage = 10;

    public List<Item> items;

    private bool isSpeedBuffed = false;
    private bool isDamageBuffed = false;

    public float attackRange = 1f;
    public float attackCooldown = 0.5f; // Intervalo entre os ataques

    private float lastAttackTime = 0f;
    public float acceleration = 20f; // Aceleração ao mover
    public float deceleration = 10f; // Desaceleração ao parar

    public float jumpHoldTime = 0.5f; // Tempo máximo de sustentação do pulo
    private float jumpHoldTimer = 0f;

    private float buffStartTimeDamage = 0f;
    private float buffStartTimeSpeed = 0f;
    private float buffDuration = 10f;

    private PanelManager panelManager;

    public GameObject cameraPrefab; // Prefab da câmera que será instanciada para este jogador

    private CameraFollow cameraFollow; // Referência ao script da câmera
    private PhotonView photonView;
    private Vida vidaComponent;

    void Start()
    {
        panelManager = FindAnyObjectByType<PanelManager>();
        vidaComponent = GetComponent<Vida>();
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>(); 
        SetupCamera();
        if (photonView.IsMine)
        {
            string teamTag = GameManager.Instance.GetTeamTag();
            photonView.RPC("SyncTagRPC", RpcTarget.AllBuffered, teamTag);
        }
    }
    [PunRPC]
    private void SyncTagRPC(string teamTag)
    {
        gameObject.tag = teamTag;
        if (teamTag == "Right"){
            gameObject.layer = 9;
        }
        else{
            gameObject.layer = 8;
        }
        Debug.Log($"Tag sincronizada para {teamTag} em {gameObject.name}");
    }
    private void SetupCamera()
    {
        PhotonView photonView = GetComponent<PhotonView>();

        // Certifique-se de que a câmera só seja criada para o jogador local
        if (photonView.IsMine)
        {
            if (cameraPrefab != null)
            {
                GameObject cameraInstance = Instantiate(cameraPrefab);
                cameraFollow = cameraInstance.GetComponent<CameraFollow>();

                if (cameraFollow != null)
                {
                    // Atribui este jogador como alvo da câmera
                    cameraFollow.AssignPlayer(transform);
                    Debug.Log($"Câmera criada para o jogador local: {gameObject.name}");
                }
                else
                {
                    Debug.LogError("O prefab da câmera não possui o script CameraFollow.");
                }
            }
            else
            {
                Debug.LogError("Prefab de câmera não está configurado no script do Player.");
            }
        }
        else
        {
            Debug.Log($"Câmera não criada para {gameObject.name}, pois não pertence ao jogador local.");
        }
    }

    void Update()
    {
        // Verifica se este objeto pertence ao jogador local
        PhotonView photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            return; // Ignora inputs para jogadores que não são locais
        }

        if (controlEnabled)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float targetSpeed = moveX * speed;

            // Aceleração e desaceleração suaves
            if (moveX != 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
            }

            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

            CheckGround();

            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Pulo detectado!");
                Jump();
                TryJumpThroughPlatform();
            }

            HandleFalling();

            if (Input.GetKey(KeyCode.S))
            {
                DropThroughPlatform();
            }

            if (Input.GetMouseButtonDown(0)) // Botão esquerdo do mouse
            {
                Attack();
            }

            if (isDamageBuffed && Time.time - buffStartTimeDamage >= buffDuration)
            {
                RemoveDamageBuff(2f);
            }

            if (isSpeedBuffed && Time.time - buffStartTimeSpeed >= buffDuration)
            {
                RemoveBuff();
            }
        }
    }



    void Move()
    {
        // Determina a direção do movimento com base nas entradas do jogador
        float moveX = Input.GetAxisRaw("Horizontal"); // -1 para esquerda, 1 para direita, 0 para parado

        // Calcula a velocidade alvo com base na entrada do jogador e na velocidade atual
        float targetSpeed = moveX * speed;

        // Suaviza a transição entre parada e movimento
        if (moveX != 0)
        {
            // Aceleração para a direção alvo
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Desaceleração para 0 quando não há entrada de movimento
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        // Atualiza a velocidade do rigidbody mantendo a componente Y
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
    }


    void Jump()
    {
        if (jumpHoldTimer < jumpHoldTime)  // Verifica se o jogador está pressionando o botão de pulo
        {
            jumpHoldTimer += Time.deltaTime;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else
        {
            jumpHoldTimer = jumpHoldTime;  // Limita o tempo máximo de pulo
        }
    }

    void CheckGround()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 1f, groundLayer);

        // Raycasts extras podem ajudar a garantir a detecção correta
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - 0.25f, transform.position.y - 0.5f), Vector2.down, 0.5f, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(transform.position.x + 0.25f, transform.position.y - 0.5f), Vector2.down, 0.5f, groundLayer);

        isGrounded = hit.collider != null || hitLeft.collider != null || hitRight.collider != null;
    }


    void HandleFalling()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }


    void TryJumpThroughPlatform()
    {
        // Ajustar a posição de origem para ser um pouco acima do topo do colisor do personagem
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - GetComponent<Collider2D>().bounds.extents.y / 2);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, 3.5f, platformLayer);
        if (hit.collider != null)
        {
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;
            Invoke("ReactivateCollider", 0.8f); // Reativa o colisor rapidamente
        }
        else
        {
            Debug.Log("Nenhuma plataforma detectada acima para subir");
        }
    }


    void DropThroughPlatform()
    {
        // Obtém a posição ligeiramente abaixo do centro do jogador para detectar plataformas
        Vector2 feetPosition = new Vector2(transform.position.x, transform.position.y - 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(feetPosition, Vector2.down, 1f, platformLayer);

        // Verifica se o jogador está sobre uma plataforma que é da camada 6
        if (hit.collider != null && hit.collider.gameObject.layer == 6)
        {
            Collider2D collider = GetComponent<Collider2D>();
            // Desativa temporariamente o colisor do personagem
            collider.enabled = false;
            Invoke("ReactivateCollider", 0.2f); // Reativa o colisor após 0.5 segundos
        }
        else
        {
            Debug.Log("Não está em uma plataforma que permite descer.");
        }
    }

    void ReactivateCollider()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    [PunRPC]
    public void ApplyBuffRPC(float multiplier)
    {
        if (!isSpeedBuffed)
        {
            currentSpeed = speed * multiplier;
            isSpeedBuffed = true;
            buffStartTimeSpeed = Time.time;
            Debug.Log($"Buff de velocidade aplicado a {gameObject.name}. Velocidade: {currentSpeed}");
        }
    }

    [PunRPC]
    public void RemoveBuffRPC()
    {
        if (isSpeedBuffed)
        {
            currentSpeed = speed;
            isSpeedBuffed = false;
            Debug.Log($"Buff de velocidade removido de {gameObject.name}. Velocidade: {currentSpeed}");
        }
    }
    public void ApplyBuff(float multiplier)
    {
        photonView.RPC("ApplyBuffRPC", RpcTarget.All, multiplier);
    }

    public void RemoveBuff()
    {
        photonView.RPC("RemoveBuffRPC", RpcTarget.All);
    }
    [PunRPC]
    public void ApplyDamageBuffRPC(float multiplier)
    {
        if (!isDamageBuffed)
        {
            damageMultiplier *= multiplier;
            isDamageBuffed = true;
            buffStartTimeDamage = Time.time;
            Debug.Log($"Buff de dano aplicado a {gameObject.name}. Multiplicador de dano: {damageMultiplier}");
        }
    }

    [PunRPC]
    public void RemoveDamageBuffRPC(float multiplier)
    {
        if (isDamageBuffed)
        {
            damageMultiplier /= multiplier;
            isDamageBuffed = false;
            Debug.Log($"Buff de dano removido de {gameObject.name}. Multiplicador de dano: {damageMultiplier}");
        }
    }
    public void ApplyDamageBuff(float multiplier)
    {
        photonView.RPC("ApplyDamageBuffRPC", RpcTarget.All, multiplier);
    }

    public void RemoveDamageBuff(float multiplier)
    {
        photonView.RPC("RemoveDamageBuffRPC", RpcTarget.All, multiplier);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} recebeu {damage} de dano. AAAAAAAAAA");
        photonView.RPC("TakeDamageRPC", RpcTarget.All, damage);
    }

    [PunRPC]
    public void TakeDamageRPC(int damage)
    {
        Debug.Log($"{gameObject.name} recebeu {damage} de dano.");
        if (vidaComponent != null)
        {
            vidaComponent.TakeDamage(damage, 0, true); 
        }
    }


    public void GainGold(int amount)
    {
        gold += amount;
        Debug.Log($"Jogador ganhou {amount} de ouro. Total: {gold}");
    }

    public void BuyItem(Item item)
    {
        if (gold >= item.price)
        {
            gold -= item.price;
            items.Add(item);
            ApplyPermanentEffect(item);
            Debug.Log($"Você comprou {item.name}! Ouro restante: {gold}");
        }
        else
        {
            Debug.Log("Você não tem ouro suficiente para comprar este item.");
        }
    }

    private void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time; // Atualiza o tempo do último ataque

            // Realiza a lógica de encontrar o alvo e atacar
            FindTargetAndAttack();
        }
        else
        {
            Debug.Log("Aguardando recarga do ataque...");
        }
    }


    private void FindTargetAndAttack()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        Collider2D closestTarget = null;

        foreach (Collider2D potentialTarget in potentialTargets)
        {
            if ((CompareTag("Left") && potentialTarget.CompareTag("Right")) ||
                (CompareTag("Right") && potentialTarget.CompareTag("Left")))
            {
                Debug.Log($"Analisando alvo: {potentialTarget.name}, Tag: {potentialTarget.tag}");
                Vida vidaComponent = potentialTarget.GetComponent<Vida>();
                if (vidaComponent != null)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, potentialTarget.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = potentialTarget;
                    }
                }
            }
        }

        if (closestTarget != null)
        {
            PhotonView targetPhotonView = closestTarget.GetComponent<PhotonView>();
            if (targetPhotonView == null)
            {
                Debug.LogWarning($"O alvo {closestTarget.name} não possui um PhotonView!");
            }
            if (targetPhotonView != null)
            {
                int damageToDeal = Mathf.RoundToInt(baseDamage * damageMultiplier);
                targetPhotonView.RPC("TakeDamageRPC", RpcTarget.All, damageToDeal, 1);
                Debug.Log($"{gameObject.name} atacou {closestTarget.name} causando {damageToDeal} de dano");
            }
        }
    }

    [PunRPC]
    private void SyncRespawn(Vector3 position, Quaternion rotation)
    {
        // Reposiciona e redefine a rotação do jogador
        transform.position = position;
        transform.rotation = rotation;

        Debug.Log($"Player {gameObject.name} foi respawnado em {position}");
    }
    public void ApplyPermanentEffect(Item item)
    {
        switch (item.type)
        {
            case ItemType.SpeedBuff:
                ApplyPermanentSpeedBoost(item.effectValue);
                break;
            case ItemType.DamageBuff:
                ApplyPermanentDamageBoost(item.effectValue);
                break;
            case ItemType.HealthBuff:
                ApplyPermanentHealthBoost(item.effectValue);
                break;
            default:
                Debug.LogWarning("Tipo de item desconhecido.");
                break;
        }
    }

    // Aumenta permanentemente a velocidade
    private void ApplyPermanentSpeedBoost(float value)
    {
        speed += value;
        currentSpeed = speed; // Atualiza o valor atual da velocidade
        Debug.Log($"Velocidade permanentemente aumentada para: {speed}");
    }

    // Aumenta permanentemente o dano
    private void ApplyPermanentDamageBoost(float value)
    {
        damageMultiplier += value;
        Debug.Log($"Dano permanentemente aumentado para: {damageMultiplier}");
    }

    // Aumenta permanentemente a vida máxima e restaura a vida atual
    private void ApplyPermanentHealthBoost(float value)
    {
        vidaComponent.maxHealth += value;
        vidaComponent.currentHealth = Mathf.Clamp(vidaComponent.currentHealth + value, 0, vidaComponent.maxHealth);
        Debug.Log($"Vida permanentemente aumentada para: {vidaComponent.maxHealth}. Vida atual: {vidaComponent.currentHealth}");
    }
}