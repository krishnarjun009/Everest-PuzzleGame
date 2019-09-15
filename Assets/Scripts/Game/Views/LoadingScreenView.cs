using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Iniectio.Lite;
using UnityEngine.UI;

namespace Everest.PuzzleGame
{
    public class LoadingScreenView : View
    {
        [SerializeField] private Slider m_Slider;
        [SerializeField] private Text m_ProgressText;

        [Listen(typeof(UpdateProgressBarSignal))]
        private void OnUpdateProgress(float value)
        {
            m_Slider.value = value;
            m_ProgressText.text = (value * 100f) + "%";
        }
    }
}