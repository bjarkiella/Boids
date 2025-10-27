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
using Boids.Background;
using System.Globalization;

namespace Boids
{

    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        GumService Gum => GumService.Default;
        KeyboardState _prevKeyboardState;
        BoidManager _boidManager;
        PlayerEntity _player;
        Animation _playerAnimation;
        Animation _boidAnimation;
        ParallaxManager _smallCloudPLManager;
        ParallaxManager _largeCloudPLManager;

        Texture2D _mainBackground;
        Texture2D _treeBackground;
        Texture2D _treeDarkBackground;
        Texture2D _treeSheet;

        Texture2D _bloodParticles;

        List<(Rectangle frame, Vector2 position)> _staticTrees = [];
        readonly float _treeScale = 4.0f;
        List<Rectangle> _treeFrames;

        public static float BoidVisionRadius {get; private set;}
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

            // Static background - Sky
            _mainBackground = Content.Load<Texture2D>("sky");
            _treeBackground = Content.Load<Texture2D>("Tlayer2");
            _treeDarkBackground = Content.Load<Texture2D>("Tlayer3");

            // Static background - Trees
            int frameCount= 5;
            int frameWidth= 28;
            int frameHeight= 64;
            int startX= 21;   
            int startY= 0;
            int spacingX= 3;  
            int spacingY= 0;  
            _treeSheet = Content.Load<Texture2D>("Textures&trees");
            _treeFrames = BackgroundUtils.LoadSprites(frameCount,frameWidth,frameHeight,startX,startY,spacingX,spacingY);

            // Moving background
            Texture2D largeCloudSheet = Content.Load<Texture2D>("clouds_big");
            Texture2D smallCloudSheet = Content.Load<Texture2D>("clouds_small");

            // Moving background - Large Clouds 
            List<int> cloudWidth = [36,40];
            List<int> cloudHeight =[24,24];
            List<int> cloudOffX = [15,45];
            List<int> cloudOffY = [6,11];
            List<Rectangle> _largeClouds = [];
            for (int i = 0; i<cloudWidth.Count;i++)
            {
                Rectangle ble = new(cloudOffX[i],cloudOffY[i],cloudWidth[i],cloudHeight[i]);
                _largeClouds.Add(ble); 
            }
            _largeCloudPLManager = new ParallaxManager(largeCloudSheet, _largeClouds);

            // Moving background - Small Clouds 
            cloudWidth = [16,20,20];
            cloudHeight =[10,16,16];
            cloudOffX = [0,16,11];
            cloudOffY = [0,0,6];
            List<Rectangle> _smallClouds = [];
            for (int i = 0; i<cloudWidth.Count;i++)
            {
                Rectangle ble = new(cloudOffX[i],cloudOffY[i],cloudWidth[i],cloudHeight[i]);
                _smallClouds.Add(ble); 
            }
            _smallCloudPLManager = new ParallaxManager(smallCloudSheet, _smallClouds);

            // Animation
            Texture2D birdiesSheet = Content.Load<Texture2D>("Birdies");

            // Animation - Player
            frameCount = 5;
            frameWidth = 28;
            frameHeight = 25;
            int startColumn = 1;
            int startRow = 3;
            List<Rectangle> playerFrames = Animation.LoadAnimation(frameCount,frameWidth,frameHeight,startColumn * frameWidth, startRow * frameHeight);
            _playerAnimation = new(birdiesSheet, playerFrames, 0.1f, true);

            // Animation - Boids 
            frameCount = 5;
            frameWidth = 15;
            frameHeight = 21;
            startColumn = 12;
            startRow = 0;
            List<Rectangle> boidFrames = Animation.LoadAnimation(frameCount,frameWidth,frameHeight,startColumn * frameWidth, startRow * frameHeight);
            _boidAnimation = new(birdiesSheet, boidFrames, 0.1f, true);

            // Some variable exposure, used for player detection
            BoidVisionRadius = BoidConstants.CalculateBoidVisionRadius(_boidAnimation);
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
                    _largeCloudPLManager.Update();
                    _smallCloudPLManager.Update();
                    _boidManager.Update();
                    Gum.Update(gameTime);
                    break;
                case GameMode.Player:
                    _largeCloudPLManager.Update();
                    _smallCloudPLManager.Update();

