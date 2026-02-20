using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetSpawnId = "Default";
    [SerializeField] private bool requirePlayerTag = true;
    [SerializeField] private bool autoOpenDoorOnTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHandleTrigger(other, allowOpen: true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHandleTrigger(other, allowOpen: false);
    }

    private void TryHandleTrigger(Collider2D other, bool allowOpen)
    {
        if (requirePlayerTag && !other.CompareTag("Player")) return;

        DoorController door = GetDoor();

        if (door != null)
        {
            if (door.IsOpen)
            {
                Transition();
                return;
            }

            if (allowOpen && autoOpenDoorOnTrigger)
            {
                door.RequestOpen();
            }

            return;
        }

        Transition();
    }

    public void Transition()
    {
        if (string.IsNullOrWhiteSpace(targetSceneName)) return;

        if (!string.IsNullOrWhiteSpace(targetSpawnId))
        {
            PlayerPersistence.SetNextSpawnId(targetSpawnId);
        }

        SceneManager.LoadScene(targetSceneName);
    }

    private DoorController GetDoor()
    {
        DoorController door = GetComponentInParent<DoorController>();
        if (door != null) return door;

        return GetComponent<DoorController>();
    }
}
