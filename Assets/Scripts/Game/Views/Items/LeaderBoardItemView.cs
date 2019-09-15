using UnityEngine;
using UnityEngine.UI;

namespace Everest.PuzzleGame
{
    public class LeaderBoardItemView : MonoBehaviour
    {
        [SerializeField] private Text m_SerialNoText;
        [SerializeField] private Text m_NameText;
        [SerializeField] private Text m_BestScoreText;

        public void SetView(string sn, string name, string bestScore)
        {
            m_SerialNoText.text = sn;
            m_NameText.text = name;
            m_BestScoreText.text = bestScore;
        }
    }
}
