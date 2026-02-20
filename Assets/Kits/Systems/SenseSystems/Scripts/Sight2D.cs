using UnityEngine;

public class Sight2D : MonoBehaviour
{
    [SerializeField] float radius = 5f;
    [SerializeField] float checkFrequency = 5f;
    [Space]
    [SerializeField] IVisible2D.Side[] perceivedSides;   
    float lastCheckTime = 0f;
    Collider2D[] colliders;

    Transform closestTarget;
    float distanceToClosestTarget;
    int pritorityOfClosestTarget;

    void Update()
    {
        if ((Time.time - lastCheckTime) > (1f / checkFrequency))
        {
            lastCheckTime = Time.time;
            closestTarget = null;
            distanceToClosestTarget = Mathf.Infinity;
            pritorityOfClosestTarget = -1;
            colliders = Physics2D.OverlapCircleAll(transform.position, radius);            

            // Los colliders no se devuelven por orden de cercania, hay que buscarlo
            for (int i = 0; i < colliders.Length; i++)
            {
                IVisible2D visible = colliders[i].GetComponent<IVisible2D>();
                if (visible != null && CanSee(visible))
                {
                    float distanceToTarget = Vector3.Distance(transform.position, colliders[i].transform.position);
                    if ((visible.GetPriority() > pritorityOfClosestTarget) ||
                        ((visible.GetPriority() == pritorityOfClosestTarget) && (distanceToTarget < distanceToClosestTarget)))
                    {
                        closestTarget = colliders[i].transform;
                        distanceToClosestTarget = distanceToTarget;
                        pritorityOfClosestTarget = visible.GetPriority();
                    }
                }
            }
        }
    }

    bool CanSee(IVisible2D visible)
    {
        bool canSee = false;

        for (int i = 0; !canSee && (i < perceivedSides.Length); i++)
        {
            canSee = visible.GetSide() == perceivedSides[i];
        }

        return canSee;
    }


    public Transform GetClosestTarget()
    {
        return closestTarget;
    }


    public bool IsPlayerInSight()
    {
        bool IsPlayerInSight = false;

        for (int i = 0; !IsPlayerInSight && (i<colliders.Length);i++)
        {
            if (colliders[i].CompareTag("Player"))
            {
                IsPlayerInSight = true;
            }
        }

        return IsPlayerInSight;
    }
}
