using UnityEngine;

public class BaseCharacter : MonoBehaviour, IVisible2D
{    
    [Header("Moving parameters")]
    [SerializeField] protected string actionLayerName = "Action Layer";
    [SerializeField] protected float linearspeed = 2f; // Velocidad base en unidades por segundo.
    [SerializeField] protected float runMultiplier = 1.5f; // Multiplica la velocidad al correr.
    [SerializeField] protected float speedParamScale = 1f; // Escala el parametro Speed del Animator.
    [SerializeField] protected float moveDeadZone = 0.01f; // Minimo input para actualizar la direccion.

    protected Animator animator;
    protected Vector2 rawMove;
    protected Vector2 lastMove = Vector2.down;
    protected bool isRunning;
    int actionLayerIndex = -1;

    static readonly int MoveX = Animator.StringToHash("MoveX");
    static readonly int MoveY = Animator.StringToHash("MoveY");
    static readonly int Speed = Animator.StringToHash("Speed");
    static readonly int IsRunning = Animator.StringToHash("IsRunning");
    static readonly int AttackTrigger = Animator.StringToHash("Attack");

    [Header("Audio")]
    [SerializeField] protected AudioClip hitSfx;
    [SerializeField] protected AudioClip hurtSfx;
    [SerializeField] protected AudioClip deathSfx;

    [Header("IVisible2D parameters")]
    [SerializeField] protected int Priority = 0;
    [SerializeField] protected IVisible2D.Side side = IVisible2D.Side.Neutrals;

    [Header("Check Collider Attack")]
    [SerializeField] protected float attackRadius = 0.3f;
    [SerializeField] protected float attackDistance = 0.5f;
    [SerializeField] protected string attackStateName = "Attack";
    
    protected Rigidbody2D rb2D;
    AudioSource audioSource;
    [SerializeField] protected Life life;
    public Life Life => life;


    protected virtual void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        actionLayerIndex = animator.GetLayerIndex(actionLayerName);
        if (actionLayerIndex < 0)
        {
            // Debug.LogError("Animator layer not found: " + actionLayerName, this);
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = GetComponentInChildren<AudioSource>();
        life = GetComponent<Life>();
        if (life == null) life = GetComponentInChildren<Life>();

        if (life != null)
        {
            life.OnDeath.AddListener(OnDeath);
        }
    }

    protected virtual void Update()
    {
    }

    protected void Move(Vector2 direction)
    {
        // Optenemos el multiplicador de velocidad según si estamos corriendo o no
        float speedMultiplier = isRunning ? runMultiplier : 1f;
        
        // Asegura que cuando nos movemos en diagonal mantiene la velocidad máxima
        if (direction.sqrMagnitude > 1f) direction.Normalize();

        // Cambiamos la posición en función de la dirección, la velocidad y el tiempo
        rb2D.position += direction * speedMultiplier * linearspeed * Time.deltaTime;
        UpdateAnimator(direction, speedMultiplier);
    }

    protected void UpdateAnimator(Vector2 moveInput, float speedMultiplier)
    {
        bool actionLocksFacing = IsActionActive();
        if (!actionLocksFacing && moveInput.sqrMagnitude > moveDeadZone * moveDeadZone)
        {
            lastMove = moveInput.normalized;
        }

        animator.SetFloat(MoveX, lastMove.x);
        animator.SetFloat(MoveY, lastMove.y);
        animator.SetFloat(Speed, moveInput.magnitude * linearspeed * speedMultiplier * speedParamScale);
        animator.SetBool(IsRunning, isRunning);
    }

    protected bool TryAttack()
    {
        if (!CanAttack()) return false;

        PlayOneShot(hitSfx);
        animator.SetTrigger(AttackTrigger);
        return true;
    }

    // Comprueba si hemos dado a alguien o no
    protected void CheckAttack(Vector2 direction, float damage, string targetTag)
    {
        // Este método se llama desde un evento de animación en el momento de hacer daño
        // Aquí es donde deberíamos comprobar qué enemigos están en rango y aplicarles daño
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRadius, direction, attackDistance);
        // Debug.Log("Checking attack hits: " + hits.Length);
        
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag(targetTag))
            {
                BaseCharacter otherCharacter = hit.collider.GetComponent<BaseCharacter>();
                if (otherCharacter != null) otherCharacter.NotifyDamage(damage);
            }
        }
    }

    // Para depurar la zona de ataque en el editor (hay que seleccionar al personaje en la escena)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 start = transform.position;
        Vector3 dir = (Vector3)lastMove.normalized;
        Vector3 end = start + dir * attackDistance;

        Gizmos.DrawWireSphere(start, attackRadius); // círculo inicial
        Gizmos.DrawLine(start, end);                // barrido
        Gizmos.DrawWireSphere(end, attackRadius);   // círculo final
    }

    protected void NotifyDamage(float damage)
    {
        PlayOneShot(hurtSfx);
        animator.SetTrigger("Hurt");

        if (life != null)
        {
            life.OnHitReceived(damage);
        }
    }

    protected bool CanAttack()
    {
        if (actionLayerIndex < 0) return false;

        // Si estamos transicionando de estado, no dejamos atacar.
        if (animator.IsInTransition(actionLayerIndex)) return false;

        // Leemos el estado actual de la capa Action.
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(actionLayerIndex);

        // Si estamos en Hurt o Death, no atacamos, y si ya estamos en Attack y no ha terminado el ciclo, no repetimos.
        if (info.IsName("Death") || info.IsName("Hurt")) return false;
        if (info.IsName(attackStateName) && info.normalizedTime < 1f) return false;

        return true;
    }

    protected bool IsActionActive()
    {
        if (actionLayerIndex < 0) return false;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(actionLayerIndex);
        return info.IsName("Attack") || info.IsName("Hurt") || info.IsName("Death");
    }

    // Llamado cuando el personaje muere (suscrito a Life.OnDeath).
    protected virtual void OnDeath()
    {
        PlayOneShot(deathSfx);
        animator.SetTrigger("Death");
    }

    // Reproduce sonido si hay fuente y clip.
    protected void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip, AudioManager.SfxVolume);
            return;
        }

        AudioSource.PlayClipAtPoint(clip, transform.position, AudioManager.SfxVolume);
    }

    // Implementación de la interfaz Visible2D
    public int GetPriority()
    {
        return Priority;
    }

    public IVisible2D.Side GetSide()
    {
        return side;
    }
}