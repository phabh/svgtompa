using Newtonsoft.Json;
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
        static Regex regex = new Regex(@"^id(?<flow>\d*\.?\d*?\.?\d*?)_(?<tree>\d*\.?\d*?\.?\d*?)_(?<type>(Text|Image|QuickReply))", RegexOptions.IgnoreCase);

        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader("C:\\svg\\Frame.svg", Encoding.UTF8);

            var svgFile = SvgDocument.Open<SvgDocument>(reader.BaseStream);

            var elements = svgFile.Descendants().Where(e => e.ID != null && regex.IsMatch(e.ID)).ToList();

            var flows = GetFlows(elements);

            using (var writer = new StreamWriter("C:\\svg\\Frame.json"))
            {
                var flowsSerial = JsonConvert.SerializeObject(flows);

                writer.Write(flowsSerial);

            }

            
        }

        private static IEnumerable<Flow> GetFlows(IEnumerable<SvgElement> elements)
        {
            var elementsGroupeByFlow = elements.GroupBy(e => new { Flow = regex.Match(e.ID).Groups["flow"].Value });

            var flowList = new List<Flow>();

            foreach(var flowGroup in elementsGroupeByFlow)
            {
                var flow = new Flow();
                flow.Name = flowGroup.Key.Flow;
                flow.BlockList = new List<Block>();

                foreach(var blockElement in flowGroup)
                {
                    var position = regex.Match(blockElement.ID).Groups["tree"].Value;
                    var blocks = GetBlocksWithType(blockElement);

                    foreach(var block in blocks)
                    {
                        block.Order = position;
                        flow.BlockList.Add(block);
                    }
                }

                flow.BlockList = flow.BlockList.OrderBy(b => b.Order).ToList();
                flowList.Add(flow);
            }

            flowList = flowList.OrderBy(f => f.Name).ToList();

            return flowList;
        }

        private static IEnumerable<Block> GetBlocksWithType(SvgElement element)
        {
            switch(regex.Match(element.ID).Groups["type"].Value.ToLower())
            {
                case "quickreply": return GetBlocks<BlockMenu>(element);
                case "image": return GetBlocks<BlockImage>(element);
                default: return GetBlocks<BlockText>(element);
            }
        }


        private static IEnumerable<T> GetBlocks<T>(SvgElement elements) where T : Block
        {
            if (typeof(T) == typeof(BlockText))
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
            if (typeof(T) == typeof(BlockMenu))
            {
                var dictionaryMenu = new Dictionary<string, BlockMenu>();

                var menus = elements.Descendants().Where(e => e.ID != null && (e.ID.Contains("#t=") || e.ID.Contains("default")))
                    .ToList();

                foreach (var e in menus)
                {
                    var nameMenu = e.Parents.Where(p => p.ID != null && regex.IsMatch(p.ID)).First();
                    BlockMenu blockMenu = null;

                    if (!dictionaryMenu.ContainsKey(nameMenu.ID))
                    {
                        blockMenu = new BlockMenu();
                        blockMenu.OptionList = new List<BlockMenuOption>();
                        blockMenu.MenuType = MenuType.Default;
                    }
                    else
                    {
                        blockMenu = dictionaryMenu[nameMenu.ID];
                    }

                    if (e.Parent.ID.Contains("QuickReply"))
                    {
                        blockMenu.MenuType = MenuType.QuickReply;
                    }


                    if (e.Parent.ID.Contains("Title"))
                    {
                        blockMenu.Text = GetEncodedText(e.ID.Substring(3));
                    }
                    else
                    if (e.Parent.ID.Contains("option"))
                    {
                        var option = new BlockMenuOption();
                        option.Text = GetEncodedText(e.ID.Substring(3));

                        blockMenu.OptionList.Add(option);
                    }
                    else
                    if (e.ID.Contains("default"))
                    {
                        var option = new BlockMenuOption();
                        option.Text = "efault";

                        blockMenu.OptionList.Add(option);
                    }

                    if (dictionaryMenu.ContainsKey(nameMenu.ID))
                    {
                        dictionaryMenu.Remove(nameMenu.ID);
                    }

                    dictionaryMenu.Add(nameMenu.ID, blockMenu);
                }

                return dictionaryMenu.Values.AsEnumerable() as IEnumerable<T>;
            }
            else
            if (typeof(T) == typeof(BlockImage))
            {
                var imageList = new List<BlockImage>();
                imageList.Add(new BlockImage());
                return imageList.AsEnumerable() as IEnumerable<T>;
            }

            return null;
        }

        public static string GetEncodedText(string text)
        {
            return text;
            //return Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));
        }


    }
}
