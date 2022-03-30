using System;

namespace HandyControl.Controls
{
    public class QueryValueFromTextEventArgs : EventArgs
    {
        public QueryValueFromTextEventArgs(string text, object value)
        {
            m_text = text;
            m_value = value;
        }

        #region Text Property

        private string m_text;

        public string Text
        {
            get { return m_text; }
        }

        #endregion Text Property

        #region Value Property

        private object m_value;

        public object Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        #endregion Value Property

        #region HasParsingError Property

        private bool m_hasParsingError;

        public bool HasParsingError
        {
            get { return m_hasParsingError; }
            set { m_hasParsingError = value; }
        }

        #endregion HasParsingError Property

    }
}
