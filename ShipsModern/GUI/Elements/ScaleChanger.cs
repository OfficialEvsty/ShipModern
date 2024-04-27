using System;
using System.Windows;

namespace ShipsForm.GUI.Elements
{
    /// <summary>
    /// This entity manages scaling over the map.
    /// </summary>
    static class ScaleChanger
    {
        private static Size m_screenSize = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        private static float i_max_scale = 10f;
        private static float i_min_scale = 1f;
 /*           (m_screenSize.Width > m_screenSize.Height)
            ? (int)Math.Ceiling(m_screenSize.Width / m_screenSize.Height)
            : (int)Math.Ceiling(m_screenSize.Height / m_screenSize.Width);*/
        private static float i_scale = i_min_scale;
        private static float i_minimum_scale_step = 1f;
        

        public static float Scale { get { return i_scale; } set { i_scale = value; } }

        /// <summary>
        /// This action notify all GUI entities about scale has changed.
        /// </summary>
        public static Action? OnChangeScale;
        public static void IncreaseScale()
        {
            i_scale = Math.Min(i_scale + i_minimum_scale_step, i_max_scale);
            OnChangeScale?.Invoke();
        }

        public static void DecreaseScale()
        {
            i_scale = Math.Max(i_scale - i_minimum_scale_step, i_min_scale);
            OnChangeScale?.Invoke();
        }
    }
}
