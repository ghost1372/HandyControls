using System;

namespace HandyControl.Controls
{
    public class QueryTextFromValueEventArgs : EventArgs
    {
        public QueryTextFromValueEventArgs(object value, string text)
        {
            m_value = value;
            m_text = text;
        }

        #region Value Property

        private object m_value;

        public object Value
        {
            get { return m_value; }
        }

        #endregion Value Property

        #region Text Property

        private string m_text;

        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        #endregion Text Property
    }
}
