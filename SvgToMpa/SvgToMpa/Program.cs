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
        static Regex regex = new Regex(@"^id(?<flow>\d*\.?\d*?\.?\d*?)_(?<tree>\d*\.?\d*?\.?\d*?)_(?<type>(Text|Image|QuickReply)");

        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader("C:\\svg\\Inicio.svg", Encoding.UTF8);

            var svgFile = SvgDocument.Open<SvgDocument>(reader.BaseStream);

            var elements = svgFile.Descendants().Where(e => e.ID != null && regex.IsMatch(e.ID)).ToList();

            var blocks = GetBlocks<BlockText>(elements);


        }


        private static IEnumerable<T> GetBlocks<T>(IEnumerable<SvgElement> elements) where T : Block
        {
            if (typeof(T) is BlockText)
            {
                return elements.Descendants().Where(e => e.ID != null && e.ID.Contains("#t="))
                    .Select(e =>
                    {
                        return (new BlockText()
                        {
                            Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(e.ID.Substring(3)))
                        } as T);
                    });
            }
            else
            if (typeof(T) is BlockMenu)
            {
                var dictionaryMenu = new Dictionary<string, BlockMenu>();


                return elements.Descendants().Where(e => e.ID != null && ( e.ID.Contains("#t=") || e.ID.Contains("default") ) )
                    .Select(e =>
                    {
                        var nameMenu = e.Parents.Where(p => p.ID != null && regex.IsMatch(p.ID)).First();
                        var blockMenu = dictionaryMenu[nameMenu.ID];

                        if( blockMenu == null )
                        {
                            blockMenu = new BlockMenu();
                        }





                        return (new BlockText()
                        {
                            Text = Encoding.UTF8.GetString(Encoding.Default.GetBytes(e.ID.Substring(3)))
                        } as T);
                    });
            }
        }

        public static string GetEncodedText(string text)
        {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));
        }


    }
}
