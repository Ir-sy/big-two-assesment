using UnityEngine;

namespace BigTwo.Character
{
    [CreateAssetMenu(fileName = "New Character Model", menuName = "Big Two/Character/Character Model")]
    public class CharacterModelSO : ScriptableObject
    {
        public string Name;
        public Sprite HappySprite;
        public Sprite AngrySprite;
        public Color TintColour;

        public CharacterModel ToCharacterModel()
        {
            return new CharacterModel(Name, HappySprite, AngrySprite, TintColour);
        }
    }
}