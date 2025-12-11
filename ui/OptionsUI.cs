using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Gum.Wireframe;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameGum;
using Microsoft.Xna.Framework;
using Gum.DataTypes;
using Gum.Managers;

namespace Boids.ui
{
    public class OptionsUI
    {
        // Layout constants
        private const int PanelWidth = 400;
        private const int SliderWidth = 200;
        private const int DefaultSpacing = 8;
        private const int LineHeight = 2;
        private const int PanelPadding = 16;

        // JSON file paths
        private const string ResolutionsPath = "Options/Resolutions.json";
        private const string ResponsesPath = "Options/OptionsResponses.json";

        // Loaded data
        private List<string> _resolutions;
        private Dictionary<string, List<string>> _humorousResponses;

        // Random for selecting messages
        private readonly Random _random = new();

        // Default slider value
        private const double DefaultSliderValue = 50;

        // Popup UI elements
        private ContainerRuntime _popupOverlay;
        private Label _popupMessageLabel;
        private Button _popupOkButton;
        private bool _isPopupVisible;

        // Flag to prevent re-entry during event handlers
        private bool _isResettingControl;

        // Containers
        private ContainerRuntime _mainContainer;

        // Audio controls
        private Slider _musicSlider;
        private Slider _sfxSlider;
        private Label _musicValueLabel;
        private Label _sfxValueLabel;

        // Graphics controls
        private CheckBox _vsyncCheckBox;
        private CheckBox _hdrCheckBox;
        private CheckBox _antiAliasingCheckBox;
        private CheckBox _textureQualityCheckBox;

        // Resolution control
        private ComboBox _resolutionComboBox;

        // Navigation
        private Button _backButton;

        // Callbacks
        public Action OnBackClicked;

        internal void BuildUI()
        {
            // Load JSON data
            LoadResolutions();
            LoadHumorousResponses();

            // ==========================================
            // MAIN CONTAINER (centered, fixed width)
            // ==========================================
            _mainContainer = new ContainerRuntime()
            {
                WidthUnits = DimensionUnitType.Absolute,
                Width = PanelWidth,
                HeightUnits = DimensionUnitType.RelativeToChildren
            };
            _mainContainer.Anchor(Anchor.Center);

            // ==========================================
            // BACKGROUND PANEL
            // ==========================================
            ColoredRectangleRuntime backgroundPanel = new()
            {
                Color = Color.SlateGray,
                WidthUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                ChildrenLayout = ChildrenLayout.TopToBottomStack,
                StackSpacing = DefaultSpacing
            };
            _mainContainer.AddChild(backgroundPanel);

            // ==========================================
            // CONTENT CONTAINER (for padding)
            // ==========================================
            ContainerRuntime contentContainer = new()
            {
                WidthUnits = DimensionUnitType.Absolute,
                Width = PanelWidth - (PanelPadding * 2),
                HeightUnits = DimensionUnitType.RelativeToChildren,
                ChildrenLayout = ChildrenLayout.TopToBottomStack,
                StackSpacing = DefaultSpacing,
                X = PanelPadding,
                Y = PanelPadding
            };
            backgroundPanel.AddChild(contentContainer);

            // ==========================================
            // TITLE
            // ==========================================
            Label titleLabel = new() { Text = "OPTIONS" };
            contentContainer.AddChild(titleLabel.Visual);

            // ==========================================
            // HORIZONTAL LINE 1
            // ==========================================
            ColoredRectangleRuntime line1 = new()
            {
                Color = Color.DarkSlateGray,
                WidthUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                HeightUnits = DimensionUnitType.Absolute,
                Height = LineHeight
            };
            contentContainer.AddChild(line1);

            // ==========================================
            // AUDIO SECTION
            // ==========================================
            Label audioHeader = new() { Text = "Audio" };
            contentContainer.AddChild(audioHeader.Visual);

            // Music row
            ContainerRuntime musicRow = new()
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                ChildrenLayout = ChildrenLayout.LeftToRightStack,
                StackSpacing = DefaultSpacing
            };

            Label musicLabel = new() { Text = "Music" };
            musicRow.AddChild(musicLabel.Visual);

            _musicSlider = new Slider()
            {
                Width = SliderWidth,
                Minimum = 0,
                Maximum = 100,
                Value = 50
            };
            musicRow.AddChild(_musicSlider);

            _musicValueLabel = new() { Text = "50" };
            musicRow.AddChild(_musicValueLabel.Visual);

            contentContainer.AddChild(musicRow);

