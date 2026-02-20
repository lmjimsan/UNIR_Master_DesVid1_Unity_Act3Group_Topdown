using UnityEngine;

[CreateAssetMenu()]

public class DropDefinition : ScriptableObject
{
    public float healthRecovery;
    public float powerUpDamage;
    public float powerUpShield;
    public float powerUpDuration;
    public int coins;
}