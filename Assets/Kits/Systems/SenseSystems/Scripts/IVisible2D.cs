using UnityEngine;

public interface IVisible2D
{
    enum Side { PlayFriends, Enemies, Neutrals } 

    public int GetPriority();
    public Side GetSide();
}
