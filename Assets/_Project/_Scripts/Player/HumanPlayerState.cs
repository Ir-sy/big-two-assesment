using BigTwo.GameLoop;

namespace BigTwo.Player
{
    public class HumanPlayerState : PlayerState
    {
        public HumanPlayerState(int playerIndex) : base(playerIndex)
        {
        }

        public override void Action(GameInfo gameInfo)
        {
            InvokeOnPlayerAction(gameInfo);
        }
    }
}