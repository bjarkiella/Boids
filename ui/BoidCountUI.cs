using Gum.Wireframe;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Boids.Shared;
using MonoGameGum.GueDeriving;
using Gum.DataTypes;

namespace Boids.ui
{
    internal class BoidCountUI 
    {
        private ContainerRuntime _mainContainer;
        private TextRuntime _boidText;

        internal void BuildBoidCountUI()
        {
            // Container to hold the timer display
            _mainContainer = new() 
            {
                WidthUnits = DimensionUnitType.RelativeToChildren,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                X = 20,  // Padding from left edge
                Y = 20   // Padding from top
            };
            _mainContainer.Anchor(Anchor.TopLeft);

            // Text element that displays the timer
            _boidText = new()
            {
                Text = "",
                FontSize = 32,
                Color = Color.Black
            };
            _boidText.Parent = _mainContainer;
        }

        internal void UpdateBoidCountDisplay(int boidNumber)
        {
            // if (boidNumber == null) return;

            _boidText.Text = boidNumber.ToString(); 
        }

        internal void HideUI()
        {
            if (_mainContainer != null && _mainContainer.Parent != null)
            {
                _mainContainer.RemoveFromRoot();
            }
        }

        internal void ShowUI()
        {
            if (_mainContainer != null && _mainContainer.Parent == null)
            {
                _mainContainer.AddToRoot();
            }
        }

        internal void RebuildAndShowUI()
        {
            if (_mainContainer != null && _mainContainer.Parent != null)
            {
                _mainContainer.RemoveFromRoot();
            }
            BuildBoidCountUI();
            ShowUI();
        }

        public void ReSizeUI(int newWidth, int newHeight)
        {
            if (_mainContainer != null)
            {
                UIUtils.UpdateUISize(newWidth,newHeight);
                _mainContainer.UpdateLayout();
            }
        }
    }
}
