using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : MonoBehaviour
{
    public SPUM_Prefabs _prefabs; // Controlador de animações
    public float _charMS = 5f; // Velocidade do personagem
    public float jumpForce = 5f; // Força do pulo

    public enum PlayerState
    {
        idle,
        move,
        attack,
        jump,
        death,
    }
    public PlayerState _playerState = PlayerState.idle;

    private Vector2 _movement; // Entrada de movimento do teclado
    private Vector2 _goalPos; // Posição de destino
    private Rigidbody2D rb;
    private bool _isMouseMovement = false;
    private bool isAttacking = false;
    private bool isGrounded = false; // Verifica se o personagem está no chão

    [Header("Configurações de Ataque")]
    public KeyCode attackKey = KeyCode.Z; // Tecla configurável para ataque
    public AttackType attackType = AttackType.Sword;

    [Header("Configurações de Pulo")]
    public KeyCode jumpKey = KeyCode.Space; // Tecla configurável para pulo
    public Transform groundCheck; // Objeto para verificar o chão
    public LayerMask groundLayer; // Layer do chão

    public enum AttackType
    {
        Sword, // 4
        Bow,   // 5
        Magic  // 6
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Inicializa o Rigidbody2D
    }

    void Update()
    {
        HandleInput(); // Captura o movimento
        HandleJump();  // Captura o pulo
        HandleAttack(); // Captura o ataque
        HandleAnimation(); // Atualiza animações

        if (_isMouseMovement)
            HandleMouseMovement();

        // Ajuste de posição no eixo Z
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.localPosition.y * 0.01f);
    }

    void FixedUpdate()
    {
        if (_playerState == PlayerState.move)
        {
            rb.MovePosition(rb.position + _movement * _charMS * Time.fixedDeltaTime);
        }
    }

    void HandleInput()
    {
        if (isAttacking) return;

        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        if (_movement != Vector2.zero)
        {
            _playerState = PlayerState.move;
            _isMouseMovement = false;

            if (_movement.x > 0) _prefabs.transform.localScale = new Vector3(-1, 1, 1);
            else if (_movement.x < 0) _prefabs.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (!_isMouseMovement)
        {
            _playerState = PlayerState.idle;
        }
    }

    void HandleJump()
    {
        // Verifica se o personagem está no chão usando OverlapCircle
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            _playerState = PlayerState.jump;

            // Adiciona força para pular
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            // Toca a animação de "Stun" (não há animação de pulo específica no pacote)
            _prefabs.PlayAnimation(3);
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        _playerState = PlayerState.attack;

        switch (attackType)
        {
            case AttackType.Sword:
                _prefabs.PlayAnimation(4);
                break;
            case AttackType.Bow:
                _prefabs.PlayAnimation(5);
                break;
            case AttackType.Magic:
                _prefabs.PlayAnimation(6);
                break;
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        _playerState = PlayerState.idle;
    }

    void HandleAnimation()
    {
        if (isAttacking || _playerState == PlayerState.jump) return;

        switch (_playerState)
        {
            case PlayerState.idle:
                _prefabs.PlayAnimation(0);
                break;

            case PlayerState.move:
                _prefabs.PlayAnimation(1);
                break;
        }
    }

    void HandleMouseMovement()
    {
        Vector2 direction = _goalPos - rb.position;
        if (direction.sqrMagnitude > 0.1f)
        {
            rb.MovePosition(rb.position + direction.normalized * _charMS * Time.deltaTime);

            if (direction.x > 0) _prefabs.transform.localScale = new Vector3(-1, 1, 1);
            else if (direction.x < 0) _prefabs.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            _isMouseMovement = false;
            _playerState = PlayerState.idle;
        }
    }

    public void SetMovePos(Vector2 pos)
    {
        _goalPos = pos;
        _isMouseMovement = true;
        _playerState = PlayerState.move;
        _prefabs.PlayAnimation(1);
    }
}
