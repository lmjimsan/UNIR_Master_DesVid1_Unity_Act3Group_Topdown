using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerCharacter : BaseCharacter
{

    [Header("Inventario UI")]
    [SerializeField] private PlayerInventoryUI inventoryUI; // Asigna el PlayerInventoryUI en el inspector
    private bool inventoryOpen = false;
    [Header("Player-specific parameters")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference run;
    [SerializeField] InputActionReference attack;
    public float damage = 0.5f;
    public float shield = 0f; // Valor del escudo, si es 0 no hay escudo
    [SerializeField] float attackHitDelay = 0f;
    [SerializeField] float respawnDelay = 2f;
    public float Shield => shield;
    bool mustAttack;
    Purse purse;
    Coroutine attackHitRoutine;
    bool isDead = false;

    // Flag para saber si el puntero está sobre UI
    private bool pointerOverUI = false;
    private bool hasWon = false;

    protected override void Awake()
    {
        base.Awake();
        purse = GetComponent<Purse>();
        // life se inicializa en BaseCharacter
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Actualizar flag de UI al principio del frame
        pointerOverUI = false;
        if (EventSystem.current != null)
            pointerOverUI = EventSystem.current.IsPointerOverGameObject();
        // Abrir/cerrar inventario con la tecla I
        if (Keyboard.current != null)
        {
            if (Keyboard.current.iKey.wasPressedThisFrame)
            {
                if (inventoryUI != null)
                {
                    inventoryOpen = !inventoryOpen;
                    if (inventoryOpen)
                        inventoryUI.ShowInventory();
                    else
                        inventoryUI.HideInventory();
                }
            }
            // Salir al menú principal con Escape (solo si no has ganado)
            if (!hasWon && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                // Debug.Log("Escape presionado, cargando escena Menu...");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            }
        }
    
        // Comprobar si quedan enemigos y lanzar corrutina de victoria si no hay
        if (!hasWon && SceneManager.GetActiveScene().name == "Level3")
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
            {
                hasWon = true;
                StartCoroutine(HandleVictoryAndGoToYouWin());
            }
        }
    

        // Primero movemos al personaje en función de la entrada del jugador
        Move(rawMove);

        // Si hemos pulsado el boton de ataque, intentamos atacar
        // Importante hacer el Move() primero para obtener la dirección del ataque
        if (mustAttack)
        {
            // Bloquear ataque si el ratón está sobre UI (Input System y clicks directos)
            if (UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                // Debug.Log("Ataque bloqueado por UI");
                mustAttack = false;
                return;
            }
            // Atacamos, y si podemos hacerlo, comprobamos si hemos dado a alguien
            if (TryAttack())
            {
                // Debug.Log("Player pass TryAttack(), and attacked in direction " + attackDirection);
                if (attackHitDelay > 0f)
                {
                    if (attackHitRoutine != null) StopCoroutine(attackHitRoutine);
                    attackHitRoutine = StartCoroutine(DelayedAttackHit());
                }
                else
                {
                    // Debug.Log("Player attacked immediately in direction " + lastMove);
                    CheckAttack(lastMove, damage, "Enemy");
                }
            }
            mustAttack = false;
        }
    }

    // Método para aplicar daño al jugador, considerando el escudo
    public void ApplyDamage(float amount)
    {
        float finalDamage = amount;
        if (shield > 0f)
        {
            finalDamage = amount / shield;
        }
        // Aquí se aplica el daño a la vida
        if (life != null)
        {
            life.OnHitReceived(finalDamage);
        }
        else
        {
            // Debug.LogWarning("No se encontró componente Life en PlayerCharacter.");
        }
    }

    protected void OnEnable()
    {
        // base.OnEnable();
        if (move != null)
        {
            move.action.Enable();
            move.action.started += OnMove;
            move.action.performed += OnMove;
            move.action.canceled += OnMove;
        }
        if (run != null)
        {
            run.action.Enable();
            run.action.started += OnRun;
            run.action.performed += OnRun;
            run.action.canceled += OnRun;
        }
        if (attack != null)
        {
            attack.action.Enable();
            attack.action.started += OnAttack;
        }
    }

    protected void OnDisable()
    {
        // base.OnDisable();
        if (move != null)
        {
            move.action.Disable();
            move.action.started -= OnMove;
            move.action.performed -= OnMove;
            move.action.canceled -= OnMove;
        }
        if (run != null)
        {
            run.action.Disable();
            run.action.started -= OnRun;
            run.action.performed -= OnRun;
            run.action.canceled -= OnRun;
        }
        if (attack != null)
        {
            attack.action.Disable();
            attack.action.started -= OnAttack;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        rawMove = context.ReadValue<Vector2>();
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValue<float>() > 0.1f;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        // Bloquear ataque si el puntero está sobre UI (flag actualizado en Update)
        if (pointerOverUI)
        {
            // Debug.Log("Click bloqueado por UI");
            return;
        }
        mustAttack = true;
    }



    protected override void OnDeath()
    {
        if (isDead) return;
        
        isDead = true;
        base.OnDeath();
        
        // Desactivar control del jugador
        rb2D.linearVelocity = Vector2.zero;
        if (move != null)
        {
            move.action.Disable();
        }
        
        // Desactivar collider para no recibir más golpes
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Iniciar respawn después de la animación
        StartCoroutine(RespawnRoutine());
    }

    // Espera unos segundos tras ganar y salta a la escena YouWin
    private IEnumerator HandleVictoryAndGoToYouWin()
    {
        // Espera 2 segundos para que se vea la animación de muerte o celebración
        yield return new WaitForSeconds(2f);
        // Debug.Log("¡Victoria! Cargando escena YouWin...");
        SceneManager.LoadScene("YouWin");
    }

    private IEnumerator DelayedAttackHit()
    {
        yield return new WaitForSeconds(attackHitDelay);
        // Debug.Log("Player attacked after delay in direction " + lastMove);
        CheckAttack(lastMove, damage, "Enemy");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Recoger drops (curación, monedas, etc.)
        Drop drop = other.GetComponent<Drop>();
        if (drop != null)
        {
            if (drop.dropDefinition != null)
            {
                if (drop.dropDefinition.healthRecovery > 0f && life != null)
                {
                    life.RecoverHealth(drop.dropDefinition.healthRecovery);
                }

                if (drop.dropDefinition.coins > 0 && purse != null)
                {
                    purse.OnGetCoins(drop.dropDefinition.coins);
                }
            }
            drop.NotifyPickedUp();
            return;
        }
    }

/*
    // Coroutine para revertir el power up
    public IEnumerator RevertPowerUp(PlayerCharacter player, float damage, float shield, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (damage > 0)
        {
            player.damage -= damage;
            // Debug.Log($"PowerUp daño revertido: -{damage}");
        }
        if (shield > 0)
        {
            player.shield -= shield;
            // Debug.Log($"PowerUp escudo revertido: -{shield}");
        }
        // Cambiar skin según el nivel de daño
        var animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            RuntimeAnimatorController level1 = Resources.Load<RuntimeAnimatorController>("PlayerLevel1");
            RuntimeAnimatorController level2 = Resources.Load<RuntimeAnimatorController>("PlayerLevel2");
            RuntimeAnimatorController level3 = Resources.Load<RuntimeAnimatorController>("PlayerLevel3");
            if (player.damage <= 0.5f && level1 != null)
                animator.runtimeAnimatorController = level1;
            else if (player.damage > 0.5f && player.damage < 3.5f && level2 != null)
                animator.runtimeAnimatorController = level2;
            else if (player.damage >= 3.5f && level3 != null)
                animator.runtimeAnimatorController = level3;
        }
    }
*/

    private IEnumerator RespawnRoutine()
    {
        // Esperar a que termine la animación de muerte
        yield return new WaitForSeconds(respawnDelay);
        
        // Comprobar si estamos en la escena Home
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (currentSceneName != "Home")
        {
            // Si no estamos en Home, cargar la escena Home
            // Debug.Log($"Player murió en {currentSceneName}, cargando Home...");
            
            // Suscribirse al evento de carga de escena para hacer el reset después
            SceneManager.sceneLoaded += OnRespawnSceneLoaded;
            
            SceneManager.LoadScene("Home");
        }
        else
        {
            // Ya estamos en Home, hacer respawn in-situ
            PerformRespawn();
        }
    }
    
    private void OnRespawnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Desuscribirse del evento
        SceneManager.sceneLoaded -= OnRespawnSceneLoaded;
        
        // Esperar un frame para que todo se inicialice
        StartCoroutine(DelayedRespawn());
    }
    
    private IEnumerator DelayedRespawn()
    {
        yield return null; // Esperar 1 frame
        PerformRespawn();
    }
    
    private void PerformRespawn()
    {
        // Debug.Log("Ejecutando PerformRespawn...");
        
        // Buscar punto de spawn por defecto
        PlayerSpawnPoint[] spawnPoints = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        PlayerSpawnPoint selectedSpawn = null;
        
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            foreach (var spawn in spawnPoints)
            {
                if (spawn.IsDefault)
                {
                    selectedSpawn = spawn;
                    break;
                }
            }
            
            if (selectedSpawn == null)
            {
                selectedSpawn = spawnPoints[0];
            }
        }
        
        // Teleportar al spawn point
        if (selectedSpawn != null)
        {
            transform.position = selectedSpawn.transform.position;
            // Debug.Log($"Player teleportado a {selectedSpawn.name} en posición {transform.position}");
        }
        
        // Re-habilitar collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        // Resetear velocidad
        rb2D.linearVelocity = Vector2.zero;

        // Restaurar vida al máximo
        if (life != null)
        {
            life.Respawn();
        }

        // Resetear parámetros del Animator y lanzar trigger Respawn
        if (animator != null)
        {
            animator.ResetTrigger("Death"); // Por limpieza, si existe
            animator.SetTrigger("Respawn");
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsRunning", false);
        }

        // Resetear estado de muerte
        isDead = false;
        // Volver a habilitar el movimiento
        if (move != null)
        {
            move.action.Enable();
        }
    }
}
