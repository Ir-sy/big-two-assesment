using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BigTwo.UserInterface
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _gameResultText;
        [SerializeField]
        private Button _restartButton;

        private void Awake()
        {
            _restartButton.onClick.AddListener(OnRestartButtonPressed);
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(OnRestartButtonPressed);
        }

        private void OnRestartButtonPressed()
        {
            SceneManager.LoadScene(0);
        }

        public void Initialise(bool localPlayerWin)
        {
            string resultText = localPlayerWin ? "YOU WIN" : "YOU LOSE";

            _gameResultText.SetText(resultText);
        }
    }
}