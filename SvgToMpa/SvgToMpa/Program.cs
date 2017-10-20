using Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SvgToMpa
{
    class Program
    {
        static void Main(string[] args)
        {
            var svgFile = SvgDocument.Open("D:\\Frame.svg");

            var regex = new Regex(@"^id(?<flow>\d*\.?\d*?\.?\d*?)_(?<tree>\d*\.?\d*?\.?\d*?)_(?<type>Text)");

            var elements = svgFile.Descendants().Where(e => e.ID != null && regex.IsMatch(e.ID) ).ToList();
         
            elements[0].Children



        }
    }
}