                    List<Rectangle> cloudBounds = _largeCloudPLManager.GetEntityBounds();
                    cloudBounds.AddRange(_smallCloudPLManager.GetEntityBounds());
                    List<Rectangle> treeBounds = BackgroundUtils.GetTreeBounds(_staticTrees,_treeScale);
                    _player.Update(current, _prevKeyboardState,cloudBounds,treeBounds);
                    _boidManager.Update(_player.Position, _player.EatRadius ,_player.EatBoid,_player.OpacityFactor);
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
            // Background trees initialized (Placed here due to re-size availability)
            _staticTrees = BackgroundUtils.SpritePosition(BackgroundConstants.treeCount,_treeFrames,_treeScale);

            // Textures and boids created
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

            // Background trees initialized (Placed here due to re-size availability)
            _staticTrees = BackgroundUtils.SpritePosition(BackgroundConstants.treeCount,_treeFrames,_treeScale);

            // Textures, boids and player initialized
            _player = new PlayerEntity(_playerAnimation, new Vector2(Constants.ActiveWidth / 2, Constants.ActiveHeight / 2), new Vector2(0, 0),PlayerConstants.eatRadiusFactor);
            _boidManager = new BoidManager(_boidAnimation);
            for (int i = 0; i < 150; i++) _boidManager.SpawnBoid();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Background setup here to align with window re-size
            Rectangle aspectBackground = BackgroundUtils.AspectBackground(_mainBackground);
            float widthScale = 2f;
            float heightScale = 2f;
            List<Rectangle> tileTrees = BackgroundUtils.TileBackground(
                    _treeBackground,
                    widthScale: widthScale,    
                    heightScale: heightScale,
                    tileX: true,       // tile horizontally
                    tileY: false,      // don't tile vertically (since it's at the bottom)
                    xPos: 0,         // start at left edge
                    yPos: Constants.ActiveHeight - _treeBackground.Height*heightScale
                    );
            widthScale = 1.5f;
            heightScale = 1.5f;
            List<Rectangle> tileDarkTrees = BackgroundUtils.TileBackground(
                    _treeDarkBackground,
                    widthScale: widthScale,    
                    heightScale: heightScale,
                    tileX: true,       // tile horizontally
                    tileY: false,      // don't tile vertically (since it's at the bottom)
                    xPos: 0,         // start at left edge
                    yPos: Constants.ActiveHeight - _treeDarkBackground.Height*heightScale
                    );

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
                    _spriteBatch.Draw(_mainBackground,aspectBackground,Color.White);
                    foreach (var tile in tileTrees)
                    {
                        _spriteBatch.Draw(_treeBackground, tile, Color.White);
                    }
                    foreach (var tile in tileDarkTrees)
                    {
                        _spriteBatch.Draw(_treeDarkBackground, tile, Color.White);
                    }
                    foreach(var (frame,pos) in _staticTrees)
                    {
                        _spriteBatch.Draw(_treeSheet,pos,frame,Color.White);
                    }
                    _largeCloudPLManager.Draw(_spriteBatch);
                    _smallCloudPLManager.Draw(_spriteBatch);
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
                            );
                    _spriteBatch.Draw(_mainBackground,aspectBackground,Color.White);
                    foreach (var tile in tileTrees)
                    {
                        _spriteBatch.Draw(_treeBackground, tile, Color.White);
                    }
                    foreach (var tile in tileDarkTrees)
                    {
                        _spriteBatch.Draw(_treeDarkBackground, tile, Color.White);
                    }
                    foreach(var (frame,pos) in _staticTrees)
                    {
                        _spriteBatch.Draw(_treeSheet,pos,frame,Color.White,0f,Vector2.Zero,_treeScale,SpriteEffects.None,0f);
                    }
                    _largeCloudPLManager.Draw(_spriteBatch);
                    _smallCloudPLManager.Draw(_spriteBatch);
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
