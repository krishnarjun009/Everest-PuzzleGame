using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Everest.PuzzleGame
{
    public static class BoardUtils
    {
        private static Camera m_MainCamera = null;
        private static float rowConst = 0.225f, colConst = 0.235f;

        private static Color32[] colors = new Color32[] {
                
            new Color32(251, 133, 98, 255),
            new Color32(239, 66, 65, 255),
            new Color32(76, 188, 45, 255),
        };

        public static Color32 GetRandomColor()
        {
            return colors[UnityEngine.Random.Range(0, colors.Length)];
        }

        public static Color32 GetDefaultColor() => new Color32(255, 255, 255, 255);

        public static void Setup(Camera camera)
        {
            m_MainCamera = camera;
        }

        public static Vector3 GetScreenPointFromViewPort(int row, int col)
        {
            var position = m_MainCamera.ViewportToWorldPoint(
                new Vector3(rowConst * row, 1 - colConst * col, 0)
                );
            return position;
        }
    }
}
