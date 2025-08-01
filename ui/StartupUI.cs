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
        public Action OnExitClicked;
        private Button simButton;
        private Button playerButton;
        private Button exitButton;
        private StackPanel _mainPanel;
        
        internal void HookEvents()
        {
            simButton.Click += (_, _) => OnSimulationModeClicked?.Invoke();
            playerButton.Click += (_, _) => OnPlayerModeClicked?.Invoke();
            exitButton.Click += (_,_) => OnExitClicked?.Invoke(); 
        }
        public void BuildUI(Game game)
        {

            _mainPanel = new StackPanel();
            _mainPanel.Spacing = 3;
            _mainPanel.Anchor(Anchor.Center);

            simButton = new Button();
            simButton.Text = "Simulation Mode";

            playerButton = new Button();
            playerButton.Text = "Player Mode";
            
            exitButton = new Button();
            exitButton.Text = "Exit";

            _mainPanel.AddChild(simButton);
            _mainPanel.AddChild(playerButton);
            _mainPanel.AddChild(exitButton);
        }
        public void ShowUI()
        {
            try
            {
                _mainPanel.AddToRoot();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Startup panel has not been initialized", ex);
            }
        }
    
    }
}