using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellerMapGraphs
{
    class ListOrString
    {
        public string Text => TextList.First();
        public List<string> TextList;

        public ListOrString(string text)
        {
            TextList = new List<string>();
            TextList.Add(text);
        }

        public ListOrString(List<string> text)
        {
            TextList = text;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var t in TextList)
            {
                sb.Append(t + " ");
            }

            return sb.ToString();
        }
    }
}