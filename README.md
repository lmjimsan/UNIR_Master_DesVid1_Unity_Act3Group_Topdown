# Way of Kamael

**Versión:** 1.0  
**Motor:** Unity 6000.2.7f2  
**Género:** RPG 2D de Acción  
**Autor:** LMJimene

---

## 📖 Manual de Usuario

---

## 🧪 Sistema de Pociones y Consumibles

### ¿Qué es una poción?
Las pociones son objetos consumibles que el jugador puede usar para recuperar vida, obtener power-ups temporales (daño, escudo) o recibir otros efectos. Se pueden obtener como drops de enemigos, encontrarlas en el mundo, o recibirlas en el inventario/baúl.

### ¿Cómo se definen?
Las pociones y consumibles se definen mediante dos assets:

- **DropDefinition (ScriptableObject):** Define los efectos de la poción (vida, daño, escudo, duración, monedas).
  - Ubicación: `Assets/Kits/GamePlayObjects/Drops/`
  - Ejemplo de campos:
    - `healthRecovery`: Vida que recupera.
    - `powerUpDamage`: Aumento temporal de daño.
    - `powerUpShield`: Aumento temporal de escudo.
    - `powerUpDuration`: Duración del efecto (segundos).
    - `coins`: Monedas que otorga.

- **Item (ScriptableObject):** Representa el objeto en el inventario.
  - Ubicación: `Assets/Kits/Systems/InventorySystem/`
  - Campos relevantes:
    - `itemType`: Debe ser `Consumable` para pociones.
    - `useType`: `Manual` (el jugador debe pulsar para usarla) o `Automatic` (se usa al recoger).
    - `dropDefinition`: Referencia al asset de DropDefinition que define el efecto.

### ¿Cómo se configuran?
1. Crea un nuevo asset de tipo `DropDefinition` y ajusta los valores según el efecto deseado.
2. Crea un nuevo asset de tipo `Item`:
   - Ponle nombre, sprite, descripción, etc.
   - Selecciona `itemType = Consumable`.
   - Selecciona `useType = Manual` si quieres que el jugador la use desde el inventario, o `Automatic` si se consume al recogerla.
   - Asigna el campo `dropDefinition` con el asset creado en el paso 1.
3. Asigna el Item como drop de un enemigo, colócalo en el mundo, o añádelo al inventario/baúl.

### ¿Cómo se obtienen?
- **Como drop de enemigos:** Al morir, los enemigos pueden soltar objetos configurados como pociones (ver sistema de drops).
- **En el mundo:** Puedes colocar un objeto con el componente `Drop` y asignar la DropDefinition de la poción.
- **En el inventario/baúl:** Puedes añadir la poción directamente al inventario del jugador o al baúl desde el editor.

### ¿Cómo se usan?
- **Recogida directa:** Si la poción es de `useType = Automatic`, al recogerla se aplica el efecto automáticamente (vida, monedas, etc.).
- **Desde inventario:** Si es de `useType = Manual`, el jugador debe abrir el inventario (`I`) y hacer clic izquierdo sobre la poción para consumirla. El efecto se aplica y la poción se elimina del inventario.
  - Si la poción otorga un power-up temporal (daño, escudo), el efecto dura el tiempo configurado y luego se revierte automáticamente.

### Ejemplo de flujo de uso
1. El jugador derrota a un enemigo y este suelta una poción.
2. El jugador la recoge:
   - Si es automática, recupera vida al instante.
   - Si es manual, aparece en el inventario.
3. El jugador abre el inventario (`I`), hace clic izquierdo sobre la poción y se aplica el efecto.
4. Si la poción otorga un power-up, el HUD y el aspecto del jugador pueden cambiar durante la duración del efecto.

### Notas para desarrolladores
- El sistema es extensible: puedes crear nuevos efectos añadiendo campos a DropDefinition y gestionando su uso en PlayerCharacter y PlayerSlotUI.
- El sistema de inventario y drops es genérico y permite añadir fácilmente nuevos tipos de consumibles.
- Los efectos de las pociones se aplican en los scripts `PlayerCharacter.cs` (al recoger) y `PlayerSlotUI.cs` (al consumir desde inventario).

---

### Historia

En un mundo olvidado por los dioses, donde las sombras acechan en cada rincón, un héroe solitario se alza para restaurar el equilibrio perdido. **Way of Kamael** narra la historia de un guerrero ancestral que debe atravesar tierras hostiles, enfrentarse a hordas de enemigos y superar desafíos mortales para alcanzar el santuario sagrado de Kamael.

Cada nivel es un nuevo desafío, cada enemigo una prueba de valentía. El camino está plagado de puertas selladas, tesoros ocultos y criaturas que no dudarán en defender su territorio. Solo los más valientes lograrán completar el Way of Kamael.

---

### Menú Principal

Al iniciar el juego, encontrarás el **Menú Principal** con las siguientes opciones:

- **Play**: Comienza una nueva partida o continúa desde donde lo dejaste.
- **Options**: Accede a la configuración de audio (música y efectos de sonido).
- **Credits**: Visualiza los créditos del juego.
- **Quit**: Cierra el juego.

#### Opciones de Audio

En el menú de opciones podrás ajustar:
- **Music Volume**: Controla el volumen de la música de fondo (0% - 100%).
- **SFX Volume**: Controla el volumen de los efectos de sonido (0% - 100%).

