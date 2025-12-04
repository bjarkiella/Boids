using System;
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
        private TextRuntime _timerText;
        private float _elapsedSeconds = 0f;

        internal void BuildTimerUI()
        {
            // Container to hold the timer display
            _mainContainer = new() 
            {
                WidthUnits = DimensionUnitType.RelativeToChildren,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                X = 20,  // Padding from right edge
                Y = 20   // Padding from top
            };
            _mainContainer.Anchor(Anchor.TopRight);

            // Text element that displays the timer
            _timerText = new()
            {
                Text = "00:00",
                FontSize = 32,
                Color = Color.Black
            };
            _timerText.Parent = _mainContainer;
        }

        internal void UpdateTimer()
        {
            _elapsedSeconds += Time.Delta;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            int minutes = (int)(_elapsedSeconds / 60);
            int seconds = (int)(_elapsedSeconds % 60);
            _timerText.Text = $"{minutes:D2}:{seconds:D2}";
        }

        internal void ResetTimer()
        {
            _elapsedSeconds = 0f;
            UpdateDisplay();
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
            BuildTimerUI();
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
