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
    StackPanel mainPanel;
    StackPanel buttonPanel;
    StackPanel removePanel;
    StackPanel slidePanel;
    StackPanel infoPanel;
    KeyboardState _prevKeyboardState;
    
    BoidManager _boidManager;

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
        // Bottom container
        ContainerRuntime bottomPanel = new ContainerRuntime();
        bottomPanel.AddToManagers(SystemManagers.Default, null);
        bottomPanel.AddToRoot();
        bottomPanel.Dock(Dock.Bottom);

        // Color rectanagle created
        RectangleRuntime bottomBack = new RectangleRuntime();
        //bottomBack
        bottomBack.Height = Constants.PHeight;
        bottomBack.Width = Constants.PWidth;
        bottomBack.Color = Color.SlateGray;
        bottomPanel.AddChild(bottomBack);

        // Containers created
        buttonPanel = new StackPanel();
        //buttonPanel.Visual.AddToRoot();
        buttonPanel.Anchor(Anchor.BottomLeft);
        buttonPanel.Spacing = 3;


        slidePanel = new StackPanel();
        slidePanel.Visual.AddToRoot();
        slidePanel.Anchor(Anchor.Bottom);
        slidePanel.Spacing = 3;

        infoPanel = new StackPanel();
        infoPanel.Visual.AddToRoot();
        infoPanel.Anchor(Anchor.BottomRight);
        infoPanel.Spacing = 3;

        bottomPanel.AddChild(buttonPanel);

        // Button Container
        Label boidLabel = new Label();
        boidLabel.Text = "Add Spawn";

        Button addOneBtn = new Button();
        addOneBtn.Text = "1x";
        addOneBtn.Visual.Width = 200;

        Button addTenBtn = new Button();
        addTenBtn.Text = "10x";
        addTenBtn.Visual.Width = 200;

        Button addHunBtn = new Button();
        addHunBtn.Text = "100x";
        addHunBtn.Visual.Width = 200;

        buttonPanel.AddChild(boidLabel);

        buttonPanel.AddChild(addOneBtn);
        buttonPanel.AddChild(addTenBtn);
        buttonPanel.AddChild(addHunBtn);

        addOneBtn.Click += (_, _) =>
            _boidManager.SpawnBoid();

        addTenBtn.Click += (_, _) =>{
            for (int i = 1; i <= 10; i++)
            {
                _boidManager.SpawnBoid();
            }
        };
        addHunBtn.Click += (_, _) =>
        {
            for (int i = 1; i <= 100; i++)
            {
                _boidManager.SpawnBoid();
            }
        };

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