Los cambios se guardan automáticamente y persisten entre sesiones.

---

### El Jugador

Controlas a un **guerrero heroico** con las siguientes características:

#### Atributos
- **Vida (Life)**: Representada por una barra sobre la cabeza del jugador y en el HUD superior.
- **Monedero (Purse)**: Acumula monedas recogidas durante la aventura, mostradas en el HUD.
- **Inventario de Llaves**: Recoge llaves para abrir puertas específicas.

#### Controles de Teclado

|-------------|------------------------------|
| Acción      | Tecla(s)                     |
|-------------|------------------------------|
| **Moverse** | `W`, `A`, `S`, `D` o Flechas |
| **Correr**  | Mantener `Shift`             |
| **Atacar**  | `Espacio` o `Clic Izquierdo` |
| **Inventario** | `I`                       |
|-------------|------------------------------|



#### Inventario
- Pulsa la tecla `I` para abrir o cerrar la ventana de inventario y ver los objetos que llevas.

### Almacenamiento (Baúl/Home Storage)

En la casa principal (Home) encontrarás un baúl de almacenamiento. Puedes usarlo para guardar objetos y transferirlos entre partidas.

- **Abrir el baúl:** Acércate al baúl (Store) y se abrirá automáticamente la ventana de almacenamiento.
- **Cerrar el baúl:** Al alejarte del baúl, la ventana se cierra automáticamente.
- **Intercambiar objetos:**
  1. Con el baúl abierto, pulsa `I` para abrir tu inventario de jugador.
    2. Puedes transferir objetos entre los slots del baúl y los de tu inventario simplemente haciendo clic izquierdo:
      - Haz clic izquierdo sobre un objeto del inventario del jugador para enviarlo al primer hueco libre del baúl.
      - Haz clic izquierdo sobre un objeto del baúl para enviarlo al primer hueco libre del inventario del jugador.
      - Si no hay hueco disponible, el objeto no se mueve.
      - No es necesario arrastrar ni soltar, ni ningún Canvas especial ni icono de drag.
  3. El inventario del jugador se cierra con `I` como siempre; el del baúl solo al salir del área.
- **Capacidad:** El baúl tiene 25 slots, igual que su ventana.
- **Persistencia:** Los objetos guardados en el baúl permanecen aunque mueras o cambies de escena.

Esta mecánica te permite gestionar tu inventario y planificar qué objetos llevar contigo y cuáles dejar a salvo en casa.

#### Mecánicas del Jugador

- **Movimiento 8-direccional**: El jugador puede moverse en todas las direcciones.
- **Correr**: Duplica la velocidad de movimiento al mantener Shift.
- **Ataque**: Realiza un ataque cuerpo a cuerpo en la dirección en la que mira. Tiene un pequeño delay antes de detectar el impacto para sincronizar con la animación.
- **Sistema de Respawn**: Al morir, el jugador reaparece en el punto de inicio del nivel **Home** con la vida restaurada al máximo, conservando todas las monedas y objetos recogidos.

---

### Enemigos

El juego cuenta con varios tipos de enemigos, cada uno con comportamientos únicos:

#### Tipos de Enemigos

1. **Enemigos Patrulleros**:
   - Se mueven en patrones aleatorios (wander).
   - Atacan al jugador cuando está dentro de su rango de detección.
   - Daño variable según el tipo.

2. **Enemigos Estáticos**:
   - Permanecen en un área fija.
   - Solo atacan si el jugador se acerca demasiado.

#### Sistema de Daño

- Cada enemigo tiene un valor de **daño** configurable (típicamente 0.1 - 0.5 por golpe).
- El jugador también inflige daño configurable a los enemigos (típicamente 0.5 por golpe).
- Las barras de vida sobre las cabezas cambian de color según el estado:
  - **Verde**: Vida superior al 60%.
  - **Amarillo**: Vida entre 30% - 60%.
  - **Rojo**: Vida inferior al 30%.

#### Sistema de Oleadas

Algunos niveles cuentan con **generadores de oleadas** que spawnean enemigos en grupos:
- Pueden generar múltiples oleadas consecutivas.
- Limitan el número de enemigos vivos simultáneamente.
- Se activan cuando el jugador se acerca a un radio específico.

---

### Objetos del Juego

#### Monedas

- **Icono**: Moneda dorada animada.
- **Función**: Incrementa el contador de monedas del jugador.
- **Comportamiento**: Se lanzan hacia arriba al aparecer (animación de salto) y quedan disponibles para recolección.

#### Corazones (Hearts)

- **Icono**: Corazón rojo pulsante.
- **Función**: Restaura parte de la vida del jugador.
- **Comportamiento**: Similar a las monedas, aparecen en el suelo tras derrotar enemigos o en ubicaciones específicas.

#### Llaves (Keys)

- **Icono**: Llave dorada.
- **Función**: Permite abrir puertas específicas que coincidan con su **Key ID**.
- **Comportamiento**: Se recogen al tocarlas y se almacenan en el inventario del jugador.

#### Puertas (Doors)

- **Estados**:
  - **Cerrada**: Bloqueada con collider activo.
  - **Abierta**: Permite el paso, collider desactivado.
  - **Con Llave (Locked)**: Requiere una llave específica para abrirse.