            // SFX row
            ContainerRuntime sfxRow = new()
            {
                WidthUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                ChildrenLayout = ChildrenLayout.LeftToRightStack,
                StackSpacing = DefaultSpacing
            };

            Label sfxLabel = new() { Text = "Sound Effects" };
            sfxRow.AddChild(sfxLabel.Visual);

            _sfxSlider = new Slider()
            {
                Width = SliderWidth,
                Minimum = 0,
                Maximum = 100,
                Value = 50
            };
            sfxRow.AddChild(_sfxSlider);

            _sfxValueLabel = new() { Text = "50" };
            sfxRow.AddChild(_sfxValueLabel.Visual);

            contentContainer.AddChild(sfxRow);

            // ==========================================
            // HORIZONTAL LINE 2
            // ==========================================
            ColoredRectangleRuntime line2 = new()
            {
                Color = Color.DarkSlateGray,
                WidthUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                HeightUnits = DimensionUnitType.Absolute,
                Height = LineHeight
            };
            contentContainer.AddChild(line2);

            // ==========================================
            // GRAPHICS SECTION
            // ==========================================
            Label graphicsHeader = new() { Text = "Graphics" };
            contentContainer.AddChild(graphicsHeader.Visual);

            _vsyncCheckBox = new CheckBox() { Text = "VSync" };
            contentContainer.AddChild(_vsyncCheckBox.Visual);

            _hdrCheckBox = new CheckBox() { Text = "HDR" };
            contentContainer.AddChild(_hdrCheckBox.Visual);

            _antiAliasingCheckBox = new CheckBox() { Text = "Anti-aliasing" };
            contentContainer.AddChild(_antiAliasingCheckBox.Visual);

            _textureQualityCheckBox = new CheckBox() { Text = "Texture Quality" };
            contentContainer.AddChild(_textureQualityCheckBox.Visual);

            // ==========================================
            // HORIZONTAL LINE 3
            // ==========================================
            ColoredRectangleRuntime line3 = new()
            {
                Color = Color.DarkSlateGray,
                WidthUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                HeightUnits = DimensionUnitType.Absolute,
                Height = LineHeight
            };
            contentContainer.AddChild(line3);

            // ==========================================
            // RESOLUTION SECTION
            // ==========================================
            Label resolutionHeader = new() { Text = "Resolution" };
            contentContainer.AddChild(resolutionHeader.Visual);

            _resolutionComboBox = new ComboBox();
            foreach (string resolution in _resolutions)
            {
                _resolutionComboBox.Items.Add(resolution);
            }
            _resolutionComboBox.SelectedIndex = 0;  // Set default selection
            contentContainer.AddChild(_resolutionComboBox.Visual);

            // ==========================================
            // HORIZONTAL LINE 4
            // ==========================================
            ColoredRectangleRuntime line4 = new()
            {
                Color = Color.DarkSlateGray,
                WidthUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                HeightUnits = DimensionUnitType.Absolute,
                Height = LineHeight
            };
            contentContainer.AddChild(line4);

            // ==========================================
            // BACK BUTTON
            // ==========================================
            _backButton = new Button() { Text = "Back" };
            contentContainer.AddChild(_backButton.Visual);

            // Build popup (initially hidden)
            BuildPopupUI();
        }

        internal void HookEvents()
        {
            // Back button
           _backButton.Click += (_, _) => OnBackClicked?.Invoke();
            
            // Popup OK button
            _popupOkButton.Click += (_, _) => HidePopup();
            
            // Music slider
            _musicSlider.ValueChanged += (_, _) =>
            {
                if (_isResettingControl) return;
                
                _musicValueLabel.Text = ((int)_musicSlider.Value).ToString();
                ShowPopup("volume");
                
                _isResettingControl = true;
                _musicSlider.Value = DefaultSliderValue;
                _musicValueLabel.Text = ((int)DefaultSliderValue).ToString();
                _isResettingControl = false;
            };
            
            // SFX slider
            _sfxSlider.ValueChanged += (_, _) =>
            {
                if (_isResettingControl) return;
                
                _sfxValueLabel.Text = ((int)_sfxSlider.Value).ToString();
                ShowPopup("sfx");
                
                _isResettingControl = true;
                _sfxSlider.Value = DefaultSliderValue;
                _sfxValueLabel.Text = ((int)DefaultSliderValue).ToString();
                _isResettingControl = false;
            };
            
            // Checkboxes
            _vsyncCheckBox.Click += (_, _) =>
            {
                ShowPopup("vsync");
                _vsyncCheckBox.IsChecked = false;
            };
            
            _hdrCheckBox.Click += (_, _) =>
            {
                ShowPopup("hdr");
                _hdrCheckBox.IsChecked = false;
            };
            
            _antiAliasingCheckBox.Click += (_, _) =>
            {
                ShowPopup("antialiasing");
                _antiAliasingCheckBox.IsChecked = false;
            };
            
            _textureQualityCheckBox.Click += (_, _) =>
            {
                ShowPopup("texturequality");
                _textureQualityCheckBox.IsChecked = false;
            };
            
            // Resolution dropdown
            _resolutionComboBox.SelectionChanged += (_, _) =>
            {
                if (_isResettingControl) return;
                
                ShowPopup("resolution");
                
                _isResettingControl = true;
                _resolutionComboBox.SelectedIndex = 0;
                _isResettingControl = false;
            };
        }

