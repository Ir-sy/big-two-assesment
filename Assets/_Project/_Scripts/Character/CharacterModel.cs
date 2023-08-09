using UnityEngine;

namespace BigTwo.Character
{
    public class CharacterModel
    {
        public string Name { get; private set; }
        public Sprite HappySprite { get; private set; }
        public Sprite AngrySprite { get; private set; }
        public Color TintColour { get; private set; }

        public CharacterModel(string name, Sprite happySprite, Sprite angrySprite, Color tintColour)
        {
            Name = name;
            HappySprite = happySprite;
            AngrySprite = angrySprite;
            TintColour = tintColour;
        }
    }
}