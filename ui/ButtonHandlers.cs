using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MonoGameGum.Forms.Controls;
using RenderingLibrary.Math.Geometry;

namespace Boids
{
    static class ButtonHandlers
    {
        public static void addOrRemButtons(List<Button> buttons, BoidManager manager)
        {
            if (manager == null) return;

            foreach (Button b in buttons)
            {
                int spawns = getCount(b.Name);
                b.Click += (_, _) =>
                {
                    for (int i = 0; i < Math.Abs(spawns); i++)
                    {
                        if (spawns > 0) manager.SpawnBoid();
                        else manager.RemoveBoid();
                    }
                };
            }
        }
        private static int getCount(string name)
        {
            int countOut;
            if (!int.TryParse(name, out countOut)) {
                countOut = 1;
                Console.WriteLine("Unable to parse the name:" + name + ", set to " + countOut + ". ");
            }
            return countOut;
        }
        public static void sliderHandling(List<ControlPair<Slider, Label>> sliders)
        {
            foreach (ControlPair<Slider, Label> pair in sliders)
            {
                Slider slider = pair.First;
                Label label = pair.Second;

                slider.ValueChanged += (_, _) =>
                {
                    label.Text = Math.Round(slider.Value, 2).ToString();
                    float newValue = float.Parse(label.Text);
                    if (slider.Name == "Cohesion")
                    {
                        Constants.coheFactor = newValue;
                    }
                    else if (slider.Name == "Seperation")
                    {
                        Constants.sepFactor = newValue;
                    }
                    else if (slider.Name == "Alignment")
                    {
                        Constants.alignFactor = newValue;
                    }
                };
            }
        }
        public static void bcHandling(List<ComboBox> comboBoxes)
        {
            foreach (ComboBox comboBox in comboBoxes)
            {
                comboBox.SelectionChanged += (_, _) =>
                {
                    if (comboBox.SelectedObject.ToString() == "Steer")
                    {
                        Constants.bcCondition = Constants.BoundaryType.Steer;
                    }
                    else if (comboBox.SelectedObject.ToString() == "Wrap")
                    {
                        Constants.bcCondition = Constants.BoundaryType.Wrap;
                    }
                    else if (comboBox.SelectedObject.ToString() == "Bounce")
                    {
                        Constants.bcCondition = Constants.BoundaryType.Bounce;
                    }
                    else Constants.bcCondition = Constants.BoundaryType.Steer;
                };
            }
        }
    }
}