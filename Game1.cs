using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;

using Boids.Shared;

namespace Boids;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    GumService Gum => GumService.Default;
    KeyboardState _prevKeyboardState;
    BoidManager _boidManager;
    PlayerEntity _player;
    private SimUI _simUI;
    private StartupUI _startupUI;
    public enum GameMode { None, Simulation, Player }
    private GameMode _gamemode = GameMode.None;
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
        SetupStartup();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(gameTime);
        KeyboardState current = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        {
            Exit();
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            _gamemode = GameMode.None;
            SetupStartup();
        }

        // Updating the boids and player movement
        switch (_gamemode)
        {
            case GameMode.None:
                Gum.Update(gameTime);
                break;
            case GameMode.Simulation:
                _boidManager.Update(gameTime);
                Gum.Update(gameTime);
                break;
            case GameMode.Player:
                _player.Update(gameTime, current, _prevKeyboardState);
                _boidManager.Update(gameTime, _player.Position, _player._eatRadius,_player.EatBoid);
                break;
        }
        _prevKeyboardState = current; // Used to keep track if key is pressed multiple times

        base.Update(gameTime);
    }
    private void SetupStartup()
    {
        Gum.Root.Children.Clear();
        if (_startupUI == null)
        {
            _startupUI = new StartupUI();
            _startupUI.BuildUI(this);
            _startupUI.HookEvents();
        }
        _startupUI.ShowUI();
        _startupUI.OnSimulationModeClicked = () =>
        {
            _gamemode = GameMode.Simulation;
            SetupSimulation();
        };
        _startupUI.OnPlayerModeClicked = () =>
        {
            _gamemode = GameMode.Player;
            SetupPlayerMode();
        };
        _startupUI.OnExitClicked = () =>
        {
            Exit();
        };
    }
    private void SetupSimulation()
    {
        // Textures and boids created
        Texture2D boidTexture = Content.Load<Texture2D>("circle");
        _boidManager = new BoidManager(boidTexture);

        // New UI drawn
        Gum.Root.Children.Clear();
        if (_simUI == null)
        {
            _simUI = new SimUI();
            _simUI.BuildUI();
            _simUI.HookEvents(_boidManager);
        }
        _simUI.ShowUI();
    }
    private void SetupPlayerMode()
    {
        // UI cleared
        Gum.Root.Children.Clear();

        // Constants modified
        Constants.PHeight = 0;

        // Textures, boids and player initialized
        Texture2D boidTexture = Content.Load<Texture2D>("circle");
        Texture2D playerTexture = Content.Load<Texture2D>("red_circle");
        _player = new PlayerEntity(playerTexture, new Vector2(Constants.ActiveWidth / 2, Constants.ActiveHeight / 2), new Vector2(0, 0),PlayerConstants.eatRadiusFactor);
        _boidManager = new BoidManager(boidTexture);
        for (int i = 0; i < 50; i++) _boidManager.SpawnBoid();
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        switch (_gamemode)
        {
            case GameMode.Simulation:
                _boidManager.Draw(_spriteBatch);
                break;
            case GameMode.Player:
                _player.Draw(_spriteBatch);
                _boidManager.Draw(_spriteBatch);
                break;
        }
        _spriteBatch.End();
        switch (_gamemode)
        {
            case GameMode.None:
            case GameMode.Simulation:
                Gum.Draw();
                break;
        }

        // Drawing the game
        base.Draw(gameTime);
    }
}
