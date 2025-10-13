using System;
using System.Collections.Generic;
using MonoGameGum.Forms.Controls;

using Boids.Boids;
using Boids.Shared;

namespace Boids.ui
{
    static class SimHandling 
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
                        BoidConstants.CoheFactor = newValue;
                    }
                    else if (slider.Name == "Seperation")
                    {
                        BoidConstants.SepFactor = newValue;
                    }
                    else if (slider.Name == "Alignment")
                    {
                        BoidConstants.AlignFactor = newValue;
                    }
                };
            }
        }
    }
}
