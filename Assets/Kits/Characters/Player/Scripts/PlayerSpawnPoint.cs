using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnId = "Default";
    [SerializeField] private bool isDefault = true;

    public string SpawnId => string.IsNullOrWhiteSpace(spawnId) ? null : spawnId.Trim();
    public bool IsDefault => isDefault;
}
