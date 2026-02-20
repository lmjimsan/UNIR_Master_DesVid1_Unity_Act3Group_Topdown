using UnityEngine;

public class Coin : MonoBehaviour, IVisible2D
{
    enum State { CoinBlink, CoinRotate, CoinRotateJump }
    [SerializeField] State state = State.CoinBlink;
    [SerializeField] int priority = 0;
    [SerializeField] IVisible2D.Side side = IVisible2D.Side.Neutrals;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animator.Play(state.ToString());
    }

    void SetState(State newState)
    {
        state = newState;
        animator.Play(state.ToString());
    }

    public void PlayJump()
    {
        SetState(State.CoinRotateJump);
    }

    int IVisible2D.GetPriority()
    {
        return priority;
    }

    IVisible2D.Side IVisible2D.GetSide()
    {
        return side;
    }
}
