using AssetManagementBase;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;

namespace Boids;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    KeyboardState _prevKeyboardState;
    
    BoidManager _boidManager;

    Desktop _desktop;
    
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
        // TODO: Add your initialization logic here
        base.Initialize();

        // Setting up the UI interface
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Texture2D texture = Content.Load<Texture2D>("circle");
        _boidManager = new BoidManager(texture);

        // Setting up Myra
        MyraEnvironment.Game = this;

        // Style setup
        AssetManager assets = AssetManager.CreateResourceAssetManager(typeof(Game1).Assembly, "Content.stylesheets.");
        string styleName = "ui_stylesheet.xmms";
        Stylesheet stylesheet = assets.LoadStylesheet(styleName);
        Stylesheet.Current = stylesheet;

        // Setting up grid
        Grid grid = new Grid
        {
            ColumnSpacing = 8,
            RowSpacing = 8,
            Padding = new Thickness(10),
            Background = new Myra.Graphics2D.Brushes.SolidBrush(Color.DarkSlateBlue)
        };

        // Grid size
        grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

        Button button1 = Button.CreateTextButton("Add boid");
        button1.Click += (s, a) =>
        {
            for (int i = 0; i < 10; i++)
            {
                _boidManager.SpawnBoid();
            }
        };
        grid.Widgets.Add(button1);
        Grid.SetColumn(button1, 0);
        Grid.SetRow(button1, 0);

        // Create the bottom panel
        Panel panel = new Panel
        {
            Width = Constants.PWidth,
            Height = Constants.PHeight,
            //Background = new Myra.Graphics2D.Brushes.SolidBrush(Color.Gray),
            Top = Constants.SHeight - Constants.PHeight
        };
        panel.Widgets.Add(grid);

        // Deskop rendered
        _desktop = new Desktop
        {
            Root = panel
        };

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
        // TODO: Add your update logic here
        // Updating the boids movement
        _boidManager.Update(gameTime);
        _prevKeyboardState = current;
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin(SpriteSortMode.FrontToBack,BlendState.AlphaBlend);
        //_spriteBatch.Draw(boidEntity.texture, boidEntity.position, Color.CornflowerBlue);
        _boidManager.Draw(_spriteBatch); 
        _spriteBatch.End();

        // Rendering the desktop
        _desktop.Render();

        // Drawing the game
        base.Draw(gameTime);
    }
}
