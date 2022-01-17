using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemCompLogger.model
{
    /* These classes define the hierarchy built by the StoryLoader which
     * represents the story structure. */
    public class Menu
    {
        public List<MenuPage> pages { get; set; }
        public List<MenuArt> art { get; set; }
    }

    public class MenuPage
    {
        public string pageName { get; set; }
        public string textOpen { get; set; }
        public string textClose { get; set; }
        public string textReturn { get; set; }
        public bool visited { get; set; }
        public List<MenuButton> buttons { get; set; }
    }

    public class MenuButton
    {
        public int num { get; set; }
        public string action { get; set; }
        public string go { get; set; }
        public string text { get; set; }
        public string textClick { get; set; }
    }

    public class MenuArt
    {
        public int artNo { get; set; }
        public string text { get; set; }
    }
}
