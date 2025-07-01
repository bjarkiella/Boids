using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gum.Wireframe;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;

namespace Boids
{
    public static class UI
    {
        public static List<Button> AddButtonRow(string labelName, int bWidth, List<int> bName, string preFix, StackPanel stackPanel)
        {
            List<Button> listOut = new List<Button>();
            Button button;
            Label label = new Label();
            label.Text = labelName;
            label.Anchor(Anchor.Top);
            stackPanel.AddChild(label);
            foreach (int num in bName) {
                button = new Button();
                button.Text = preFix + num;
                button.Name = preFix + num;
                button.Visual.Width = bWidth;
                listOut.Add(button);
                stackPanel.AddChild(button);
            }
            return listOut;
        }
    
    }
}