        public void ShowUI()
        {
            try
            {
                _mainContainer.AddToRoot();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Options container has not been initialized: {ex}");
            }
        }

        public void ReSizeUI(int newWidth, int newHeight)
        {
            if (_mainContainer != null)
            {
                UIUtils.UpdateUISize(newWidth, newHeight);
                _mainContainer.Anchor(Anchor.Center);
                _mainContainer.UpdateLayout();
            }
        }

        internal void RebuildAndShowUI()
        {
            if (_mainContainer != null && _mainContainer.Parent != null)
            {
                _mainContainer.RemoveFromRoot();
            }
            BuildUI();
            HookEvents();
            ShowUI();
            _popupOverlay.AddToRoot();  // Add popup AFTER main container, so it's on top
        }

        // JSON deserialization class
        private class ResolutionsData
        {
            public List<string> resolutions { get; set; }
        }

        private void LoadResolutions()
        {
            try
            {
                string json = File.ReadAllText(ResolutionsPath);
                var data = JsonSerializer.Deserialize<ResolutionsData>(json);
                _resolutions = data?.resolutions ?? new List<string> { "1280x720" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load resolutions: {ex.Message}");
                _resolutions = new List<string> { "1280x720" };
            }
        }

        private void LoadHumorousResponses()
        {
            try
            {
                string json = File.ReadAllText(ResponsesPath);
                _humorousResponses = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load responses: {ex.Message}");
                _humorousResponses = new Dictionary<string, List<string>>();
            }
        }

        private string GetRandomResponse(string category)
        {
            if (_humorousResponses == null || !_humorousResponses.ContainsKey(category))
            {
                return "This setting is currently unavailable.";
            }
            
            var responses = _humorousResponses[category];
            if (responses.Count == 0)
            {
                return "This setting is currently unavailable.";
            }
            
            int index = _random.Next(responses.Count);
            return responses[index];
        }

        private void BuildPopupUI()
        {
            // Small floating popup panel - like a Windows error message
            _popupOverlay = new ContainerRuntime()
            {
                WidthUnits = DimensionUnitType.Absolute,
                Width = 450,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                Visible = false
            };
            _popupOverlay.Anchor(Anchor.Center);
            
            // Popup background
            ColoredRectangleRuntime popupBg = new()
            {
                Color = Color.DarkSlateGray,
                WidthUnits = DimensionUnitType.PercentageOfParent,
                Width = 100,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                ChildrenLayout = ChildrenLayout.TopToBottomStack,
                StackSpacing = 12
            };
            _popupOverlay.AddChild(popupBg);
            
            // Content with padding
            ContainerRuntime popupContent = new()
            {
                WidthUnits = DimensionUnitType.Absolute,
                Width = 450 - 24,
                HeightUnits = DimensionUnitType.RelativeToChildren,
                ChildrenLayout = ChildrenLayout.TopToBottomStack,
                StackSpacing = 12,
                X = 12,
                Y = 12
            };
            popupBg.AddChild(popupContent);
            
            // Message label - set width so text wraps
            _popupMessageLabel = new Label() { Text = "" };
            _popupMessageLabel.Visual.WidthUnits = DimensionUnitType.Absolute;
            _popupMessageLabel.Visual.Width = 450 - 24;  // Match content width
            popupContent.AddChild(_popupMessageLabel.Visual);
            
            // OK button
            _popupOkButton = new Button() { Text = "OK" };
            popupContent.AddChild(_popupOkButton.Visual);
            
            }

        private void ShowPopup(string category)
        {
            if (_isPopupVisible) return;  // Don't show another popup if one is already visible
            
            string message = GetRandomResponse(category);
            _popupMessageLabel.Text = message;
            _popupOverlay.Visible = true;
            _isPopupVisible = true;
        }

        private void HidePopup()
        {
            _popupOverlay.Visible = false;
            _isPopupVisible = false;
        }
    }
}
