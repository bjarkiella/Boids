using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;

using Boids.Shared;
using Boids.Boids;
using Boids.Player;
using Boids.ui;

namespace Boids
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        GumService Gum => GumService.Default;
        KeyboardState _prevKeyboardState;
        BoidManager _boidManager;
        PlayerEntity _player;
        Animation _playerAnimation;
        Animation _boidAnimation;
        // PlayerCamera _playerCamera;
        private SimUI _simUI;
        private StartupUI _startupUI;
        public enum GameMode { None, Simulation, Player }
        private GameMode _gamemode = GameMode.None;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = Constants.SHeight;
            _graphics.PreferredBackBufferWidth = Constants.SWidth;
            // _playerCamera = new (Vector2.Zero);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }
        private void OnResize(object sender, EventArgs e)
        {
            Constants.SWidth = Window.ClientBounds.Width;
            Constants.SHeight = Window.ClientBounds.Height;
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
            Texture2D birdiesSheet = Content.Load<Texture2D>("Birdies");

            // Animation for player
            int frameCount = 5;
            int frameWidth = 28;
            int frameHeight = 25;
            int startColumn = 1;
            int startRow = 3;
            List<Rectangle> playerFrames = Animation.LoadAnimation(frameCount,frameWidth,frameHeight,startColumn * frameWidth, startRow * frameHeight);
            _playerAnimation = new(birdiesSheet, playerFrames, 0.1f, true);

            // Animation for boids 
            frameCount = 5;
            frameWidth = 15;
            frameHeight = 21;
            startColumn = 12;
            startRow = 0;
            List<Rectangle> boidFrames = Animation.LoadAnimation(frameCount,frameWidth,frameHeight,startColumn * frameWidth, startRow * frameHeight);
            _boidAnimation = new(birdiesSheet, boidFrames, 0.1f, true);
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
                    _boidManager.Update();
                    Gum.Update(gameTime);
                    break;
                case GameMode.Player:
                    _player.Update(current, _prevKeyboardState);
                    _boidManager.Update(_player.Position, _player.EatRadius,_player.EatBoid);
                    // _playerCamera.Follow(_player.PlayerBox,Constants.ScreenSize);
                    // Constants.CameraPosition = _playerCamera.CamPosition;
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
            // List<Rectangle> simBoidFrames = Animation.LoadAnimation(5,15,21,0,13);
            // Texture2D boidTexture = Content.Load<Texture2D>("circle");
            _boidManager = new BoidManager(_boidAnimation);

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
            // Texture2D boidTexture = Content.Load<Texture2D>("circle");
            // Texture2D playerTexture = Content.Load<Texture2D>("red_circle");
            _player = new PlayerEntity(_playerAnimation, new Vector2(Constants.ActiveWidth / 2, Constants.ActiveHeight / 2), new Vector2(0, 0),PlayerConstants.eatRadiusFactor);
            _boidManager = new BoidManager(_boidAnimation);
            for (int i = 0; i < 150; i++) _boidManager.SpawnBoid();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (_gamemode)
            {
                case GameMode.Simulation:
                    _spriteBatch.Begin(
                            SpriteSortMode.Deferred,
                            BlendState.AlphaBlend,
                            SamplerState.PointClamp,  
                            depthStencilState: null,
                            rasterizerState: null
                            );
                    _boidManager.Draw(_spriteBatch);
                    _spriteBatch.End();
                    break;
                case GameMode.Player:
                    _spriteBatch.Begin(
                            SpriteSortMode.Deferred,
                            BlendState.AlphaBlend,
                            SamplerState.PointClamp,   
                            depthStencilState: null,
                            rasterizerState: null,
                            effect: null
                            // transformMatrix: _playerCamera.Transform
                            );
                    _player.Draw(_spriteBatch);
                    _boidManager.Draw(_spriteBatch);
                    _spriteBatch.End();
                    break;
            }
            switch (_gamemode)
            {
                case GameMode.None:
                case GameMode.Simulation:
                    _spriteBatch.Begin(
                            SpriteSortMode.Deferred,
                            BlendState.AlphaBlend,
                            SamplerState.PointClamp, 
                            depthStencilState: null,
                            rasterizerState: null
                            );
                    Gum.Draw();
                    _spriteBatch.End();
                    break;
            }

            // Drawing the game
            base.Draw(gameTime);
        }
    }
}
