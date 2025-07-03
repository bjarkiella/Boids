using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gum.DataTypes.Variables;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using RenderingLibrary;
using RenderingLibrary.Math.Geometry;

namespace Boids;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    GumService Gum => GumService.Default;
    KeyboardState _prevKeyboardState;

    BoidManager _boidManager;
    List<Button> _addbuttons,_rembuttons,_addpredbuttons,_rempredbuttons;
    List<ControlPair<Slider, Label>> _boidSlider;
    List<ComboBox> _bcCond;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferHeight = Constants.SHeight;
        _graphics.PreferredBackBufferWidth = Constants.SWidth;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    public static void printSizeCont(ContainerRuntime panel)
    {
        Console.WriteLine("ContainerRuntime: " + "\n" +
        "Name: " + panel.Name + "\n" +
        "Absolotue bottom: " + panel.AbsoluteBottom +"\n" +
        "Absolote Left: " + panel.AbsoluteLeft +"\n" +
        "Absolute Right" + panel.AbsoluteRight +"\n" +
        "Absolute top: " + panel.AbsoluteTop +"\n" +
        "Absolute x: " + panel.AbsoluteX +"\n" +
        "Absoulte y: " + panel.AbsoluteY);
    }
    public static void printSizeStac(StackPanel panel)
    {
        Console.WriteLine("StackPanel: " +"\n" +
        "Name: " + panel.Name + "\n" +
        "Absolote Left: " + panel.AbsoluteLeft +"\n" +
        "Absolute top: " + panel.AbsoluteTop +"\n" +
        "Absolute height: " + panel.ActualHeight+"\n" +
        "Absoulte width: " + panel.ActualWidth);
    }
    protected override void Initialize()
    {

        Gum.Initialize(this);

        // Bottom container where all the control parts are kept 
        ContainerRuntime bottomContainer = new ContainerRuntime();
        bottomContainer.Name = "bottomPanel";
        bottomContainer.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToChildren;
        bottomContainer.AddToManagers(SystemManagers.Default, null);
        bottomContainer.AutoGridHorizontalCells = 3;
        bottomContainer.AutoGridVerticalCells = 1;
        bottomContainer.Width = Constants.SWidth;
        bottomContainer.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;
        bottomContainer.AddToRoot();
        bottomContainer.Dock(Dock.Bottom);
        //printSizeCont(bottomContainer);

        // Color rectanagle created
        //RectangleRuntime bottomBack = new RectangleRuntime();
        //bottomBack
        //bottomBack.Height = Constants.PHeight;
        //bottomBack.Width = Constants.PWidth;
        //bottomBack.Color = Color.SlateGray;
        //bottomPanel.AddChild(bottomBack);

        ////////////////////////////////
        // Button containers created //
        //////////////////////////////
        ContainerRuntime buttonContainer = new ContainerRuntime();
        buttonContainer.Name = "buttonContainer";
        buttonContainer.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToChildren;
        buttonContainer.AutoGridHorizontalCells = 4;
        buttonContainer.AutoGridVerticalCells = 1;
        buttonContainer.StackSpacing = 3;
        buttonContainer.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;
        //printSizeCont(buttonContainer);

        StackPanel buttonPanel = new StackPanel();
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Spacing = 3;
        //printSizeStac(buttonPanel);

        StackPanel addButtons = new StackPanel();
        addButtons.Spacing = 3;
        StackPanel remButtons = new StackPanel();
        remButtons.Spacing = 3;
        StackPanel addPredButtons = new StackPanel();
        addPredButtons.Spacing = 3;
        StackPanel remPredButtons = new StackPanel();
        remPredButtons.Spacing = 3;

        List<int> buttonName = new List<int> { 1, 10, 100 };
        _addbuttons = UI.AddButtonRow("Add boid", 125, buttonName, "+", addButtons);
        _rembuttons = UI.AddButtonRow("Remove boid", 125, buttonName, "-", remButtons);
        _addpredbuttons = UI.AddButtonRow("Add predator", 125, buttonName, "+", addPredButtons,false);
        _rempredbuttons = UI.AddButtonRow("Remove predator", 125, buttonName, "+", remPredButtons,false);

        // Nesting from outer to inner (Button stacks)
        bottomContainer.AddChild(buttonPanel);
        buttonPanel.AddChild(buttonContainer);
        buttonContainer.AddChild(addButtons);
        buttonContainer.AddChild(remButtons);
        buttonContainer.AddChild(addPredButtons);
        buttonContainer.AddChild(remPredButtons);

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
        bottomContainer.AddChild(slideContainer);
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
        _bcCond = UI.addCombobox(bcItems, "bcCondition", "Steer", 125,infoPanel,"Boundary Conditions",infoLabel);

        // Nesting from out to inner (Info stack)
        bottomContainer.AddChild(infoContainer);
        infoContainer.AddChild(infoPanel);
        infoContainer.AddChild(infoLabel);


        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Texture2D texture = Content.Load<Texture2D>("circle");
        _boidManager = new BoidManager(texture);

        // Button hooking
        ButtonHandlers.addOrRemButtons(_addbuttons, _boidManager);
        ButtonHandlers.addOrRemButtons(_rembuttons, _boidManager);

        // Slider hooking
        ButtonHandlers.sliderHandling(_boidSlider);

        // Combobox handling
        ButtonHandlers.bcHandling(_bcCond);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState current = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (current.IsKeyDown(Keys.Space) && !_prevKeyboardState.IsKeyDown(Keys.Space))
        {
            for (int i = 1; i <= 10; i++)
            {
                _boidManager.SpawnBoid();
            }
        }
        // Updating the boids movement
        _boidManager.Update(gameTime);
        _prevKeyboardState = current;

        Gum.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        _boidManager.Draw(_spriteBatch);
        _spriteBatch.End();

        Gum.Draw();
        // Drawing the game
        base.Draw(gameTime);
    }
}
