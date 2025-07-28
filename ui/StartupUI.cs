using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gum.Wireframe;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using MonoGameGum;
using Microsoft.Xna.Framework;
namespace Boids
{
    public class StartupUI 
    {
        GumService Gum => GumService.Default;
        public Action OnSimulationModeClicked;
        public Action OnPlayerModeClicked;
        private Button simButton;
        private Button playerButton;
        internal void HookEvents()
        {
            simButton.Click += (_, _) => OnSimulationModeClicked?.Invoke();
            playerButton.Click += (_, _) => OnPlayerModeClicked?.Invoke();
        }
        public void drawUI(Game game)
        {
            Gum.Initialize(game);

            StackPanel mainPanel = new StackPanel();
            mainPanel.Spacing = 3;
            mainPanel.Anchor(Anchor.Center);
            mainPanel.AddToRoot();

            simButton = new Button();
            simButton.Text = "Simulation Mode";

            playerButton = new Button();
            playerButton.Text = "Player Mode";

            mainPanel.AddChild(simButton);
            mainPanel.AddChild(playerButton);
        }
    
    }
}