- **Mecánica de Llaves**:
  - Cada puerta tiene un **Required Key ID** (ej: "Level1Key").
  - Solo se puede abrir con una llave que coincida exactamente (case-insensitive).
  - Al abrir una puerta con llave, se reproduce un sonido de desbloqueo.

---

### Sistema de Drops

Al derrotar enemigos, estos pueden soltar objetos según probabilidades configuradas:

- **Drop Probability A**: Probabilidad de soltar el primer tipo de objeto (0% - 100%).
- **Drop Probability B**: Probabilidad del segundo tipo si el primero falla.
- **Drop Probability C**: Probabilidad del tercer tipo si los anteriores fallan.

Los objetos caen al suelo con una animación de salto y emiten un sonido al ser recogidos.

---

### Interfaz de Usuario (HUD)

El HUD muestra información esencial en tiempo real:

- **Barra de Vida**: En la esquina superior izquierda, con un corazón que cambia de color.
- **Contador de Monedas**: Muestra el total de monedas acumuladas.
- **Barras de Vida sobre Enemigos**: Indican el estado de salud de cada enemigo.

---

### Transiciones de Escena

El juego utiliza un sistema de transiciones entre niveles:

- **Puertas Interactivas**: Al atravesar ciertas puertas, se carga un nuevo nivel.
- **Sistema de Spawn Points**: Cada nivel tiene puntos de spawn identificados por ID. Al entrar a un nivel, el jugador aparece en el spawn point correspondiente a la puerta que atravesó.

---

## 🛠️ Documento Técnico

### Estructura del Proyecto

El proyecto está organizado en las siguientes carpetas principales:

```
Assets/
├── Animations/          # Animaciones de personajes y objetos
├── Audio/               # Clips de audio (música y SFX)
├── Graphics/            # Sprites, tilesets y recursos visuales
├── InputActions/        # Configuración del Input System de Unity
├── Prefab/              # Prefabs globales del proyecto
├── Scenes/              # Escenas del juego (MainMenu, Home, Level1, etc.)
├── Kits/                # Sistemas y componentes del juego
│   ├── Characters/      # Scripts y recursos de personajes
│   │   ├── Commons/     # BaseCharacter (clase base compartida)
│   │   ├── Enemies/     # BaseEnemy, EnemyWaveSpawner
│   │   └── Player/      # PlayerCharacter, PlayerPersistence, PlayerSpawnPoint
│   ├── GamePlayObjects/ # Objetos interactivos del juego
│   │   ├── Coins/       # Coin (monedas)
│   │   ├── Doors/       # Door, Key (puertas y llaves)
│   │   ├── Drops/       # Drop, DropDefinition (sistema de drops)
│   │   └── Heart/       # Heart (corazones de vida)
│   └── Systems/         # Sistemas core del juego
│       ├── Audio/       # AudioManager, SceneMusic
│       ├── CombatSystem/
│       ├── HUDSystem/   # HUDManager, LifeBar, PurseHUDAmount
│       ├── LifeSystem/  # Life (componente de vida)
│       ├── MainMenu/    # MainMenu, OptionsMenu
│       ├── PurseSystem/ # Purse (sistema de monedas)
│       ├── SceneTransitions/ # SceneTransition
│       └── SenseSystems/ # IVisible2D (sistema de detección)
```

---

### 1. Sistema de Escenas

El juego utiliza **carga aditiva de escenas** para mantener ciertos elementos persistentes:

#### Escenas Principales
- **MainMenu**: Menú principal del juego.
- **Home**: Nivel inicial del jugador.
- **Level1, Level2, ...**: Niveles adicionales.
- **HUD**: Escena aditiva que se carga automáticamente con el HUD.

#### Carga del HUD
El script `HUDLoader` se ejecuta al inicio del juego y carga la escena HUD de forma aditiva si no está ya cargada:

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
static void LoadHUD()
{
    if (!IsSceneLoaded("HUD"))
    {
        SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
    }
}
```

---

### 2. Sistema de Audio Global (AudioManager)

**Ubicación**: `Assets/Kits/Systems/Audio/Scripts/AudioManager.cs`

#### Características

- **Singleton con DontDestroyOnLoad**: Persiste entre escenas.
- **Control de Volumen Unificado**: Maneja el volumen de música y SFX globalmente.
- **Eventos de Cambio de Volumen**: Notifica a otros componentes cuando cambia el volumen.
- **Persistencia con PlayerPrefs**: Guarda las preferencias de volumen.
- **Auto-creación en Runtime**: Se crea automáticamente al iniciar el juego.

#### Uso

```csharp
// Cambiar volumen de música
AudioManager.SetMusicVolume(0.8f);

// Cambiar volumen de SFX
AudioManager.SetSfxVolume(0.6f);

// Obtener volumen actual
float sfxVol = AudioManager.SfxVolume;

