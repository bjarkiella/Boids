using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
    List<Button> _addbuttons;
    List<Button> _rembuttons;
    List<Button> _addpredbuttons;
    List<Button> _rempredbuttons;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferHeight = Constants.SHeight;
        _graphics.PreferredBackBufferWidth = Constants.SWidth;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        Gum.Initialize(this);
        // Bottom container where all the control parts are kept 
        ContainerRuntime bottomContainer = new ContainerRuntime();
        bottomContainer.Name = "bottomPanel";
        bottomContainer.AddToManagers(SystemManagers.Default, null);
        bottomContainer.AutoGridHorizontalCells = 3;
        bottomContainer.AutoGridVerticalCells = 1;
        bottomContainer.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;
        bottomContainer.AddToRoot();
        bottomContainer.Dock(Dock.Bottom);

        // Color rectanagle created
        //RectangleRuntime bottomBack = new RectangleRuntime();
        //bottomBack
        //bottomBack.Height = Constants.PHeight;
        //bottomBack.Width = Constants.PWidth;
        //bottomBack.Color = Color.SlateGray;
        //bottomPanel.AddChild(bottomBack);

        // Containers created
        StackPanel buttonPanel = new StackPanel();
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Spacing = 3;

        ContainerRuntime buttonContainer = new ContainerRuntime();
        buttonContainer.AutoGridHorizontalCells = 4;
        buttonContainer.AutoGridVerticalCells = 1;
        buttonContainer.StackSpacing = 3;
        buttonContainer.ChildrenLayout = global::Gum.Managers.ChildrenLayout.LeftToRightStack;

        StackPanel addButtons = new StackPanel();
        addButtons.Spacing = 3;
        StackPanel remButtons = new StackPanel();
        remButtons.Spacing = 3;
        StackPanel addPredButtons = new StackPanel();
        addPredButtons.Spacing = 3;
        StackPanel remPredButtons = new StackPanel();
        remPredButtons.Spacing = 3;

        StackPanel slidePanel = new StackPanel();
        //slidePanel.Visual.AddToRoot();
        //slidePanel.Anchor(Anchor.Bottom);
        slidePanel.Spacing = 3;

        StackPanel infoPanel = new StackPanel();
        //infoPanel.Visual.AddToRoot();
        //infoPanel.Anchor(Anchor.BottomRight);
        infoPanel.Spacing = 3;

        // Nesting from outer to inner (Button stacks)
        bottomContainer.AddChild(buttonPanel);
        buttonPanel.AddChild(buttonContainer);
        buttonContainer.AddChild(addButtons);
        buttonContainer.AddChild(remButtons);
        buttonContainer.AddChild(addPredButtons);
        buttonContainer.AddChild(remPredButtons);

        List<int> buttonName = new List<int> { 1, 10, 100 };
        _addbuttons = UI.AddButtonRow("Add boid", 125, buttonName,"+", addButtons);
        _rembuttons = UI.AddButtonRow("Remove boid", 125, buttonName,"-", remButtons);
        _addpredbuttons = UI.AddButtonRow("Add predator", 125, buttonName,"+", addPredButtons);
        _rempredbuttons = UI.AddButtonRow("Remove predator", 125, buttonName,"+", remPredButtons);

        // Info Container
        Label infoLabel = new Label();
        infoLabel.Text = "Boid Information";

        infoPanel.AddChild(infoLabel);

        // Slider Container
        Label slideLabel = new Label();
        slideLabel.Text = "Boid Controls";

        slidePanel.AddChild(slideLabel);

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
