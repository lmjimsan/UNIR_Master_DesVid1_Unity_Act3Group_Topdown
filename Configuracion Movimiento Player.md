# CONFIGURACIÓN MOVIMIENTO PLAYER

## RESUMEN
Este documento describe la configuración técnica del sistema de animación y movimiento del personaje jugador en Unity, incluyendo parámetros del Animator, configuración de scripts y ajustes de transiciones.

---

## PARÁMETROS DE VELOCIDAD

### `linearspeed` (BaseCharacter)
**Tipo:** `float` (SerializeField protected)  
**Ubicación:** Componente BaseCharacter en el Inspector  
**Función:** Define la velocidad de movimiento base del personaje al caminar.

**Interpretación:**
- Es la velocidad real en unidades/segundo cuando el jugador camina sin correr.
- Se multiplica por el `runMultiplier` cuando el jugador corre.
- Afecta directamente al desplazamiento físico del GameObject.
- Afecta indirectamente al parámetro `Speed` del Animator.

**Ejemplo:**
- `linearspeed = 1.5` → el personaje se mueve a 1.5 unidades/seg al caminar.
- Con `runMultiplier = 2` → corre a 3 unidades/seg.

---

### `runMultiplier` (PlayerCharacter)
**Tipo:** `float` (SerializeField)  
**Ubicación:** Componente PlayerCharacter en el Inspector  
**Función:** Multiplicador aplicado a `linearspeed` cuando el jugador pulsa el botón de correr.

**Interpretación:**
- `runMultiplier = 2` → velocidad de correr es el doble de la velocidad de caminar.
- Velocidad real de Run = `linearspeed × runMultiplier`
- No afecta al parámetro del Animator directamente; se aplica primero al movimiento físico.

**Ejemplo:**
- `linearspeed = 1.5`, `runMultiplier = 2` → Walk = 1.5, Run = 3.

---

### `speedParamScale` (PlayerCharacter)
**Tipo:** `float` (SerializeField)  
**Ubicación:** Componente PlayerCharacter en el Inspector  
**Función:** Factor de escala aplicado al parámetro `Speed` del Animator.

**Interpretación:**
- Ajusta el valor enviado al Animator sin cambiar la velocidad real del movimiento.
- Útil cuando los umbrales del Blend Tree no coinciden con la velocidad física.
- Fórmula: `Speed (Animator) = moveInput.magnitude × linearspeed × speedMultiplier × speedParamScale`

**Ejemplo:**
- Si `linearspeed = 2` pero el umbral de Walk es 1.5:
  - Pon `speedParamScale = 0.75` para que 2 × 0.75 = 1.5.

---

### `moveDeadZone` (PlayerCharacter)
**Tipo:** `float` (SerializeField)  
**Ubicación:** Componente PlayerCharacter en el Inspector  
**Función:** Umbral mínimo para ignorar movimientos muy pequeños del input.

**Interpretación:**
- Evita que el personaje "tiemble" con inputs de joystick muy pequeños.
- Evita que la dirección cambie cuando el input es casi cero.
- Para teclado puro, puede dejarse en 0.

---

## PARÁMETROS DEL ANIMATOR

### Parámetros necesarios (definidos en el Animator Controller)

| Parámetro | Tipo | Función |
|-----------|------|---------|
| `MoveX` | float | Dirección horizontal normalizada (-1 a 1) |
| `MoveY` | float | Dirección vertical normalizada (-1 a 1) |
| `Speed` | float | Magnitud de la velocidad de movimiento |
| `IsRunning` | bool | Indica si el jugador está corriendo |
| `Attack` | trigger | Dispara la animación de ataque |
| `Hurt` | trigger | Dispara la animación de daño |
| `Death` | trigger | Dispara la animación de muerte |

**Cómo funcionan:**
- **MoveX/MoveY**: Usados por Blend Trees 2D para seleccionar la dirección (Up/Down/Left/Right).
- **Speed**: Usado por Blend Trees 1D para seleccionar Idle/Walk/Run según umbrales.
- **IsRunning**: Opcional; puede usarse para lógica adicional (actualmente no se usa en los Blend Trees).
- **Triggers**: Activan transiciones desde `Any State` a estados de acción.