// Reproducir sonido con el volumen global
audioSource.PlayOneShot(clip, AudioManager.SfxVolume);
```

#### SceneMusic

Componente que se adjunta a los AudioSource de música en cada escena. Se suscribe a los cambios de volumen del AudioManager y ajusta el volumen automáticamente:

```csharp
private void OnMusicVolumeChanged(float newVolume)
{
    if (musicSource != null)
    {
        musicSource.volume = newVolume;
    }
}
```

---

### 3. Sistema de HUD

**Ubicación**: `Assets/Kits/Systems/HUDSystem/`

#### HUDManager

Script central que conecta el HUD con el jugador:

- Se suscribe al evento `SceneManager.sceneLoaded` para reconectar cuando cambia la escena.
- Busca automáticamente al `PlayerCharacter` en la escena.
- Conecta la barra de vida (`LifeBar`) con el componente `Life` del jugador.
- Conecta el contador de monedas (`PurseHUDAmount`) con el `Purse` del jugador.

#### LifeBar

Componente que muestra la barra de vida:

- Se suscribe a los eventos `OnLifeChanged` y `OnDeath` del componente `Life`.
- Cambia de color según el porcentaje de vida (verde/amarillo/rojo).
- **Comportamiento en Player vs Enemigos**:
  - **Enemigos**: `destroyOnDeath = true` → Se destruye al morir.
  - **Player**: `destroyOnDeath = false` → Se oculta y reaparece al respawnear.

---

### 4. Sistema de Persistencia del Jugador

**Ubicación**: `Assets/Kits/Characters/Player/Scripts/PlayerPersistence.cs`

#### PlayerPersistence

- **Singleton con DontDestroyOnLoad**: El jugador persiste entre escenas.
- **Sistema de Spawn Points**: Al cargar una escena, busca `PlayerSpawnPoint` con el ID correspondiente.
- **SetNextSpawnId()**: Método estático para indicar dónde debe aparecer el jugador en la siguiente escena.

#### PlayerSpawnPoint

Componente marcador que indica dónde puede aparecer el jugador:

```csharp
[SerializeField] private string spawnId = "Default";
[SerializeField] private bool isDefault = true;
```

#### Sistema de Respawn

Cuando el jugador muere:

1. Reproduce la animación de muerte (2 segundos por defecto).
2. Si está en otra escena, carga **Home**.
3. Si ya está en Home, se teleporta al spawn point por defecto.
4. Restaura la vida al 100% con `life.Respawn()`.
5. Resetea el Animator con `animator.Rebind()` para volver al estado Idle.
6. Reactiva el collider.

**Importante**: El jugador **NO se destruye**, mantiene todas sus monedas, objetos e inventario.

---

### 5. Sistema de Personajes

#### BaseCharacter

**Ubicación**: `Assets/Kits/Characters/Commons/Scripts/BaseCharacter.cs`

Clase base compartida por `PlayerCharacter` y `BaseEnemy`:

- **Movimiento**: Maneja el movimiento con Rigidbody2D.
- **Animación**: Controla el Animator con parámetros como `MoveX`, `MoveY`, `Speed`, `IsRunning`.
- **Sistema de Ataque**:
  - Utiliza `Physics2D.OverlapCircle` para detectar colisiones en un radio.
  - Filtra enemigos por tag.
  - Aplica daño al componente `Life` del objetivo.
  
```csharp
protected void CheckAttack(Vector2 direction, float damage, string targetTag)
{
    Vector2 attackOrigin = (Vector2)transform.position + direction * attackDistance;
    Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin, attackRadius);
    
    foreach (var hit in hits)
    {
        if (hit.CompareTag(targetTag))
        {
            Life targetLife = hit.GetComponent<Life>();
            if (targetLife != null)
            {
                targetLife.OnHitReceived(damage);
            }
        }
    }
}
```

- **Audio**: Reproduce sonidos al atacar, recibir daño y morir con volumen controlado por AudioManager.

#### PlayerCharacter

Extiende `BaseCharacter` con:

- **Input del jugador**: Usa el nuevo Input System de Unity.
- **Detección de drops**: OnTriggerEnter2D para recoger DropDefinitions.
- **Sistema de respawn completo**.

#### BaseEnemy

Extiende `BaseCharacter` con:

- **IA de movimiento**: Modo idle con wander aleatorio.
- **Detección del jugador**: Persigue si está dentro del rango.
- **Ataque automático**: Ataca cuando el jugador está cerca.
- **Sistema de drops**: Al morir, genera un drop basado en probabilidades.

---

### 6. Sistema de Generación de Oleadas

**Ubicación**: `Assets/Kits/Characters/Enemies/Scripts/EnemyWaveSpawner.cs`

#### EnemyWaveSpawner

Sistema que genera oleadas de enemigos:

- **Configuración**:
  - `waveCount`: Número de oleadas.
  - `timeBetweenWaves`: Tiempo entre oleadas.
  - `timeBetweenSpawns`: Tiempo entre cada spawn individual.
  - `enemyPrefabA` / `enemyPrefabB`: Dos tipos de enemigos.
  - `enemyCountA` / `enemyCountB`: Cantidad de cada tipo.
  - `maxAlive`: Límite de enemigos vivos simultáneamente.
  - `spawnPoints[]`: Array de puntos de spawn.
  
- **Activación**:
  - Manual: Llamar a `StartWaves()`.
  - Automática: Cuando el jugador entra en un radio (`startWhenPlayerNear`).

- **Algoritmo**:
  1. Espera a que haya espacio (enemigos vivos < maxAlive).
  2. Elige aleatoriamente entre enemyA y enemyB.
  3. Instancia en un spawn point aleatorio.
  4. Repite hasta completar la oleada.
  5. Espera y comienza la siguiente oleada.

---

### 7. Sistema de Drops

**Ubicación**: `Assets/Kits/GamePlayObjects/Drops/`

#### DropDefinition (ScriptableObject)

Define qué otorga un drop:

```csharp
public class DropDefinition : ScriptableObject
{
    public float healthRecovery;  // Cantidad de vida a recuperar
    public int coins;              // Cantidad de monedas
}
```

#### Drop

Componente genérico para objetos recogibles:

- Contiene una referencia a `DropDefinition`.
- Método `NotifyPickedUp()` que:
  - Reproduce el sonido de recogida.
  - Oculta el sprite.
  - Desactiva el collider.
  - Destruye el GameObject después de que termine el sonido (evita destrucción inmediata).

#### Coin y Heart

Ambos heredan o usan el sistema de `Drop`:

- **Coin**: Salta hacia arriba al aparecer con `PlayJump()`.
- **Heart**: Similar comportamiento, restaura vida al jugador.

#### Algoritmo de Drops en Enemigos

Cuando un enemigo muere:

1. Genera un número aleatorio (0-1).
2. Compara con `dropProbabilityA`:
   - Si es menor → Drop A.
3. Si falla, compara con `dropProbabilityB`:
   - Si es menor → Drop B.
4. Si falla, compara con `dropProbabilityC`:
   - Si es menor → Drop C.
5. Si todo falla → Usa un drop por defecto si está configurado.

---

### 8. Sistema de Puertas y Llaves

**Ubicación**: `Assets/Kits/GamePlayObjects/Doors/`

#### Door (DoorController)

Componente complejo para puertas:

- **Estados**:
  - `isOpen`: Puerta abierta o cerrada.
  - `isOpening` / `isClosing`: Estados transitorios durante animaciones.
  - `isLocked`: Puerta cerrada con llave.

- **Sistema de Llaves**:
  - `requiredKeyId`: ID de la llave necesaria (ej: "Level1Key").
  - Método `RequestOpen(string keyId)`:
    - Si está con llave, verifica que el keyId coincida (case-insensitive).
    - Si coincide, desbloquea y abre.
    - Si no coincide, reproduce sonido de bloqueo y devuelve false.

- **Animación**:
  - Usa triggers del Animator: "TransitionOpen", "TransitionClose".
  - Estados idle: "OpenIdle", "CloseIdle".
  - Se sincroniza con el Animator para activar/desactivar el collider bloqueante.

- **Collider Bloqueante**:
  - Busca un hijo llamado "BlockCollider".
  - Se activa cuando está cerrada, se desactiva cuando está abierta.

#### Key

Componente simple para llaves:

```csharp
[SerializeField] private string keyId;  // ID único de la llave
[SerializeField] private AudioClip pickupSfx;
```

- Al recogerla:
  - Busca puertas cercanas con `FindObjectsByType<DoorController>`.
  - Filtra la que tenga el mismo `requiredKeyId`.
  - Llama a `door.RequestOpen(keyId)`.
  - Reproduce sonido y se destruye (con delay para permitir reproducción del audio).

---

### 9. Configuración del Animator

El proyecto utiliza un sistema de Animator con múltiples capas:

#### Base Layer (Locomoción)
- Blend Tree 2D Simple Directional con parámetros `MoveX` y `MoveY`.
- Estados para cada dirección: Front, Back, Left, Right.
- Transiciones automáticas según el vector de movimiento.

#### Action Layer
- Overlay sobre Base Layer con peso 1.
- Estados: Attack, Hurt, Death.
- Transiciones desde Any State con triggers específicos.
- **Death no tiene transición de salida** (estado final).
- Para resetear después de Death se usa `animator.Rebind()`.

---

### 10. Sistema de Daño y Vida

#### Life Component

**Ubicación**: `Assets/Kits/Systems/LifeSystem/Scripts/Life.cs`

```csharp
public class Life : MonoBehaviour
{
    [SerializeField] float startingLife = 1f;
    [SerializeField] float currentLife;
    
