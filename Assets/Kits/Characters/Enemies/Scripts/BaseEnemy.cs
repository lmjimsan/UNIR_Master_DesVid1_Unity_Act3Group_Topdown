using UnityEngine;

public class BaseEnemy : BaseCharacter
{
    
    [Header("Attack AI")]
    // Distancia máxima a la que el enemigo puede atacar al jugador
    [SerializeField] private float attackRange = 1f;
    // Distancia mínima que el enemigo intenta mantener del jugador (si está más cerca, se aleja)
    [SerializeField] private float optimalMinDistance = 0.2f;
    // Tiempo mínimo de espera entre ataques (en segundos)
    [SerializeField] private float minAttackCooldown = 1.5f;
    // Tiempo máximo de espera entre ataques (en segundos)
    [SerializeField] private float maxAttackCooldown = 3f;
    // Daño que causa cada ataque del enemigo al jugador
    [SerializeField] private float attackDamage = 0.2f;
    
    [Header("Death")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelaySeconds = 2f;

    [Header("Drops")]
    [SerializeField] private Drop droppedFewCoins;
    [SerializeField] private Drop droppedSomeCoins;
    [SerializeField] private Drop droppedLotOfCoins;
    [SerializeField, Range(0f, 1f)] private float baseSomeChance = 0.18f;
    [SerializeField, Range(0f, 1f)] private float baseLotChance = 0.02f;

    [Header("Idle Wander")]
    [SerializeField] private bool enableIdleWander = true;
    [SerializeField] private float idleWanderRadius = 3f;
    [SerializeField] private float idleWanderRepathTime = 2f;
    [SerializeField] private float idleWanderStopDistance = 0.1f;
    [SerializeField] private float idleWanderMinPause = 0.5f;
    [SerializeField] private float idleWanderMaxPause = 2f;

    Sight2D sight;
    bool isDead;
    float nextAttackTime;
    Vector2 wanderOrigin;
    Vector2 wanderTarget;
    float nextWanderTime;
    float idleUntilTime;

    protected override void Awake()
    {
        base.Awake();
        sight = GetComponent<Sight2D>();
        wanderOrigin = transform.position;
        if (idleWanderMaxPause < idleWanderMinPause)
        {
            idleWanderMaxPause = idleWanderMinPause;
        }
        SetNewWanderTarget();
    }

    protected override void Update()
    {
        if (isDead) return;

        Transform closestTarget = sight.GetClosestTarget();

        if (closestTarget != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, closestTarget.position);
            Vector2 directionToTarget = (closestTarget.position - transform.position).normalized;
            
            // Zona 1: Demasiado cerca (< optimalMinDistance) -> Alejarse
            if (distanceToTarget < optimalMinDistance)
            {
                Move(-directionToTarget); // Moverse en dirección contraria
                TryAttackTarget(closestTarget); // Pero sigue intentando atacar
            }
            // Zona 2: Distancia óptima (entre optimalMinDistance y attackRange) -> Mantener posición y atacar
            else if (distanceToTarget <= attackRange)
            {
                TryAttackTarget(closestTarget);
                // No moverse, mantiene la distancia
            }
            // Zona 3: Demasiado lejos (> attackRange) -> Acercarse
            else
            {
                Move(directionToTarget);
            }
        }
        else if (enableIdleWander)
        {
            HandleIdleWander();
        }
    }

    private void TryAttackTarget(Transform target)
    {
        // Verificar si ha pasado el cooldown
        if (Time.time < nextAttackTime) return;

        // NO actualizar lastMove aquí - el Enemy ataca en la dirección que está mirando
        // Si está de espaldas al Player, su ataque fallará

        // Intentar atacar (esto lanzará la animación y sonido)
        if (TryAttack())
        {
            // Comprobar impacto INMEDIATAMENTE (sin delay ni eventos)
            // usa la dirección actual (lastMove) que se actualiza en Move()
            CheckAttack(lastMove, attackDamage, "Player");
            
            // Establecer el próximo tiempo de ataque (aleatorio)
            float cooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
            nextAttackTime = Time.time + cooldown;
        }
    }

