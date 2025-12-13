using UnityEngine;

/// <summary>
/// Continuous directional movement.
/// Enemy moves infinitely in one direction
/// Stops ONLY when player enters attack range.
/// </summary>
public class DirectionalMobility : EnemyMobility
{
    public enum MoveDirection { Up, Down, Left, Right }

    #region Inspector Fields

    [Header("🚶 Movement")]
    public MoveDirection direction = MoveDirection.Right;
    public bool flipSprite = true;

    [Header("📏 Stop Conditions")]
    [Tooltip("Stop moving when player is inside this range")]
    public float stopRange = 6f;

    [Header("⚔️ One-Shot Enemies")]
    [Tooltip("If true, enemy will stop only once to attack")]
    public bool stopOnlyOnce = false;

    private bool stopConsumed;

    [Header("🧍 Visual")]
    public Transform spriteTransform;

    [Header("🎬 Animator")] 
    public Animator animator;
    public string movingBool = "Moving";
    public string attackBool = "Attack";

    #endregion

    #region Private Fields

    private Vector3 moveDir;
    private bool facingRight;
    private bool isHorizontal;
    private bool isStoppedByPlayer;

    private Vector3 lastPosition;

    #endregion

    #region Initialization

    public override void Initialize(EnemyBase baseEnemy)
    {
        base.Initialize(baseEnemy);

        animator?.SetBool(movingBool, true);
        animator?.SetBool(attackBool, false);
        if (spriteTransform == null)
            spriteTransform = transform;

        moveDir = GetDirectionVector(direction).normalized;
        isHorizontal = direction == MoveDirection.Left || direction == MoveDirection.Right;

        if (isHorizontal)
        {
            facingRight = direction == MoveDirection.Right;
            UpdateFacing();
        }

        lastPosition = transform.position;
        animator?.SetBool(movingBool, true);
    }

    private void OnEnable()
    {
        lastPosition = transform.position;
        animator?.SetBool(movingBool, true);
        animator?.SetBool(attackBool, false); // ✅
    }

    #endregion

    #region Movement Logic

    public override void HandleMovement()
    {
        if (enemy == null) return;

        HandleStopByRange();

        if (isStoppedByPlayer)
            return;

        Vector3 nextPos = transform.position +
                          moveDir * moveSpeed * Time.deltaTime;

        transform.position = nextPos;
    }


    private void HandleStopByRange()
    {
        if (!HasValidPlayer()) return;

        // ⛔ بعد از اولین شلیک، دیگر توقف مجاز نیست
        if (stopOnlyOnce && stopConsumed)
            return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool shouldStop = dist <= stopRange;

        if (shouldStop != isStoppedByPlayer)
        {
            isStoppedByPlayer = shouldStop;

            if (isStoppedByPlayer)
            {
                animator?.SetBool(movingBool, false);
                animator?.SetBool(attackBool, true);

                // ✅ این توقف مصرف شد
                if (stopOnlyOnce)
                    stopConsumed = true;
            }
            else
            {
                animator?.SetBool(attackBool, false);
                animator?.SetBool(movingBool, true);
            }
        }
    }




    private void UpdateAnimation(Vector3 currentPosition)
    {
        if (animator == null) return;

        bool moving = (currentPosition - lastPosition).sqrMagnitude > 0.0001f;
        animator.SetBool(movingBool, moving);
        lastPosition = currentPosition;
    }

    #endregion

    #region Helpers

    private void UpdateFacing()
    {
        if (!isHorizontal || spriteTransform == null) return;

        Vector3 scale = spriteTransform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        spriteTransform.localScale = scale;
    }

    private Vector3 GetDirectionVector(MoveDirection dir)
    {
        return dir switch
        {
            MoveDirection.Up    => Vector3.up,
            MoveDirection.Down  => Vector3.down,
            MoveDirection.Left  => Vector3.left,
            _                   => Vector3.right
        };
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }

    #endregion
}
