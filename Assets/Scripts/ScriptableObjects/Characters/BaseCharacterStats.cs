using UnityEngine;

[CreateAssetMenu]
public class BaseStats : ScriptableObject
{
    public new string name;
    public int vitality = 3;
    public int strength = 3;
    public int endurance = 3;
    public int hitRolls = 1;
    public int scoreToHit = 5;
    public int damages = 1;
}