---

## CONFIGURACIÓN DE BLEND TREES

### Locomotion (Base Layer)
**Tipo:** Blend Tree 1D  
**Parámetro:** `Speed`  

**Umbrales recomendados:**
- Idle Tree: `0`
- Walk Tree: `1.5`
- Run Tree: `3`

Cada sub-árbol (Idle/Walk/Run) es un **Blend Tree 2D Simple Directional** con:
- Parameter X: `MoveX`
- Parameter Y: `MoveY`

**Posiciones direccionales:**
- Front/Down: `(0, -1)`
- Back/Up: `(0, 1)`
- Left: `(-1, 0)`
- Right: `(1, 0)`

---

### Attack (Action Layer)
**Tipo:** Blend Tree 1D anidado  
**Parámetro:** `Speed`

**Umbrales:**
- Attack Idle Tree: `0`
- Attack Walk Tree: `1.5`
- Attack Run Tree: `3`

Cada sub-árbol es un **Blend Tree 2D Simple Directional** con `MoveX/MoveY`.

**Clips por dirección:**
- `Player_Lvl1_Front_Attack` / `Walk_Attack` / `Run_Attack`
- `Player_Lvl1_Back_Attack` / etc.
- `Player_Lvl1_Left_Attack` / etc.
- `Player_Lvl1_Right_Attack` / etc.

---

### Hurt (Action Layer)
**Tipo:** Blend Tree 2D Simple Directional  
**Parámetros:** `MoveX`, `MoveY`

**Clips:**
- `Player_Lvl1_Front_Hurt` (0, -1)
- `Player_Lvl1_Back_Hurt` (0, 1)
- `Player_Lvl1_Left_Hurt` (-1, 0)
- `Player_Lvl1_Right_Hurt` (1, 0)

---

### Death (Action Layer)
**Tipo:** Blend Tree 2D Simple Directional  
**Parámetros:** `MoveX`, `MoveY`

**Clips:**
- `Player_Lvl1_Front_Death` (0, -1)
- `Player_Lvl1_Back_Death` (0, 1)
- `Player_Lvl1_Left_Death` (-1, 0)
- `Player_Lvl1_Right_Death` (1, 0)

---

## CONFIGURACIÓN DE TRANSICIONES (ACTION LAYER)

### Any State → Attack
**Configuración:**
- **Has Exit Time:** OFF
- **Transition Duration:** 0
- **Can Transition To Self:** OFF
- **Interruption Source:** None
- **Conditions:** `Attack` (Trigger)

**Propósito:** Permite iniciar ataque desde cualquier estado sin interrumpirse a sí mismo.

---

### Attack → Exit
**Configuración:**
- **Has Exit Time:** ON
- **Exit Time:** 1.0
- **Transition Duration:** 0
- **Interruption Source:** None
- **Conditions:** ninguna

**Propósito:** El ataque se completa antes de volver al estado Empty. Esto evita que el ataque se corte a mitad.

---

### Any State → Hurt
**Configuración:**
- **Has Exit Time:** OFF
- **Transition Duration:** 0
- **Conditions:** `Hurt` (Trigger)

---

### Hurt → Exit
**Configuración:**
- **Has Exit Time:** ON
- **Exit Time:** 1.0
- **Conditions:** ninguna

---

### Any State → Death
**Configuración:**
- **Has Exit Time:** OFF
- **Transition Duration:** 0
- **Conditions:** `Death` (Trigger)
- **SIN SALIDA** (no hay transición Death → Exit)

**Propósito:** Death es un estado final y no vuelve a locomotion.

---

## CONFIGURACIÓN DEL CÓDIGO (PlayerCharacter.cs)

### Input Actions necesarias
En el Inspector del componente `PlayerCharacter`, asigna las siguientes referencias:

1. **move** (InputActionReference): Acción de movimiento WASD.
2. **run** (InputActionReference): Acción de correr (botón mantenido).
3. **attack** (InputActionReference): Acción de ataque (botón pulsado).

---

### Lógica de actualización de parámetros del Animator

