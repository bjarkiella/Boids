using System;
using Gum.Wireframe;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Boids.Shared;
using MonoGameGum.GueDeriving;
using Gum.DataTypes;

namespace Boids.ui
{
    internal static class UIUtils 
    {
        public static void UpdateUISize(int newWidth, int newHeight)
        {
            try
            {
                GraphicalUiElement.CanvasWidth = newWidth;
                GraphicalUiElement.CanvasHeight = newHeight;
                GumService.Default.Root.Width = newWidth;
                GumService.Default.Root.Height = newHeight;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unable to set the UI size, {ex}");
            }
        }
    }
}
