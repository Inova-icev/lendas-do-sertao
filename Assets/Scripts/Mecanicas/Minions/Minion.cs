using Photon.Pun;
using UnityEngine;

public class Minions : MonoBehaviour
{
    public float speed = 2f; // Velocidade de movimento do minion
    public float fallSpeed = 2f; // Velocidade para descer caso não haja chão
    public float groundCheckDistance = 1f; // Distância para checar o chão abaixo
    public float attackRange = 1.5f; // Alcance de ataque
    public int attackDamage = 10; // Dano de ataque
    public float attackCooldown = 2f; // Tempo de recarga entre ataques
    public string enemyTag; // Tag dos inimigos que este minion deve atacar
    public LayerMask groundLayer; // Camada do chão
    public Transform waypoint; // Ponto de caminho que o minion deve alcançar

    private Transform target; // Alvo atual do minion
    private float attackTimer = 0f; // Temporizador de ataque
    private Rigidbody2D rb;
    private Vida vidaComponent; // Referência ao componente Vida para o próprio minion

    private float goldRewardRadius = 5f;
    private int goldReward = 50;
    private bool isGrounded; // Verifica se o minion está no chão
    public float obstacleCheckDistance = 1f; // Distância para checar obstáculos à frente
    private bool isClimbing; // Indica se o minion está subindo um obstáculo
    public float climbHeight = 1f; // Altura que o minion deve alcançar ao subir
    public float climbSpeed = 5f; // Velocidade para subir

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaComponent = GetComponent<Vida>();

        if (CompareTag("Left"))
        {
            int leftLayer = LayerMask.NameToLayer("Left");
            Physics2D.IgnoreLayerCollision(leftLayer, leftLayer, true); // Minions da Left ignoram uns aos outros
        }
        else if (CompareTag("Right"))
        {
            int rightLayer = LayerMask.NameToLayer("Right");
            Physics2D.IgnoreLayerCollision(rightLayer, rightLayer, true); // Minions da Right ignoram uns aos outros
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} tem uma tag desconhecida. Nenhuma colisão configurada.");
        }
        // Configurar o enemyTag com base na tag do minion
        if (CompareTag("Left"))
        {
            enemyTag = "Right";
        }
        else if (CompareTag("Right"))
        {
            enemyTag = "Left";
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} tem uma tag desconhecida. enemyTag não foi configurado.");
        }
        // Configura o waypoint inicial
        if (CompareTag("Left"))
        {
            GameObject waypointObject = GameObject.Find("WaypointRight");
            if (waypointObject != null)
            {
                waypoint = waypointObject.transform;
            }
        }
        else if (CompareTag("Right"))
        {
            GameObject waypointObject = GameObject.Find("WaypointLeft");
            if (waypointObject != null)
            {
                waypoint = waypointObject.transform;
            }
        }

        FindTarget(); // Encontra o alvo inicial ao começar
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient) // Apenas o MasterClient controla o movimento e lógica
        {
            // Atualiza temporizador de ataque
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }

            // Verifica se está no chão
            isGrounded = IsGroundBelow();
            if (!isGrounded)
            {
                // Aplica gravidade se estiver no ar
                rb.velocity = new Vector2(rb.velocity.x, -fallSpeed);
                return; // Evita movimentação horizontal enquanto está no ar
            }

            // Se não houver um alvo, procure um
            if (target == null)
            {
                FindTarget();
                if (waypoint != null)
                {
                    MoveTowardsWaypoint();
                }
            }
            else
            {
                // Se o alvo estiver fora do alcance, mova-se em direção a ele
                if (Vector2.Distance(transform.position, target.position) > attackRange)
                {
                    MoveTowardsTarget();
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    Attack();
                }
            }

            if (!isClimbing) // Não realizar movimento padrão enquanto está subindo
            {
                MoveForward();

                // Checa se há um obstáculo para subir
                if (IsObstacleAhead() && CanClimb())
                {
                    StartCoroutine(Climb());
                }
            }
            PhotonView photonView = GetComponent<PhotonView>();
            // Envia a posição sincronizada para os outros clientes
            photonView.RPC("SyncPosition", RpcTarget.Others, transform.position, rb.velocity);
        }
    }
