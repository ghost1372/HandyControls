using System;
using System.Collections.Generic;
using System.Text;

namespace HandyControl.Controls
{
    public enum NeonLabelType
    {
        FadeNext,
        /// <summary>
        /// Slide out current content , and slide in next content.
        /// </summary>
        SlideNext,
        /// <summary>
        /// Scroll to end
        /// </summary>
        ScrollToEnd,
    }
}
