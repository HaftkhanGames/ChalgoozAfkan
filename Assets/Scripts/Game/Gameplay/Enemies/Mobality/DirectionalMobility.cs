using UnityEngine;

public class DirectionalMobility : EnemyMobility
{
    public enum MoveDirection { Up, Down, Left, Right }

    [Header("Movement")]
    public MoveDirection direction = MoveDirection.Right;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;
    public bool loopMovement = true;
    
    [Header("Visual")]
    public bool flipSprite = true;
    public Transform spriteTransform;
    
    [Header("Animation")]
    public Animator animator;

    private Vector3 startPoint, endPoint;
    private bool movingForward = true;
    private bool facingRight = true;
    private Vector3 lastPosition;

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        if (animator != null)
        {
            animator.SetBool("Start", true);
            animator.SetBool("Moving", true);
        }
        lastPosition = transform.position;
    }

    void Initialize()
    {
        if (spriteTransform == null) spriteTransform = transform;
        
        startPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        endPoint = startPoint + GetDirectionVector() * moveDistance;

        // فیکس: اول بررسی میکنیم که اصلاً حرکت افقی هست یا نه
        if (direction == MoveDirection.Right)
            facingRight = true;  // به راست حرکت = صورت راست
        else if (direction == MoveDirection.Left)
            facingRight = false; // به چپ حرکت = صورت چپ
        
        UpdateFacing();
        
        lastPosition = transform.position;
        
        if (animator != null)
        {
            animator.SetBool("Start", true);
            animator.SetBool("Moving", true);
        }
    }

    public override void HandleMovement()
    {
        if (enemy == null) return;

        Vector3 target = movingForward ? endPoint : startPoint;
        Vector3 nextPos = Vector3.MoveTowards(enemy.transform.position, target, moveSpeed * Time.deltaTime);
        nextPos.y = startPoint.y;
        
        enemy.transform.position = nextPos;

        // Update animation
        bool isMoving = Vector3.Distance(lastPosition, nextPos) > 0.001f;
        animator?.SetBool("Moving", isMoving);
        lastPosition = nextPos;

        // Check arrival
        if (Vector3.Distance(nextPos, target) <= 0.05f && loopMovement)
        {
            movingForward = !movingForward;

            // فقط اگه حرکت افقی بود فلیپ کن
            if (flipSprite && (direction == MoveDirection.Left || direction == MoveDirection.Right))
            {
                facingRight = !facingRight;
                UpdateFacing();
            }

            animator?.SetTrigger("Turn");
        }
    }

    void UpdateFacing()
    {
        if (spriteTransform == null) return;
        
        Vector3 scale = spriteTransform.localScale;
        // facingRight = true → scale.x مثبت (صورت به راست)
        // facingRight = false → scale.x منفی (صورت به چپ)
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        spriteTransform.localScale = scale;
    }

    Vector3 GetDirectionVector()
    {
        return direction switch
        {
            MoveDirection.Up => Vector3.forward,
            MoveDirection.Down => Vector3.back,
            MoveDirection.Left => Vector3.left,
            _ => Vector3.right
        };
    }

    void OnDrawGizmosSelected()
    {
        Vector3 start = Application.isPlaying ? startPoint : transform.position;
        Vector3 end = start + GetDirectionVector() * moveDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, end);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(start, 0.15f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(end, 0.15f);
    }
}
