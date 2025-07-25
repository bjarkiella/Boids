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
    public class UI
    {
        GumService Gum => GumService.Default;
        List<Button> _addbuttons,_rembuttons;
        List<ControlPair<Slider, Label>> _boidSlider;
        List<ComboBox> _bcCond;
        public static List<Button> AddButtonRow(string labelName, int bWidth, List<int> bName, string preFix, StackPanel stackPanel,bool visible=true)
        {
            List<Button> listOut = new List<Button>();
            Button button;
            Label label = new Label();
            label.Text = labelName;
            label.Anchor(Anchor.Top);
            stackPanel.AddChild(label);
            foreach (int num in bName) {
                button = new Button();
                button.Text = preFix + num;
                button.Name = preFix + num;
                button.Visual.Width = bWidth;
                button.IsVisible = visible;
                listOut.Add(button);
                stackPanel.AddChild(button);
            }
            return listOut;
        }
        public static List<ControlPair<Slider,Label>> AddSliderRow(int width, List<string> sName, StackPanel sliderPanel,StackPanel textPanel)
        {
            List<ControlPair<Slider,Label>> listOut = new List<ControlPair<Slider, Label>>();
            Label startLabel = new Label();
            startLabel.Text = " ";
            textPanel.AddChild(startLabel);

            foreach (string name in sName)
            {
                Label label = new Label();
                label.Text = name;
                Label emptyLabel = new Label();
                emptyLabel.Text = " ";
                emptyLabel.Anchor(Anchor.Center);
                Slider slider = new Slider()
                {
                    Name = name,    
                    Width = width,
                    Minimum = BoidConstants.boidMinFactor,
                    Maximum = BoidConstants.boidMaxFactor
                };
                if (name == "Cohesion")
                {
                    slider.Value = Math.Round(BoidConstants.coheFactor, Constants.roundNumber);
                }
                else if (name == "Seperation")
                {
                    slider.Value = Math.Round(BoidConstants.sepFactor, Constants.roundNumber);
                }
                else if (name == "Alignment")
                {
                    slider.Value = Math.Round(BoidConstants.alignFactor, Constants.roundNumber);
                }
                else slider.Value = 1;

                Label outText = new Label()
                {
                    Text = slider.Value.ToString()
                };
                textPanel.AddChild(emptyLabel);
                sliderPanel.AddChild(label);
                sliderPanel.AddChild(slider);
                textPanel.AddChild(outText);
                listOut.Add(new ControlPair<Slider, Label>(slider, outText));
            }
            return listOut;
        }
        public static List<ComboBox> addCombobox(List<string> comboItems, string name, string startIndex, int width,StackPanel comboPanel, string comboLabel, StackPanel textPanel)
        {
            List<ComboBox> listOut = new List<ComboBox>();
            Label label = new Label();
            label.Text = comboLabel;

            ComboBox comboBox = new ComboBox();
            foreach (string item in comboItems)
            {
                comboBox.Items.Add(item);
            }
            comboBox.Width = width;
            comboBox.Name = name;
            int defaultIndex = comboBox.Items.IndexOf(startIndex);
            comboBox.SelectedObject = comboBox.Items[defaultIndex];
            listOut.Add(comboBox);
            comboPanel.AddChild(comboBox);
            textPanel.AddChild(label); 
            
            return listOut;
        }
        internal void HookEvents(BoidManager boidManager)
        {

            // Button hooking
            ButtonHandlers.addOrRemButtons(_addbuttons, boidManager);
            ButtonHandlers.addOrRemButtons(_rembuttons, boidManager);

            // Slider hooking
            ButtonHandlers.sliderHandling(_boidSlider);

            // Combobox handling
            ButtonHandlers.bcHandling(_bcCond);
        }
        public void drawUI(Game game)
        {
            Gum.Initialize(game);

            // Bottom container where all the control parts are kept 
            ContainerRuntime bottomContainer = new ContainerRuntime();
            bottomContainer.Name = "bottomPanel";
            bottomContainer.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            bottomContainer.AddToManagers(SystemManagers.Default, null);
            //bottomContainer.AutoGridHorizontalCells = 3;
            //bottomContainer.AutoGridVerticalCells = 1;
            bottomContainer.Width = Constants.SWidth;
            //bottomContainer.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;
            bottomContainer.AddToRoot();
            bottomContainer.Dock(Dock.Bottom);

            // Color rectanagle created
            ColoredRectangleRuntime bottomBack = new ColoredRectangleRuntime(); 
            bottomBack.Height = Constants.PHeight;
            bottomBack.Width = Constants.PWidth;
            bottomBack.Color = Color.SlateGray;
            bottomBack.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            bottomBack.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;
            bottomBack.Dock(Dock.Bottom);
            bottomContainer.AddChild(bottomBack);

            ////////////////////////////////
            // Button containers created //
            //////////////////////////////
            ContainerRuntime buttonContainer = new ContainerRuntime();
            buttonContainer.Name = "buttonContainer";
            buttonContainer.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            buttonContainer.StackSpacing = 3;
            buttonContainer.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;

            StackPanel buttonPanel = new StackPanel();
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Spacing = 3;

            StackPanel addButtons = new StackPanel();
            addButtons.Spacing = 3;
            StackPanel remButtons = new StackPanel();
            remButtons.Spacing = 3;

            List<int> buttonName = new List<int> { 1, 10, 100 };
            _addbuttons = AddButtonRow("Add boid", 125, buttonName, "+", addButtons);
            _rembuttons = AddButtonRow("Remove boid", 125, buttonName, "-", remButtons);

            // Nesting from outer to inner (Button stacks)
            bottomBack.AddChild(buttonPanel);
            buttonPanel.AddChild(buttonContainer);
            buttonContainer.AddChild(addButtons);
            buttonContainer.AddChild(remButtons);

            ////////////////////////////////
            // Slider containers created //
            //////////////////////////////
            ContainerRuntime slideContainer = new ContainerRuntime();
            slideContainer.Name = "slideContainer";
            slideContainer.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            slideContainer.AutoGridHorizontalCells = 2;
            slideContainer.AutoGridVerticalCells = 1;
            slideContainer.StackSpacing = 3;
            slideContainer.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;

            StackPanel boidPanel = new StackPanel();
            boidPanel.Spacing = 3;
    
            StackPanel boidLabelPanel = new StackPanel();
            boidLabelPanel.Spacing = 3;

            // Creating the sliders
            List<string> sliderNames = new List<string> { "Cohesion", "Seperation", "Alignment" };
            _boidSlider = UI.AddSliderRow(125, sliderNames, boidPanel, boidLabelPanel);

            // Nesting from outer to inner (Button stacks)
            bottomBack.AddChild(slideContainer);
            slideContainer.AddChild(boidPanel);
            slideContainer.AddChild(boidLabelPanel);

            //////////////////////////////
            // Info containers created //
            ////////////////////////////
            ContainerRuntime infoContainer = new ContainerRuntime();
            infoContainer.Name = "infoContainer";
            infoContainer.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            infoContainer.AutoGridHorizontalCells = 2;
            infoContainer.AutoGridVerticalCells = 1;
            infoContainer.StackSpacing = 3;
            infoContainer.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;

            StackPanel infoPanel = new StackPanel();
            infoPanel.Spacing = 3;

            StackPanel infoLabel = new StackPanel();
            infoLabel.Spacing = 3;

            // Creating info boxes
            List<string> bcItems = new List<string> { "Steer", "Wrap", "Bounce" };
            _bcCond = addCombobox(bcItems, "bcCondition", "Steer", 125,infoPanel,"Boundary Conditions",infoLabel);

            // Nesting from out to inner (Info stack)
            bottomBack.AddChild(infoContainer);
            infoContainer.AddChild(infoPanel);
            infoContainer.AddChild(infoLabel);

        }
    
    }
}