using System;
using Gum.Wireframe;
using Gum.Forms.Controls;
using MonoGameGum;
using Microsoft.Xna.Framework;

namespace Boids.ui
{
    public class StartupUI 
    {
        GumService Gum => GumService.Default;
        public Action OnSimulationModeClicked;
        public Action OnPlayerModeClicked;
        public Action OnOptionClicked;
        public Action OnExitClicked;
        private Button simButton;
        private Button optionButton;
        private Button playerButton;
        private Button exitButton;
        private StackPanel _mainPanel;
        
        internal void HookEvents()
        {
            simButton.Click += (_, _) => OnSimulationModeClicked?.Invoke();
            playerButton.Click += (_, _) => OnPlayerModeClicked?.Invoke();
            optionButton.Click += (_, _) => OnOptionClicked?.Invoke();
            exitButton.Click += (_, _) => OnExitClicked?.Invoke(); 
        }

        public void BuildUI(Game game)
        {
            _mainPanel = new() { Spacing = 3 };
            _mainPanel.Anchor(Anchor.Center);

            simButton = new() { Text = "Simulation Mode" };
            optionButton = new() { Text = "Options" };
            playerButton = new() { Text = "Player Mode" };
            exitButton = new() { Text = "Exit" };

            _mainPanel.AddChild(simButton);
            _mainPanel.AddChild(optionButton);
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
                Console.WriteLine($"Startup panel has not been initialized: {ex}");
            }
        }

        public void ReSizeUI(int newWidth, int newHeight)
        {
            if (_mainPanel!= null)
            {
                UIUtils.UpdateUISize(newWidth,newHeight);
                _mainPanel.UpdateState();
            }
        }
    }
}
