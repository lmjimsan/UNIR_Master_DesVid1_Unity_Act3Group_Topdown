using UnityEngine;

public class Hits : MonoBehaviour
{
    [SerializeField] string affectedTag = "Enemy";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(affectedTag))
        {
            // Debug.Log($"{collision.name} was hit!");
        }
    }
}