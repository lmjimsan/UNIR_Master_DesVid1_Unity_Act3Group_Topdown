using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence instance;
    private static string nextSpawnId;
    [SerializeField] private bool debugSpawns = false;

    public static void SetNextSpawnId(string spawnId)
    {
        nextSpawnId = NormalizeSpawnId(spawnId);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Evita reubicar al jugador cuando se cargan escenas aditivas (HUD, etc.)
        if (scene != SceneManager.GetActiveScene())
        {
            return;
        }

        PlayerSpawnPoint[] spawns = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        if (spawns == null || spawns.Length == 0)
        {
            return;
        }

        PlayerSpawnPoint selected = null;
        if (!string.IsNullOrWhiteSpace(nextSpawnId))
        {
            for (int i = 0; i < spawns.Length; i++)
            {
                string spawnId = NormalizeSpawnId(spawns[i]?.SpawnId);
                if (spawnId != null && string.Equals(spawnId, nextSpawnId, System.StringComparison.OrdinalIgnoreCase))
                {
                    selected = spawns[i];
                    break;
                }
            }
        }

        if (selected == null)
        {
            for (int i = 0; i < spawns.Length; i++)
            {
                if (spawns[i] != null && spawns[i].IsDefault)
                {
                    selected = spawns[i];
                    break;
                }
            }
        }

        if (selected == null)
        {
            selected = spawns[0];
        }

        if (debugSpawns)
        {
            string selectedId = NormalizeSpawnId(selected != null ? selected.SpawnId : null);
            Debug.Log($"[PlayerPersistence] Scene '{scene.name}' spawn='{selectedId ?? "<null>"}' next='{nextSpawnId ?? "<null>"}' count={spawns.Length}");
        }

        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        if (rb2D != null)
        {
            rb2D.position = selected.transform.position;
            rb2D.linearVelocity = Vector2.zero;
        }
        else
        {
            transform.position = selected.transform.position;
        }

        nextSpawnId = null;
    }

    private static string NormalizeSpawnId(string spawnId)
    {
        if (string.IsNullOrWhiteSpace(spawnId)) return null;
        return spawnId.Trim();
    }
}
