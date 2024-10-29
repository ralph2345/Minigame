using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "MemoryGame/Level")]
public class Level : ScriptableObject
{
    public int levelNumber;
    public int numberOfCards; 
    public int maxMoves;
    
}