    public UnityEvent<float> OnLifeChanged;
    public UnityEvent OnDeath;
    
    public void OnHitReceived(float damage) { ... }
    public void RecoverHealth(float amountHealth) { ... }
    public void Respawn() { ... }
}
```

- **OnHitReceived**: Reduce vida y dispara eventos.
- **RecoverHealth**: Solo funciona si `currentLife > 0` (no revive muertos).
- **Respawn**: Restaura vida al máximo y dispara `OnLifeChanged`.

---

### 11. Sistema de Detección (IVisible2D)

Interfaz para que los personajes se detecten entre sí:

```csharp
public interface IVisible2D
{
    enum Side { Neutrals, Allies, Enemies }
    
    Side GetSide();
    int GetPriority();
}
```

Utilizado por el `SenseSystem` para filtrar qué entidades puede ver/atacar cada personaje.

---

### 12. Buenas Prácticas Implementadas

#### Audio
- ✅ Todos los sonidos usan `AudioManager.SfxVolume`.
- ✅ `PlayOneShot` en lugar de `PlayClipAtPoint` (mejor control).
- ✅ Destrucción retrasada para permitir reproducción del audio.

#### Animator
- ✅ Uso de `Animator.StringToHash()` para optimización.
- ✅ Reset completo con `Rebind()` tras eventos críticos.

#### Persistencia
- ✅ `DontDestroyOnLoad` para elementos globales.
- ✅ Singleton pattern con protección contra duplicados.

#### Componentes
- ✅ Auto-detección de componentes en `Awake()`.
- ✅ Fallback cuando no se encuentran componentes opcionales.

#### Eventos
- ✅ UnityEvents para desacoplamiento.
- ✅ Suscripción/desuscripción correcta en `OnEnable`/`OnDisable`.

---

### 13. Flujo de Juego Completo

1. **Inicio**: Carga MainMenu → AudioManager se crea automáticamente.
2. **Play**: Carga escena Home → PlayerPersistence posiciona al jugador → HUD se conecta automáticamente.
3. **Exploración**: Jugador recoge llaves, monedas, hearts.
4. **Combate**: Enemigos detectan al jugador → Atacan → Jugador contraataca → Enemigos sueltan drops.
5. **Puertas**: Jugador interactúa con puerta → Si tiene llave correcta, se abre → Transición a nuevo nivel.
6. **Muerte**: Animación de muerte → Carga Home → Respawn con vida completa → Conserva todo el inventario.

---

### 14. ShopKeeper - Sistema de Diálogo

El ShopKeeper es un personaje interactivo que muestra diálogos animados cuando el jugador se acerca a él.

#### Scripts Utilizados

- **ShopKeeper.cs**: Detecta al jugador y gestiona la apertura/cierre del canvas de diálogo.
- **DialogueTypewriter.cs**: Muestra el texto letra a letra con sonido opcional en cada letra.

#### Estructura Recomendada del GameObject

```
ShopKeeper (GameObject)
├── Collider2D (Circle/Box - DEBE SER TRIGGER)
├── ShopKeeper.cs (Script)
├── Canvas_Dialogue (Canvas)
│   ├── Text_DialogueContent (TextMeshProUGUI)
│   │   └── DialogueTypewriter.cs (Script)
│   └── Image_NextButton (Image)
│       └── (opcional) Text (TextMeshProUGUI)
└── AudioSource (para reproducir sonidos)
```

#### Pasos de Configuración en Unity

**1. Preparar el ShopKeeper (Personaje principal)**
- En el Inspector del ShopKeeper:
  - Verifica que tenga un **Collider2D** (Circle o Box)
  - **IMPORTANTE**: Marca el Collider2D como **"Is Trigger" = TRUE**
  - Asegúrate de que el ShopKeeper tenga el tag **"ShopKeeper"** (opcional, pero recomendado)

**2. Configurar el Canvas de Diálogo**
- Crea en la jerarquía un Canvas hijo del ShopKeeper (o como hijo del Canvas principal, según tu estructura)
- Nómbralo: `Canvas_Dialogue`
- Configura su tamaño y posición según necesites

**3. Crear el TextMeshProUGUI para el texto**
- Añade un **TextMeshProUGUI** como hijo del Canvas_Dialogue
- Nómbralo: `Text_DialogueContent`
- Ajusta el tamaño y el contenido del texto
- **IMPORTANTE**: Añade el script **DialogueTypewriter.cs** a este GameObject

**4. Configurar el DialogueTypewriter en el TextMeshProUGUI**
En el Inspector del TextMeshProUGUI con DialogueTypewriter.cs:
- **Text Display**: Arrastra el mismo TextMeshProUGUI aquí (puede auto-referenciarse)
- **Delay Between Letters**: Ajusta el retardo (por ej: 0.05 segundos)
- **Typing Sound**: Arrastra el clip de audio para el sonido de tipeo
- **Sound Pitch**: Ajusta el pitch del sonido (ej: 1.0)

**5. Crear la imagen para cerrar el diálogo**
- Añade una **Image** como hijo del Canvas_Dialogue
- Nómbralo: `Image_NextButton` o `nextButtonImage`
- Asigna el sprite que desees (la imagen será clickeable)
- Configura su posición (ej: esquina inferior derecha)
- (Opcional) Añade un TextMeshProUGUI hijo para etiquetar la imagen (ej: "Siguiente", "OK")

**6. Añadir el script ShopKeeper al personaje principal**
En el Inspector del ShopKeeper GameObject:
- Añade el script **ShopKeeper.cs**
- En sus propiedades, arrastra los elementos así:
  - **Dialogue Canvas**: El Canvas_Dialogue
  - **Dialogue Typewriter**: El TextMeshProUGUI con DialogueTypewriter.cs
  - **Next Button Image**: La Image que creaste en el paso 5
  - **Welcome Message**: El texto que quieres mostrar (ej: "¡Bienvenido a mi tienda!")

**7. Configurar el AudioSource**
- El ShopKeeper debe tener un **AudioSource** en el mismo GameObject
- Si no lo tiene, el script lo crea automáticamente
- Este AudioSource reproduce los sonidos de tipeo

**8. Tag del Jugador (Importante)**
- Asegúrate de que el GameObject del jugador tenga el tag **"Player"**
- Esto permite que el ShopKeeper detecte cuando se acerca el jugador

#### Propiedades Personalizables (SerializeField)

**En ShopKeeper.cs:**
- `dialogueCanvas`: El Canvas que se activa al acercarse
- `dialogueTypewriter`: La referencia al script DialogueTypewriter
- `nextButtonImage`: La Image clickeable para cerrar el diálogo
- `welcomeMessage`: El texto que se muestra (STRING, personalizable)

**En DialogueTypewriter.cs:**
- `delayBetweenLetters`: Retardo entre letras (en segundos)
- `typingSound`: El clip de audio para el sonido de tipeo
- `soundPitch`: El pitch del sonido

#### Funcionamiento

1. **Detección de proximidad**: Cuando el jugador entra en el trigger del ShopKeeper, el diálogo se abre automáticamente
2. **Texto letra a letra**: El texto aparece letra a letra con el retardo configurado
3. **Sonido de tipeo**: Cada letra reproduce el sonido (si está asignado)
4. **Cerrar diálogo**: El jugador puede:
   - Presionar el botón "Siguiente"
   - Salir del área del trigger (si está fuera del alcance)

#### Ejemplo de Mensaje Personalizado (Opcional)

Si quieres cambiar el mensaje en código:

```csharp
shopKeeper.SetWelcomeMessage("¡Hola! ¿Necesitas algo?");
```

#### Notas Importantes

- Asegúrate de que el Collider2D sea un **TRIGGER** (Is Trigger = TRUE)
- El TextMeshProUGUI debe estar dentro del proyecto (TextMesh Pro ya está en tu proyecto)
- El AudioClip debe estar asignado si quieres el sonido de tipeo
- Si no asignas referencias en el Inspector, el script intentará buscarlas automáticamente con GetComponent

---

### 15. Sistema de Inventario

El sistema de inventario permite gestionar los objetos interactuables del juego para el jugador, el baúl de la Home y el Shopkeeper.

#### Estructura

- **Item.cs (ScriptableObject):**
  - Define cada elemento interactuable del juego (llaves, pociones, equipo, objetos varios).
  - Permite configurar nombre, descripción, sprites, prefab, precios, tipo de uso y si es apilable.
  - Los items se crean como assets en el editor y se reutilizan en todos los inventarios.

- **Inventory.cs (Componente):**
  - Permite añadir, quitar, consultar y listar items.
  - Se puede añadir a cualquier GameObject que necesite inventario (Player, baúl, Shopkeeper).
  - Gestiona una colección de items (sin stacks en esta versión).
  - Expone eventos para actualizar UI o lógica externa.

#### Interfaz y Almacenamiento (Store/Storage)

- El baúl (Store) tiene un canvas propio (InventoryCanvas) con 25 slots visuales.
- Al abrir el baúl, se muestra automáticamente su ventana de almacenamiento.
- El jugador puede abrir su propio inventario (`I`) y tener ambas ventanas abiertas a la vez.
- El intercambio de objetos se realiza mediante drag & drop entre slots:
  - Solo se permite mover objetos a slots vacíos.
  - El objeto solo se elimina del inventario origen si la transferencia es válida.
  - Si el slot destino está ocupado o no es válido, no ocurre nada.
- El canvas del baúl se cierra automáticamente al salir del área de acción.
- El sistema es extensible para otros contenedores (ej: Shopkeeper).

#### Implementación Técnica

- **Store.cs**: Controla la apertura/cierre del canvas del baúl y reproduce sonidos/animaciones.
- **StoreUI.cs**: Gestiona la UI del baúl, refresca los slots y coordina el drag & drop.
- **StoreSlotUI.cs**: Script para cada slot del baúl, implementa interfaces de drag & drop de Unity.
- **Inventory.cs**: Gestiona la lógica de añadir/quitar objetos, tanto para el jugador como para el baúl.
- **PlayerInventoryUI.cs**: No se modifica, sigue gestionando el inventario del jugador como hasta ahora.

**Flujo de interacción:**
1. El jugador entra en el área del baúl → Store.cs muestra el canvas y refresca la UI.
2. El jugador puede abrir su inventario (`I`) y ver ambos a la vez.
3. Al arrastrar un objeto de un slot a otro (drag & drop):
   - Se comprueba si el slot destino está vacío.
   - Si es válido, se transfiere el objeto usando los métodos de Inventory.cs.
   - Si no, no ocurre nada y el objeto permanece en su lugar.
4. Al salir del área del baúl, Store.cs oculta el canvas del baúl.

**Notas:**
- El sistema está preparado para ser reutilizado en el Shopkeeper, donde además se podrá implementar lógica de compra-venta.
- La persistencia del contenido del baúl se mantiene entre escenas y muertes del jugador.
- El canvas del baúl es hijo del objeto Store y puede convertirse en prefab para reutilizar en otras casas o niveles.

---

### 15.1. Sistema de Drag & Drop Visual (InventoryDragVisual)

El sistema de inventario incluye un icono visual de arrastre para mejorar la experiencia de usuario al mover objetos entre slots (Player ↔ Store, etc.).

#### ¿Qué es InventoryDragVisual?
- Es un objeto UI (Image) que sigue el cursor mientras arrastras un objeto.
- Solo necesitas **uno** por cada Canvas principal que contenga inventarios.
- El script `InventoryDragVisual` se encuentra en `Assets/Kits/Systems/DragDropSystem/Scripts/InventoryDragVisual.cs`.

#### Cómo configurarlo (paso a paso):
1. **Selecciona tu Canvas principal** (el que contiene los slots de inventario del Player y/o Store).
2. **Crea el objeto visual de arrastre:**
   - Haz clic derecho sobre el Canvas → UI → Image.
   - Renombra el objeto a `InventoryDragVisual`.
   - En el componente Image:
     - Deja el Sprite vacío.
     - Desactiva la opción "Raycast Target".
     - Pon el color en blanco (o el que prefieras para el icono).
   - Añade el componente `CanvasGroup`:
     - Desactiva "Interactable" y "Blocks Raycasts".
   - Añade el script `InventoryDragVisual` (ya incluido en el proyecto).
   - **No necesitas configurar ningún campo en el inspector**: el script obtiene el Image automáticamente.
   - Desactiva el objeto por defecto (en el inspector, desmarca el checkbox de "activo").
3. **Comprueba que está bajo el Canvas correcto**
   - Debe ser hijo directo del Canvas que contiene los slots de inventario.
4. **No necesitas duplicarlo**
   - Si Player y Store comparten Canvas, solo uno.
   - Si cada uno tiene su propio Canvas, uno en cada Canvas.

#### Funcionamiento
- Al arrastrar un objeto, el icono sigue el cursor y solo se elimina del slot si el drop es válido.
- Si sueltas fuera de un slot válido, el objeto permanece en su sitio y el icono desaparece.
- El sistema es robusto y no requiere configuración adicional en los scripts de slots.

#### Notas
- El script ya no tiene campos públicos ni requiere asignar referencias manualmente.
- Si quieres documentar el sistema, añade este apartado en la sección técnica del README.md (como aquí).

---

### 16. Sistema de Progresión y Persistencia de Historia (PlayerPrefs)

El juego implementa un sistema de progresión y persistencia temporal usando **PlayerPrefs** para guardar el estado de llaves, puertas y eventos clave durante la partida. Esto permite que, aunque el jugador cambie de escena o muera, el progreso (puertas abiertas, llaves recogidas, etc.) se mantenga hasta que termine la partida o vuelva al menú principal.

### ¿Qué es PlayerPrefs?
PlayerPrefs es un sistema de Unity para guardar datos simples (int, float, string) como pares clave-valor de forma persistente. Se usa aquí para marcar qué llaves han sido recogidas y qué puertas han sido abiertas.

### ¿Qué se guarda?
- **Llaves recogidas**: Cada llave tiene un ID único (por ejemplo, `Key_Home`, `Key_Level2`, `Key_Level3`). Cuando se recoge una llave, se guarda `PlayerPrefs.SetInt("key_collected_Key_Home", 1)`.
- **Puertas abiertas**: Cada puerta tiene un ID único (por ejemplo, `HomeLevel1Door`, `Level1Level2Door`, `Level2Level3Door`). Cuando se abre una puerta, se guarda `PlayerPrefs.SetInt("door_opened_HomeLevel1Door", 1)`.
- **Enemigos clave**: Los enemigos que sueltan llaves (OrcStrong, VampireStrong) solo sueltan la llave si el PlayerPrefs correspondiente está a 0. Si ya la tienes, sueltan monedas.
- **Reset de progreso**: Al derrotar al jefe final (ThugStrong), se borra todo el progreso con `PlayerPrefs.DeleteAll()` y se vuelve al menú principal.

### Ejemplo de claves utilizadas
- `key_collected_Key_Home` = 1 si la llave de Home ha sido recogida.
- `key_collected_Key_Level2` = 1 si la llave del Level2 ha sido recogida.
- `key_collected_Key_Level3` = 1 si la llave del Level3 ha sido recogida.
- `door_opened_HomeLevel1Door` = 1 si la puerta de Home a Level1 está abierta.
- `door_opened_Level1Level2Door` = 1 si la puerta de Level1 a Level2 está abierta.
- `door_opened_Level2Level3Door` = 1 si la puerta de Level2 a Level3 está abierta.

### Funcionamiento resumido
- **Llaves**: Si una llave ya fue recogida (PlayerPrefs=1), no vuelve a aparecer en la escena.
- **Puertas**: Si una puerta ya fue abierta (PlayerPrefs=1), permanece abierta para siempre.
- **Enemigos clave**: Solo sueltan la llave si no la tienes; si ya la tienes, sueltan monedas.
- **Jefe final**: Al derrotarlo, se borra todo el progreso y se vuelve al menú principal.
- **Al volver al menú principal y empezar de nuevo**: Todo el progreso se reinicia y la partida comienza desde cero.

### Implementación en scripts
- **KeyController.cs**: Marca la llave como recogida en PlayerPrefs al cogerla y destruye el objeto si ya fue recogida.
- **DoorController.cs**: Marca la puerta como abierta en PlayerPrefs al abrirla y la deja abierta si ya lo estaba al cargar la escena.
- **BaseEnemy.cs**: Los enemigos clave sueltan la llave solo si no la tienes (según PlayerPrefs). El jefe final borra todo el progreso y lanza el menú principal.

### Ventajas
- Permite una progresión coherente durante la partida.
- Evita que el jugador tenga que volver a abrir puertas o recoger llaves ya usadas.
- Sencillo de implementar y mantener para juegos de este tipo.

---

## 📝 Notas Finales

### Control de Versiones

Se recomienda usar `.gitignore` para Unity excluyendo:
- `Library/`
- `Temp/`
- `Obj/`
- `Build/`
- `*.csproj`
- `*.sln`

### Optimizaciones Futuras

- Pooling de enemigos y proyectiles.
- Sistema de guardado más complejo (save/load).
- Más tipos de enemigos con IA variada.
- Boss fights con mecánicas únicas.
- Sistema de mejoras y power-ups permanentes.

---

**¡Disfruta del camino de Kamael!** ⚔️
