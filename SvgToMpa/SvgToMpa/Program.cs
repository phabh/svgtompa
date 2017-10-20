using Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SvgToMpa
{
    class Program
    {
        static Regex regex = new Regex(@"^id(?<flow>\d*\.?\d*?\.?\d*?)_(?<tree>\d*\.?\d*?\.?\d*?)_(?<type>Text)");

        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader("D:\\Frame.svg", Encoding.UTF8);

            var svgFile = SvgDocument.Open<SvgDocument>(reader.BaseStream);

            var elements = svgFile.Descendants().Where(e => e.ID != null && regex.IsMatch(e.ID)).ToList();

            var blocks = GetBlocks(elements);


        }

        private static IEnumerable<BlockText> GetBlocks(IEnumerable<SvgElement> elements)
        {
            return elements.Descendants().Where(e => e.ID != null && e.ID.Contains("#t="))
                .Select(e => 
                        {
                            var match = regex.Match(e.Parent.ID);

                            return new BlockText()
                            {
                                NavigationName = match.Groups["flow"].Value,
                                FlowPosition = match.Groups["tree"].Value,
                                Type = match.Groups["type"].Value,
                                Text =  Encoding.UTF8.GetString(Encoding.Default.GetBytes(e.ID.Substring(3)))
                            };
                        });
        }


    }
}
