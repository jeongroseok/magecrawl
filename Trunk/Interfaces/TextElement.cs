namespace Magecrawl.Interfaces
{
    public class TextElement : INamedItem
    {
        private string m_name;

        public TextElement(string name)
        {
            m_name = name;
        }

        public string DisplayName
        {
            get
            {
                return m_name;
            }
        }
    }
}
