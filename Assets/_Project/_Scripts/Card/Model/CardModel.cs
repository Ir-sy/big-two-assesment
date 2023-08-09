using UnityEngine;

namespace BigTwo.Card
{
    [CreateAssetMenu(fileName = "New Card Model", menuName = "Big Two/Card/Card Model")]
    public class CardModel : ScriptableObject
    {
        public CardSuitEnum Suit;
        public CardValueEnum Value;
        public Sprite CardImage;
    }
}