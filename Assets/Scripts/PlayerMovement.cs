using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 9f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Jump & Physics Settings")]
    public float jumpForce = 80f; // Much higher jump force
    public float baseGravity = 3.5f;
    public float fallGravityMultiplier = 1.5f;

    [Header("Dash Settings")]
    public float dashSpeed = 22f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.6f;

    [Header("Combat & Damage Settings")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.25f;
    public float animationFps = 18f; // Snappy 18 FPS animation speed

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool isGrounded;
    private bool isCrouching;
    private bool isDashing;
    private bool isAttacking;
    private bool isSpecialAttacking;
    private bool isHurt;
    private bool canDash = true;
    private int facingDirection = 1;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private Vector3 originalScale;

    // Sprite Animation Collections
    private Sprite[] idleSprites;          // Row 0
    private Sprite[] runSprites;           // Row 1
    private Sprite[] crouchSprites;        // Row 2
    private Sprite[] attackSprites;        // Row 3 (Standard Slash Attack)
    private Sprite[] jumpSprites;          // Row 4
    private Sprite[] fallSprites;          // Row 5
    private Sprite[] hurtSprites;          // Row 7 (Take Damage / Knockback)
    private Sprite[] dashSprites;          // Row 8 (Dash Burst)
    private Sprite[] specialAttackSprites; // Merged Row 9 + Row 10 (Special Beam/Portal Attack)

    private Sprite[] currentAnim;
    private float animTimer;
    private int currentAnimFrame;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;

        if (rb != null)
        {
            rb.gravityScale = baseGravity;
            rb.freezeRotation = true;
        }

        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(0.8f, 1.2f);
            boxCollider.offset = new Vector2(0f, 0.6f);
            originalColliderSize = boxCollider.size;
            originalColliderOffset = boxCollider.offset;
        }

        Debug.Log($"PlayerMovement Awake: jumpForce={jumpForce}, gravityScale={baseGravity}");
        LoadSatyrSprites();
    }

    private void LoadSatyrSprites()
    {
        Object[] allAssets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/SATYR_sprite_sheet/SPRITE_SHEET.png");
        if (allAssets == null || allAssets.Length == 0) return;

        idleSprites = LoadRowSprites(allAssets, 0);
        runSprites = LoadRowSprites(allAssets, 1);
        crouchSprites = LoadRowSprites(allAssets, 2);
        attackSprites = LoadRowSprites(allAssets, 3);
        jumpSprites = LoadRowSprites(allAssets, 4);
        fallSprites = LoadRowSprites(allAssets, 5);
        hurtSprites = LoadRowSprites(allAssets, 7);
        dashSprites = LoadRowSprites(allAssets, 8);

        // Merged special attack rows
        var specialList = new System.Collections.Generic.List<Sprite>();
        specialList.AddRange(LoadRowSprites(allAssets, 9));
        specialList.AddRange(LoadRowSprites(allAssets, 10));
        specialAttackSprites = specialList.ToArray();

        SetAnimation(idleSprites);
    }

    private Sprite[] LoadRowSprites(Object[] assets, int row)
    {
        var list = new System.Collections.Generic.List<Sprite>();
        string prefix = $"Satyr_R{row}_C";
        foreach (Object obj in assets)
        {
            if (obj is Sprite s && s.name.StartsWith(prefix))
                list.Add(s);
        }
        list.Sort((a, b) => GetColIndex(a.name).CompareTo(GetColIndex(b.name)));
        return list.ToArray();
    }

    private int GetColIndex(string name)
    {
        int cIdx = name.IndexOf("_C");
        if (cIdx >= 0 && int.TryParse(name.Substring(cIdx + 2), out int val))
            return val;
        return 0;
    }

    void Update()
    {
        if (isDashing || isHurt) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput > 0) facingDirection = 1;
        else if (moveInput < 0) facingDirection = -1;

        if (spriteRenderer != null)
            spriteRenderer.flipX = facingDirection < 0;

        // Crouch
        bool crouchHeld = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftControl);
        if (crouchHeld && isGrounded && !isAttacking && !isSpecialAttacking) {
            if (!isCrouching) StartCrouch();
        } else if (isCrouching) {
            StopCrouch();
        }

        // Jump
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            && isGrounded && !isCrouching && !isAttacking && !isSpecialAttacking)
            Jump();

        // Dash
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.E))
            && canDash && !isAttacking && !isSpecialAttacking)
            StartCoroutine(PerformDash());

        // Standard Attack
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
            && !isAttacking && !isSpecialAttacking)
            StartCoroutine(PerformAttack());

        // Special Attack
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
            && !isAttacking && !isSpecialAttacking)
            StartCoroutine(PerformSpecialAttack());

        UpdateAnimationState();
        AnimateSprite();
    }

    void FixedUpdate()
    {
        if (isDashing || isHurt) return;

        // Faster fall
        if (rb.linearVelocity.y < 0 && !isGrounded)
            rb.gravityScale = baseGravity * fallGravityMultiplier;
        else
            rb.gravityScale = baseGravity;

        if (isAttacking || isSpecialAttacking) return;

        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        CheckGrounded();
    }

    private void UpdateAnimationState()
    {
        if (isHurt) SetAnimation(hurtSprites);
        else if (isSpecialAttacking) SetAnimation(specialAttackSprites);
        else if (isAttacking) SetAnimation(attackSprites);
        else if (isDashing) SetAnimation(dashSprites);
        else if (!isGrounded)
            SetAnimation(rb.linearVelocity.y > 0.1f ? jumpSprites : fallSprites);
        else if (isCrouching) SetAnimation(crouchSprites);
        else if (Mathf.Abs(moveInput) > 0.1f) SetAnimation(runSprites);
        else SetAnimation(idleSprites);
    }

    private void SetAnimation(Sprite[] newAnim)
    {
        if (newAnim == null || newAnim.Length == 0) return;
        if (currentAnim != newAnim)
        {
            currentAnim = newAnim;
            currentAnimFrame = 0;
            animTimer = 0f;
            if (spriteRenderer != null && currentAnim.Length > 0)
                spriteRenderer.sprite = currentAnim[0];
        }
    }

    private void AnimateSprite()
    {
        if (currentAnim == null || currentAnim.Length == 0 || spriteRenderer == null) return;
        animTimer += Time.deltaTime;
        if (animTimer >= 1f / animationFps)
        {
            animTimer -= 1f / animationFps;
            currentAnimFrame = (currentAnimFrame + 1) % currentAnim.Length;
            spriteRenderer.sprite = currentAnim[currentAnimFrame];
        }
    }

    private void Jump()
    {
        Debug.Log($"Jumping with force {jumpForce}");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    private void StartCrouch()
    {
        isCrouching = true;
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.6f);
            boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y * 0.2f));
        }
    }

    private void StopCrouch()
    {
        isCrouching = false;
        if (boxCollider != null)
        {
            boxCollider.size = originalColliderSize;
            boxCollider.offset = originalColliderOffset;
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float dashDir = moveInput != 0 ? moveInput : facingDirection;
        rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        SetAnimation(attackSprites);
        float attackDuration = attackSprites.Length / animationFps;
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    private IEnumerator PerformSpecialAttack()
    {
        isSpecialAttacking = true;
        SetAnimation(specialAttackSprites);
        float attackDuration = specialAttackSprites.Length / animationFps;
        yield return new WaitForSeconds(attackDuration);
        isSpecialAttacking = false;
    }

    public void TakeDamage(Vector2 damageSourcePosition)
    {
        if (isHurt) return;
        StartCoroutine(PerformHurt(damageSourcePosition));
    }

    private IEnumerator PerformHurt(Vector2 damageSourcePosition)
    {
        isHurt = true;
        SetAnimation(hurtSprites);
        float knockbackDirX = transform.position.x >= damageSourcePosition.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(knockbackDirX * knockbackForce, jumpForce * 0.5f);
        yield return new WaitForSeconds(knockbackDuration);
        isHurt = false;
    }

    private void CheckGrounded()
    {
        if (boxCollider == null) return;
        Vector2 boxCenter = (Vector2)transform.position + boxCollider.offset + Vector2.down * (boxCollider.size.y * 0.5f + 0.05f);
        Vector2 boxSize = new Vector2(boxCollider.size.x * 0.9f, 0.1f);
        var colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);
        isGrounded = false;
        foreach (var col in colliders)
            if (col != null && col.gameObject != gameObject && IsTargetObject(col.gameObject, "Ground"))
                { isGrounded = true; break; }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsTargetObject(collision.gameObject, "Ground")) isGrounded = true;
        if (IsTargetObject(collision.gameObject, "Hazard") || collision.gameObject.name.Contains("Laser") || collision.gameObject.name.Contains("Trap"))
            TakeDamage(collision.contacts[0].point);
    }

    private bool IsTargetObject(GameObject go, string nameOrTag)
    {
        if (go == null) return false;
        if (go.name.Contains(nameOrTag)) return true;
        if (go.CompareTag("Untagged")) return false;
        try { return go.CompareTag(nameOrTag); } catch { return false; }
    }
}