**En cada frame (Update):**
```csharp
// Se actualiza la dirección (MoveX, MoveY) solo si no hay acción activa
// Durante Attack/Hurt/Death se mantiene la última dirección válida

animator.SetFloat(MoveX, lastMove.x);
animator.SetFloat(MoveY, lastMove.y);
animator.SetFloat(Speed, moveInput.magnitude * linearspeed * speedMultiplier * speedParamScale);
animator.SetBool(IsRunning, isRunning);
```

---

### Bloqueo de ataques consecutivos

**Método `CanAttack()`:**
- Comprueba si el personaje está en transición.
- Comprueba si está en Death o Hurt (bloquea ataque).
- Comprueba si ya está atacando y la animación no ha terminado (`normalizedTime < 1f`).

**Resultado:** El jugador no puede disparar un nuevo ataque hasta que el anterior termine completamente.

---

### Mantenimiento de dirección durante acciones

**Método `IsActionActive()`:**
- Detecta si el personaje está en Attack, Hurt o Death.
- Si es así, no se actualiza `lastMove`, manteniendo la dirección en la que estaba cuando comenzó la acción.

**Resultado:** El personaje no cambia de orientación a mitad de un ataque o daño.

---

## EJEMPLO DE CONFIGURACIÓN COMPLETA

### Inspector - BaseCharacter
- `linearspeed = 1.5`

### Inspector - PlayerCharacter
- `runMultiplier = 2`
- `speedParamScale = 1`
- `moveDeadZone = 0.01`
- `actionLayerName = "Action"`

### Animator - Blend Tree Locomotion (1D)
- Idle Tree: threshold `0`
- Walk Tree: threshold `1.5`
- Run Tree: threshold `3`

### Animator - Transiciones Action Layer
- Any State → Attack: Has Exit Time OFF, Can Transition To Self OFF
- Attack → Exit: Has Exit Time ON (Exit Time = 1)
- Any State → Hurt: Has Exit Time OFF
- Hurt → Exit: Has Exit Time ON
- Any State → Death: Has Exit Time OFF, SIN salida

---

## RESULTADO FINAL

Con esta configuración:
- Al caminar (sin run): `Speed = 1.5` → Locomotion usa Walk Tree.
- Al correr (con run): `Speed = 3` → Locomotion usa Run Tree.
- Al atacar: se dispara `Attack` trigger, se mantiene la dirección, y se completa la animación antes de volver a locomotion.
- Durante Hurt o Death, no se pueden iniciar nuevos ataques.
- La dirección (MoveX/MoveY) se mantiene durante acciones, evitando cambios de orientación bruscos.

---

## SOLUCIÓN DE PROBLEMAS

### Problema: Siempre usa animación de Run, nunca Walk
**Causa:** El parámetro `Speed` supera el umbral de Walk incluso al caminar.  
**Solución:**
- Baja `linearspeed` o `speedParamScale`.
- Ejemplo: `linearspeed = 1.5`, `speedParamScale = 1`.

### Problema: El ataque se corta o se reinicia antes de terminar
**Causa:** Transición Attack → Exit permite interrupciones o no tiene Exit Time.  
**Solución:**
- Attack → Exit: Has Exit Time = ON, Exit Time = 1, Interruption Source = None.
- Any State → Attack: Can Transition To Self = OFF.

### Problema: El personaje no corre aunque pulso el botón
**Causa:** La acción `run` no está asignada o no es tipo Button/Value.  
**Solución:**
- Asigna `InputActionReference` de run en el Inspector.
- Asegúrate de que `runMultiplier > 1`.

### Problema: Las animaciones no cambian de dirección
**Causa:** MoveX/MoveY no se actualizan o los Blend Trees 2D no están configurados.  
**Solución:**
- Verifica que los Blend Trees usan `MoveX/MoveY` como Parameters.
- Verifica las posiciones direccionales: Front (0,-1), Back (0,1), Left (-1,0), Right (1,0).

---

**Documento creado:** 4 de febrero de 2026  
**Autor:** GitHub Copilot  
**Proyecto:** 2026_RPG2D_LMJimene
