using System;
using Gum.Wireframe;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameGum;
using Microsoft.Xna.Framework;

using Boids.Shared;
using Boids.Boids;

namespace Boids.ui
{
    public class SimUI 
    {
        private ContainerRuntime _mainContainer;
        private int _sliderWidth = 250; 
        private int _buttonWidth = 125;
        private int _defaultSpacing = 3;
        
        // Labels for dynamic value updates
        private Label _cohesionLabel;
        private Label _separationLabel;
        private Label _alignmentLabel;
        private Label _minSpeedLabel;
        private Label _maxSpeedLabel;

        internal void BuildUI(BoidManager boidManager)
        {
            // Main bottom panel container
            _mainContainer = new()
            {
                Name = "bottomPanel",
                WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute,
                Width = Constants.SWidth,
                HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute,
                Height = Constants.PHeight
            };
            _mainContainer.Dock(Dock.Bottom);

            // Colored background rectangle
            ColoredRectangleRuntime bottomBack = new()
            {
                Width = 100,
                Color = Color.SlateGray,
                WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent,
                HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                ChildrenLayout = Gum.Managers.ChildrenLayout.LeftToRightStack,
                StackSpacing = 20
            };
            _mainContainer.AddChild(bottomBack);

            // ==========================================
            // SECTION 1: Add Buttons
            // ==========================================
            StackPanel addButtonsPanel = new() { Spacing = _defaultSpacing };
            
            Label addLabel = new() { Text = "Add Boids" };
            addButtonsPanel.AddChild(addLabel);

            Button add1 = new() { Text = "+1" };
            add1.Visual.Width = _buttonWidth;
            add1.Click += (_, _) => boidManager.SpawnBoid();
            addButtonsPanel.AddChild(add1);

            Button add10 = new() { Text = "+10" };
            add10.Visual.Width = _buttonWidth;
            add10.Click += (_, _) => 
            {
                for (int i = 0; i < 10; i++)
                    boidManager.SpawnBoid();
            };
            addButtonsPanel.AddChild(add10);

            Button add100 = new() { Text = "+100" };
            add100.Visual.Width = _buttonWidth;
            add100.Click += (_, _) => 
            {
                for (int i = 0; i < 100; i++)
                    boidManager.SpawnBoid();
            };
            addButtonsPanel.AddChild(add100);

            bottomBack.AddChild(addButtonsPanel.Visual);

            // ==========================================
            // SECTION 2: Remove Buttons
            // ==========================================
            StackPanel removeButtonsPanel = new() { Spacing = _defaultSpacing };
            
            Label removeLabel = new() { Text = "Remove Boids" };
            removeButtonsPanel.AddChild(removeLabel);

            Button remove1 = new() { Text = "-1" };
            remove1.Visual.Width = _buttonWidth;
            remove1.Click += (_, _) => boidManager.RemoveBoid();
            removeButtonsPanel.AddChild(remove1);

            Button remove10 = new() { Text = "-10" };
            remove10.Visual.Width = _buttonWidth;
            remove10.Click += (_, _) => 
            {
                for (int i = 0; i < 10; i++)
                    boidManager.RemoveBoid();
            };
            removeButtonsPanel.AddChild(remove10);

            Button remove100 = new() { Text = "-100" };
            remove100.Visual.Width = _buttonWidth;
            remove100.Click += (_, _) => 
            {
                for (int i = 0; i < 100; i++)
                    boidManager.RemoveBoid();
            };
            removeButtonsPanel.AddChild(remove100);

            bottomBack.AddChild(removeButtonsPanel.Visual);

            // ==========================================
            // SECTION 3: Behavior Sliders
            // ==========================================
            ContainerRuntime behaviorContainer = new()
            {
                HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                StackSpacing = _defaultSpacing,
                ChildrenLayout = Gum.Managers.ChildrenLayout.LeftToRightStack
            };

            StackPanel behaviorSliders = new() { Spacing = _defaultSpacing };
            StackPanel behaviorLabels = new() { Spacing = _defaultSpacing  };

            // Section Header
            Label behaviorHeader = new() { Text = "       Behavior" };
            behaviorSliders.AddChild(behaviorHeader);
            Label behaviorSpacer = new() { Text = " " };
            behaviorLabels.AddChild(behaviorSpacer);

            // Cohesion Slider
            Label cohesionNameLabel = new() { Text = "Cohesion" };
            behaviorSliders.AddChild(cohesionNameLabel);

            Slider cohesionSlider = new()
            {
                Width = _sliderWidth,
                Minimum = BoidConstants.boidMinFactor,
                Maximum = BoidConstants.boidMaxFactor,
                Value = BoidConstants.CoheFactor
            };
            behaviorSliders.AddChild(cohesionSlider);

            _cohesionLabel = new() { Text = cohesionSlider.Value.ToString("F1") };
            behaviorLabels.AddChild(_cohesionLabel);

            cohesionSlider.ValueChanged += (_, _) =>
            {
                float newValue = (float)Math.Round(cohesionSlider.Value, 1);
                _cohesionLabel.Text = newValue.ToString("F1");
                BoidConstants.CoheFactor = newValue;
            };

            // Separation Slider
            Label separationNameLabel = new() { Text = "Separation" };
            behaviorSliders.AddChild(separationNameLabel);

            Slider separationSlider = new()
            {
                Width = _sliderWidth,
                Minimum = BoidConstants.boidMinFactor,
                Maximum = BoidConstants.boidMaxFactor,
                Value = BoidConstants.SepFactor
            };
            behaviorSliders.AddChild(separationSlider);

            _separationLabel = new() { Text = separationSlider.Value.ToString("F1") };
            behaviorLabels.AddChild(_separationLabel);

            separationSlider.ValueChanged += (_, _) =>
            {
                float newValue = (float)Math.Round(separationSlider.Value, 1);
                _separationLabel.Text = newValue.ToString("F1");
                BoidConstants.SepFactor = newValue;
            };

            // Alignment Slider
            Label alignmentNameLabel = new() { Text = "Alignment" };
            behaviorSliders.AddChild(alignmentNameLabel);

            Slider alignmentSlider = new()
            {
                Width = _sliderWidth,
                Minimum = BoidConstants.boidMinFactor,
                Maximum = BoidConstants.boidMaxFactor,
                Value = BoidConstants.AlignFactor
            };
            behaviorSliders.AddChild(alignmentSlider);

            _alignmentLabel = new() { Text = alignmentSlider.Value.ToString("F1") };
            behaviorLabels.AddChild(_alignmentLabel);

            alignmentSlider.ValueChanged += (_, _) =>
            {
                float newValue = (float)Math.Round(alignmentSlider.Value, 1);
                _alignmentLabel.Text = newValue.ToString("F1");
                BoidConstants.AlignFactor = newValue;
            };

            behaviorContainer.AddChild(behaviorSliders.Visual);
            behaviorContainer.AddChild(behaviorLabels.Visual);
            bottomBack.AddChild(behaviorContainer);

            // ==========================================
            // SECTION 4: Speed Sliders
            // ==========================================
            ContainerRuntime speedContainer = new()
            {
                HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren,
                StackSpacing = _defaultSpacing ,
                ChildrenLayout = Gum.Managers.ChildrenLayout.LeftToRightStack
            };

            StackPanel speedSliders = new() { Spacing = _defaultSpacing };
            StackPanel speedLabels = new() { Spacing = _defaultSpacing };

            // Section Header
            Label speedHeader = new() { Text = "Speed" };
            speedSliders.AddChild(speedHeader);
            Label speedSpacer = new() { Text = " " };
            speedLabels.AddChild(speedSpacer);

            // Min Speed Slider
            Label minSpeedNameLabel = new() { Text = "Min" };
            speedSliders.AddChild(minSpeedNameLabel);

            Slider minSpeedSlider = new()
            {
                Width = _sliderWidth,
                Minimum = 50,
                Maximum = 400,
                Value = BoidConstants.MinSpeed
            };
            speedSliders.AddChild(minSpeedSlider);

            _minSpeedLabel = new() { Text = minSpeedSlider.Value.ToString("F1") };
            speedLabels.AddChild(_minSpeedLabel);

            minSpeedSlider.ValueChanged += (_, _) =>
            {
                float newValue = (float)Math.Round(minSpeedSlider.Value, 1);
                _minSpeedLabel.Text = newValue.ToString("F1");
                BoidConstants.MinSpeed = newValue;
            };

            // Max Speed Slider
            Label maxSpeedNameLabel = new() { Text = "Max" };
            speedSliders.AddChild(maxSpeedNameLabel);

            Slider maxSpeedSlider = new()
            {
                Width = _sliderWidth,
                Minimum = 50,
                Maximum = 400,
                Value = BoidConstants.MaxSpeed
            };
            speedSliders.AddChild(maxSpeedSlider);

            _maxSpeedLabel = new() { Text = maxSpeedSlider.Value.ToString("F1") };
            speedLabels.AddChild(_maxSpeedLabel);

            maxSpeedSlider.ValueChanged += (_, _) =>
            {
                float newValue = (float)Math.Round(maxSpeedSlider.Value, 1);
                _maxSpeedLabel.Text = newValue.ToString("F1");
                BoidConstants.MaxSpeed = newValue;
            };

            speedContainer.AddChild(speedSliders.Visual);
            speedContainer.AddChild(speedLabels.Visual);
            bottomBack.AddChild(speedContainer);

            // TODO: Future texture support
            // button.Visual.Texture = Content.Load<Texture2D>("ui/button_bg");
            // bottomBack can be replaced with sprite background
        }
        
        public void ReSizeUI(int newWidth, int newHeight)
        {
            if (_mainContainer != null)
            {
                UIUtils.UpdateUISize(newWidth,newHeight);

                // Update the container width to match new screen width
                _mainContainer.Width = newWidth;
                _mainContainer.UpdateLayout();
            }
        }

        internal void RebuildAndShowUI(BoidManager boidManager)
        {
            if(_mainContainer != null && _mainContainer.Parent != null)
            {
                _mainContainer.RemoveFromRoot();
            }
            BuildUI(boidManager);
            ShowUI();
        }
        public void ShowUI()
        {
            try
            {
                _mainContainer.AddToRoot();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Simulation container has not been initialized: {ex}");
            }
        }
    }
}
