using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

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
        MyraEnvironment.Game = this;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Texture2D texture = Content.Load<Texture2D>("circle");
        //boidEntity = new BoidEntity(texture, Vector2.Zero, 0.0f, 0.0f); 
        // TODO: use this.Content to load your game content here
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

        base.Draw(gameTime);
    }
}
