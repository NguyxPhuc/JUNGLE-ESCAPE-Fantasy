using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Điều khiển nhân vật platformer 2D.
/// Yêu cầu: Rigidbody2D, CapsuleCollider2D, Animator (tuỳ chọn)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    // ===================== MOVEMENT =====================
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _fallMultiplier = 2.5f;    // Rơi nhanh hơn
    [SerializeField] private float _lowJumpMultiplier = 2f;   // Nhảy thấp khi thả phím sớm

    // ===================== GROUND CHECK =====================
    [Header("Ground Detection")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckRadius = 0.1f;

    // ===================== VISUAL =====================
    [Header("Visual")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;  // Optional

    // ===================== PRIVATE STATE =====================
    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _isJumpPressed;
    private float _horizontalInput;

    // Animator parameter hashes (performance)
    private static readonly int ANIM_SPEED = Animator.StringToHash("Speed");
    private static readonly int ANIM_ISGROUND = Animator.StringToHash("IsGrounded");
    private static readonly int ANIM_JUMP = Animator.StringToHash("Jump");

    // ===================== UNITY LIFECYCLE =====================
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Tự tìm SpriteRenderer nếu chưa gán
        if (_spriteRenderer == null)
            TryGetComponent(out _spriteRenderer);

        // Tự tìm Animator nếu chưa gán
        if (_animator == null)
            TryGetComponent(out _animator);
    }

    private void Update()
    {
        CheckGrounded();
        FlipSprite();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Move();
        ApplyBetterJumpGravity();
    }

    // ===================== INPUT CALLBACKS (New Input System) =====================

    /// <summary>Gọi từ PlayerInput component - action "Move"</summary>
    public void OnMove(InputValue value)
    {
        _horizontalInput = value.Get<Vector2>().x;
    }

    /// <summary>Gọi từ PlayerInput component - action "Jump"</summary>
    public void OnJump(InputValue value)
    {
        _isJumpPressed = value.isPressed;

        if (_isJumpPressed && _isGrounded)
            Jump();
    }

    // ===================== MOVEMENT LOGIC =====================
    private void Move()
    {
        _rb.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rb.linearVelocity.y);
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);

        if (_animator != null)
            _animator.SetTrigger(ANIM_JUMP);
    }

    /// <summary>
    /// Cải thiện feel khi nhảy:
    /// - Rơi nhanh hơn (fallMultiplier)
    /// - Nhả phím nhảy sớm → nhảy thấp hơn
    /// </summary>
    private void ApplyBetterJumpGravity()
    {
        if (_rb.linearVelocity.y < 0f)
        {
            // Đang rơi xuống
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (_rb.linearVelocity.y > 0f && !_isJumpPressed)
        {
            // Đang nhảy lên nhưng đã thả phím
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    // ===================== GROUND CHECK =====================
    private void CheckGrounded()
    {
        if (_groundCheck == null) return;

        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
    }

    // ===================== VISUAL =====================
    private void FlipSprite()
    {
        if (_spriteRenderer == null) return;

        if (_horizontalInput > 0.01f)
            _spriteRenderer.flipX = false;
        else if (_horizontalInput < -0.01f)
            _spriteRenderer.flipX = true;
    }

    private void UpdateAnimator()
    {
        if (_animator == null) return;

        _animator.SetFloat(ANIM_SPEED, Mathf.Abs(_horizontalInput));
        _animator.SetBool(ANIM_ISGROUND, _isGrounded);
    }

    // ===================== DEBUG =====================
    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null) return;

        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }
}