using BigTwo.Character;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Collection", menuName = "Big Two/Character/Character Collection")]
public class CharacterModelCollectionSO : ScriptableObject
{
    public List<CharacterModelSO> Characters = new List<CharacterModelSO>();
}