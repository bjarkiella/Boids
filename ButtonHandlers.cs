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
                Console.WriteLine("Unable to parse the name:" + name +", set to " + countOut + ". ");
            }
            return countOut;
        }
    }
}