    protected override void OnDeath()
    {
        if (isDead) return;

        isDead = true;
        base.OnDeath();

        // Desactivar collider para que no reciba más golpes mientras muere
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        SpawnDrop();

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelaySeconds);
        }
    }

    // --- Lógica de llaves y drops especiales ---
    [Header("Llave que puede soltar este enemigo (opcional)")]
    [SerializeField] private string keyToDropId; // Ej: Key_Level2, Key_Level3
    [SerializeField] private GameObject keyPrefab; // Prefab de la llave a soltar
    [Header("¿Es el jefe final?")]
    [SerializeField] private bool isFinalBoss = false;

    private void SpawnDrop()
    {
        // Si es jefe final, termina la partida
        if (isFinalBoss)
        {
            // Mostrar mensaje You Win (puedes personalizar esto)
            Debug.Log("You Win!");
            // Resetear PlayerPrefs
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            // Ir al menú principal (ajusta el nombre de la escena)
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            return;
        }

        // Si tiene llave para soltar y el jugador NO la tiene, soltar la llave
        if (!string.IsNullOrEmpty(keyToDropId) && keyPrefab != null)
        {
            if (PlayerPrefs.GetInt($"key_collected_{keyToDropId}", 0) == 0)
            {
                Instantiate(keyPrefab, transform.position, Quaternion.identity);
                return;
            }
        }

        // Si no, soltar drop normal (monedas)
        Drop prefab = ChooseDropPrefab();
        if (prefab == null) return;

        Drop spawned = Instantiate(prefab, transform.position, Quaternion.identity);
        if (spawned == null) return;

        Coin coin = spawned.GetComponent<Coin>();
        if (coin != null)
        {
            coin.PlayJump();
        }
    }

    private Drop ChooseDropPrefab()
    {
        float lifeMultiplier = 1f;
        if (life != null)
        {
            lifeMultiplier = Mathf.Max(1f, life.StartingLife);
        }

        // LotOf tiene máxima prioridad: se escala primero y se clampea a 1
        float scaledLot = Mathf.Clamp(baseLotChance * lifeMultiplier, 0f, 1f);
        
        // Some tiene segunda prioridad: se escala pero no puede hacer que Lot+Some > 1
        float scaledSome = Mathf.Clamp(baseSomeChance * lifeMultiplier, 0f, 1f - scaledLot);
        
        // Few es el resto (siempre >= 0 por construcción)
        float scaledFew = 1f - scaledLot - scaledSome;

        // Hacer el roll aleatorio (total siempre es 1)
        float roll = Random.value;
        
        // Primero chequeamos LotOf (la más rara)
        if (roll < scaledLot)
        {
            return droppedLotOfCoins != null ? droppedLotOfCoins : GetFallbackDrop();
        }
        roll -= scaledLot;
        
        // Luego Some
        if (roll < scaledSome)
        {
            return droppedSomeCoins != null ? droppedSomeCoins : GetFallbackDrop();
        }
        
        // Por defecto Few
        return droppedFewCoins != null ? droppedFewCoins : GetFallbackDrop();
    }

    private Drop GetFallbackDrop()
    {
        if (droppedFewCoins != null) return droppedFewCoins;
        if (droppedSomeCoins != null) return droppedSomeCoins;
        if (droppedLotOfCoins != null) return droppedLotOfCoins;
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizar rango de ataque en el editor (zona óptima de combate)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Visualizar distancia mínima óptima (zona de retroceso)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, optimalMinDistance);

        if (enableIdleWander)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, idleWanderRadius);
        }
    }

    private void HandleIdleWander()
    {
        if (Time.time < idleUntilTime)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, wanderTarget);
        if (Time.time >= nextWanderTime || distance <= idleWanderStopDistance)
        {
            SetNewWanderTarget();
            idleUntilTime = Time.time + Random.Range(idleWanderMinPause, idleWanderMaxPause);
            return;
        }

        Vector2 direction = (wanderTarget - (Vector2)transform.position);
        if (direction.sqrMagnitude > idleWanderStopDistance * idleWanderStopDistance)
        {
            Move(direction.normalized);
        }
    }

    private void SetNewWanderTarget()
    {
        Vector2 offset = Random.insideUnitCircle * idleWanderRadius;
        wanderTarget = wanderOrigin + offset;
        nextWanderTime = Time.time + idleWanderRepathTime;
    }
}
