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
        public static List<ControlPair<Slider,Label>> AddSliderRow(int width, List<string> sName, StackPanel sliderPanel,StackPanel textPanel)
        {
            List<ControlPair<Slider,Label>> listOut = new List<ControlPair<Slider, Label>>();
            Label startLabel = new Label();
            startLabel.Text = " ";
            textPanel.AddChild(startLabel);

            foreach (string name in sName)
            {
                Label label = new Label();
                label.Text = name;
                Label emptyLabel = new Label();
                emptyLabel.Text = " ";
                emptyLabel.Anchor(Anchor.Center);
                Slider slider = new Slider()
                {
                    Name = name,    
                    Width = width,
                    Minimum = Constants.boidMinFactor,
                    Maximum = Constants.boidMaxFactor
                };
                if (name == "Cohesion")
                {
                    slider.Value = Math.Round(Constants.coheFactor, Constants.roundNumber);
                }
                else if (name == "Seperation")
                {
                    slider.Value = Math.Round(Constants.sepFactor, Constants.roundNumber);
                }
                else if (name == "Alignment")
                {
                    slider.Value = Math.Round(Constants.alignFactor, Constants.roundNumber);
                }
                else slider.Value = 1;

                Label outText = new Label()
                {
                    Text = slider.Value.ToString()
                };
                textPanel.AddChild(emptyLabel);
                sliderPanel.AddChild(label);
                sliderPanel.AddChild(slider);
                textPanel.AddChild(outText);
                listOut.Add(new ControlPair<Slider, Label>(slider, outText));
            }
            return listOut;
        }
    
    }
}