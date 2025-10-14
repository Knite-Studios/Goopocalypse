using TMPro;
using UnityEngine;

namespace UI
{
    public class LBPlayerHolder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private TextMeshProUGUI playerScore;
        [SerializeField] private TextMeshProUGUI playerRank;

        public void SetPlayerData(string name, int score, int rank)
        {
            playerRank.text = rank.ToString();
            playerName.text = name;
            playerScore.text = score.ToString();
        }

        public void ClearText()
        {
            playerRank.text = "";
            playerName.text = "";
            playerScore.text = "";
        }
    }
}