[PunRPC]
    public void SyncPosition(Vector3 position, Vector2 velocity)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = position;
            rb.velocity = velocity;
        }
    }

    void MoveForward()
    {
        float directionMultiplier = CompareTag("Right") ? -1 : 1; 
        rb.AddForce(new Vector2(speed * directionMultiplier, 0), ForceMode2D.Force);
    }

    void MoveTowardsWaypoint()
    {
        if (waypoint != null && !isClimbing)
        {
            // Verifica se há chão abaixo ou à frente
            if (IsGroundBelow())
            {
                Vector2 position = Vector2.MoveTowards(transform.position, waypoint.position, speed * Time.deltaTime);
                rb.MovePosition(position);
            }
            else
            {
                // Aplica movimento vertical para descer
                rb.velocity = new Vector2(rb.velocity.x, -fallSpeed);
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;

            // Corrige a direção de movimento com base na tag do minion
            float directionMultiplier = CompareTag("Right") ? -1 : 1; // Minions da Right vão para a esquerda
            rb.velocity = new Vector2(direction.x * speed * directionMultiplier, rb.velocity.y);
        }
    }

        // Método para detectar obstáculos
    private bool IsObstacleAhead()
    {
        float extendedObstacleCheckDistance = 1f; // Define o comprimento do raio
        Vector2 direction = Vector2.right * (CompareTag("Left") ? 1 : -1); // Direção baseada na tag
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, extendedObstacleCheckDistance, groundLayer);

        Debug.DrawRay(transform.position, direction * extendedObstacleCheckDistance, Color.red); // Visualiza o Raycast

        return hit.collider != null;
    }


    // Verifica se há espaço acima do obstáculo para subir
    private bool CanClimb()
    {
        Vector2 direction = new Vector2(0, climbHeight); // Verifica acima
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, climbHeight, 0), Vector2.up, climbHeight, groundLayer);
        Debug.DrawRay(transform.position + new Vector3(0, climbHeight, 0), Vector2.up * climbHeight, Color.blue);
        return hit.collider == null; // Espaço livre
    }

    // Simula a subida no obstáculo
    private System.Collections.IEnumerator Climb()
    {
        isClimbing = true;

        // Desativa gravidade temporariamente
        rb.gravityScale = 0;

        // Ignorar colisões com a camada 6 (obstáculo)
        Physics2D.IgnoreLayerCollision(gameObject.layer, 6, true);

        // Calcula a posição-alvo para subir
        Vector2 targetPosition = new Vector2(
            transform.position.x + (obstacleCheckDistance * (CompareTag("Left") ? 1 : -1)),
            transform.position.y + climbHeight
        );

        // Movimenta o minion até o alvo verticalmente
        while (transform.position.y < targetPosition.y - 0.1f)
        {
            rb.velocity = new Vector2(0, climbSpeed); // Apenas sobe verticalmente
            yield return null;
        }

        // Garante que o movimento vertical seja interrompido antes do movimento horizontal
        rb.velocity = Vector2.zero;

        // Movimenta o minion horizontalmente para completar a subida
        while (Mathf.Abs(transform.position.x - targetPosition.x) > 0.1f)
        {
            rb.velocity = new Vector2(speed * (CompareTag("Left") ? 1 : -1), 0);
            yield return null;
        }

        // Garante que o minion fique exatamente na posição-alvo
        transform.position = targetPosition;

        // Restaura a gravidade após a subida
        rb.gravityScale = 10;

        // Reativa as colisões com a camada 6 (obstáculo)
        Physics2D.IgnoreLayerCollision(gameObject.layer, 6, false);

        isClimbing = false;
        rb.gravityScale = 6;
    }

    
    // Procurar o alvo mais próximo (minion ou torre inimiga)
    void FindTarget()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        Collider2D closestTarget = null;

        foreach (Collider2D potentialTarget in potentialTargets)
        {
            // Verifica se o alvo é inimigo (minion ou torre)
            if ((CompareTag("Left") && potentialTarget.CompareTag("Right")) ||
                (CompareTag("Right") && potentialTarget.CompareTag("Left")))
            {
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

        // Define o alvo se encontrado
        if (closestTarget != null)
        {
            target = closestTarget.transform;
            Debug.Log($"{gameObject.name} encontrou o alvo {closestTarget.name}");
        }
            else
        {
            target = null; // Nenhum alvo encontrado
        }
    }

    // Método de ataque
    private void Attack()
    {
        // Verifica se o cooldown acabou
        if (attackTimer <= 0f && target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange) 
            {
                Vida targetVida = target.GetComponent<Vida>();
                if (targetVida != null)
                {
                    targetVida.TakeDamage(attackDamage); 
                    Debug.Log($"{gameObject.name} atacou {target.name} causando {attackDamage} de dano.");
                    attackTimer = attackCooldown; 
                }
            }
        }
    }

    // Verifica se há chão abaixo do minion

    private bool IsGroundBelow()
    {
        Vector2 origin = transform.position;
        float extendedGroundCheckDistance = 5f; // Define o comprimento do raio
        RaycastHit2D hitCenter = Physics2D.Raycast(origin, Vector2.down, extendedGroundCheckDistance, groundLayer);

        Vector2 forwardOrigin = origin + new Vector2(CompareTag("Left") ? 0.5f : -0.5f, 0); // Ponto à frente
        RaycastHit2D hitForward = Physics2D.Raycast(forwardOrigin, Vector2.down, extendedGroundCheckDistance, groundLayer);

        Debug.DrawRay(origin, Vector2.down * extendedGroundCheckDistance, Color.green); // Raio central
        Debug.DrawRay(forwardOrigin, Vector2.down * extendedGroundCheckDistance, Color.blue); // Raio à frente

        // Retorna true se qualquer um dos Raycasts detectar o chão
        return hitCenter.collider != null || hitForward.collider != null;
    }

    [PunRPC]
    public void TakeDamageRPC(int damage)
    {
        if (vidaComponent != null)
        {
            vidaComponent.TakeDamage(damage);
        }
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    // Função para receber danopublic void TakeDamage(int damage)
    public void TakeDamage(int damage)
    {
        PhotonView photonView = GetComponent<PhotonView>();
        if (photonView != null && photonView.IsMine)
        {
            photonView.RPC("TakeDamageRPC", RpcTarget.All, damage);
        }
    }

    public void OnDeath()
    {
        if (PhotonNetwork.IsMasterClient) // Apenas o MasterClient remove os minions
        {
            // Encontre todos os jogadores inimigos em um raio
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, goldRewardRadius);
            foreach (Collider2D enemy in enemiesInRange)
            {
                // Verifica se o objeto é um jogador inimigo
                Player player = enemy.GetComponent<Player>();
                if (player != null && enemy.CompareTag(enemyTag))
                {
                    player.GainGold(goldReward); // Distribui ouro ao jogador inimigo
                    Debug.Log($"Jogador {player.name} ganhou {goldReward} de ouro pela morte do {gameObject.name}.");
                }
            }

            // Destrói o minion em todos os clientes
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
