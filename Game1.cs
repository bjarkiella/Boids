using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Gum.Wireframe;

using Boids.Shared;
using Boids.Boids;
using Boids.Player;
using Boids.ui;
using Boids.Background;

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

        Animation _sprintAnimation;
        Animation _bloodAnimation;
        Animation _alertAnimation;

        BackgroundResources _backgroundResources;
        BoidResources _boidResources;
        PlayerResources _playerResources;

        List<(Rectangle frame, Vector2 position)> _staticTrees = [];
        readonly float _treeScale = 4.0f;

        public static float BoidVisionRadius {get; private set;}
        // PlayerCamera _playerCamera;
        private SimUI _simUI;
        private StartupUI _startupUI;
        private TimerUI _timerUI = new();
        private BoidCountUI _boidCountUI = new();

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
            // Update dimension constants
            Constants.SWidth = Window.ClientBounds.Width;
            Constants.SHeight = Window.ClientBounds.Height;
            Constants.PWidth = Constants.SWidth;

            GumService.Default.Root.Width = Constants.SWidth;
            GumService.Default.Root.Height = Constants.SHeight;
            
            // Resize UI if in simulation mode
            if (_gamemode == GameMode.Simulation && _simUI != null)
            {
                _simUI.ReSizeUI(Constants.SWidth, Constants.SHeight);
            }
            // Resize timer UI if in player mode
            if (_gamemode == GameMode.Player && _timerUI != null)
            {
                _timerUI.ReSizeUI(Constants.SWidth, Constants.SHeight);
            }
            // Resize boid counter UI if in player mode
            if ((_gamemode == GameMode.Player || _gamemode == GameMode.Simulation) && _boidCountUI != null)
            {
                _boidCountUI.ReSizeUI(Constants.SWidth, Constants.SHeight);
            }
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
            Texture2D skyBackground = Content.Load<Texture2D>("sky");
            Texture2D treeBackground = Content.Load<Texture2D>("Tlayer2");
            Texture2D treeDarkBackground = Content.Load<Texture2D>("Tlayer3");

            // Static background - Trees
            int frameCount= 5;
            int frameWidth= 28;
            int frameHeight= 64;
            int startX= 21;   
            int startY= 0;
            int spacingX= 3;  
            int spacingY= 0;  
            Texture2D treeSheet = Content.Load<Texture2D>("Textures&trees");
            List<Rectangle> treeFrames = BackgroundUtils.LoadSprites(frameCount,frameWidth,frameHeight,startX,startY,spacingX,spacingY);

            // Moving background
            Texture2D largeCloudSheet = Content.Load<Texture2D>("clouds_big");
            Texture2D smallCloudSheet = Content.Load<Texture2D>("clouds_small");

            // Moving background - Large Clouds 
            int[] cloudWidth = [36,40];
            int[] cloudHeight =[24,24];
            int[] cloudOffX = [15,45];
            int[] cloudOffY = [6,11];
            List<Rectangle> _largeClouds = BackgroundUtils.LoadCloudFrames(cloudWidth,cloudHeight,cloudOffX,cloudOffY); 

            // Moving background - Small Clouds 
            cloudWidth = [16,20,20];
            cloudHeight =[10,16,16];
            cloudOffX = [0,16,11];
            cloudOffY = [0,0,6];
            List<Rectangle> _smallClouds = BackgroundUtils.LoadCloudFrames(cloudWidth,cloudHeight,cloudOffX,cloudOffY); 

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

            // Particles 
            Texture2D particleSheet = Content.Load<Texture2D>("effects");

            // Particles - Blood
            List<Rectangle> _bloodParticles = [
                new Rectangle(x:107,y:175,width:10,height:6),
                new Rectangle(x:135,y:173,width:18,height:8),
                new Rectangle(x:164,y:169,width:11,height:15),
                new Rectangle(x:178,y:170,width:10,height:16)];
            _bloodAnimation = new(particleSheet, _bloodParticles, 0.1f, true);

            // Particles - Sprint
            List<Rectangle> _sprintParticles = [
                new Rectangle(x:131,y:130,width:26,height:28),
                new Rectangle(x:164,y:130,width:26,height:28),
                new Rectangle(x:196,y:130,width:26,height:28),
                new Rectangle(x:226,y:130,width:26,height:28),
                new Rectangle(x:258,y:130,width:26,height:28),
                new Rectangle(x:291,y:130,width:26,height:28),
                new Rectangle(x:323,y:130,width:26,height:28)];
            _sprintAnimation = new(particleSheet, _sprintParticles, 0.1f, true);

            // Particles - Alert 
            List<Rectangle> _alertParticles = [
                new Rectangle(x:525,y:164,width:4,height:13),
                new Rectangle(x:589,y:165,width:4,height:13)];
            _alertAnimation = new(particleSheet, _alertParticles, 0.1f,true);

            // Some variable exposure, used for player detection
            BoidVisionRadius = BoidConstants.CalculateBoidVisionRadius(_boidAnimation);

            // Resources 
            _boidResources = new()
            {
                BoidAnimation = _boidAnimation,
                AlertParticleAnimation = _alertAnimation, 
                BloodParticleAnimation = _bloodAnimation 
            };
            _playerResources = new () 
            {
                PlayerAnimation = _playerAnimation,
                SprintParticleAnimation = _sprintAnimation
            };
            _backgroundResources = new()
            {
                Sky = skyBackground,
                TreeSheet = treeSheet,
                TreeFrames = treeFrames,
                TreeBackground = treeBackground,
                TreeDarkBackground = treeDarkBackground,
                LargeCloudSheet = largeCloudSheet,
                SmallCloudSheet = smallCloudSheet,
                LargeCloudFrames = _largeClouds,
                SmallCloudFrames = _smallClouds
            };

            // Managers
            _largeCloudPLManager = new ParallaxManager(_backgroundResources.LargeCloudSheet, _backgroundResources.LargeCloudFrames);
            _smallCloudPLManager = new ParallaxManager(_backgroundResources.SmallCloudSheet, _backgroundResources.SmallCloudFrames);
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
                    if (_boidManager.ListOfBoids != null)
                    {
                        int boidsNumber = _boidManager.ListOfBoids.Count;
                        _boidCountUI.UpdateBoidCountDisplay(boidsNumber);
                    }
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
                    
                    if (_boidManager.ListOfBoids != null)
                    {
                        int boidsNumber = _boidManager.ListOfBoids.Count;
                        _boidCountUI.UpdateBoidCountDisplay(boidsNumber);
                    }

                    _timerUI.UpdateTimer();
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
            _staticTrees = BackgroundUtils.SpritePosition(BackgroundConstants.treeCount,_backgroundResources.TreeFrames,_treeScale);

            // Textures and boids created
            _boidManager = new BoidManager(_boidResources);

            // Window size resize 
            Constants.SWidth = Window.ClientBounds.Width;
            Constants.SHeight = Window.ClientBounds.Height;
            Constants.PWidth = Constants.SWidth;
            Constants.PHeight = Constants.DefaultPanelHeight;
            UIUtils.UpdateUISize(Window.ClientBounds.Width,Window.ClientBounds.Height);

            // New UI drawn
            Gum.Root.Children.Clear();
            if (_simUI == null)
            {
                _simUI = new SimUI();
                _simUI.BuildUI(_boidManager);
            }
            _simUI.RebuildAndShowUI(_boidManager);

            _boidCountUI.BuildBoidCountUI();
            _boidCountUI.ShowUI();
        }
        private void SetupPlayerMode()
        {
            // UI cleared
            Gum.Root.Children.Clear();

            // Constants modified
            Constants.PHeight = 0;

            // Background trees initialized (Placed here due to re-size availability)
            _staticTrees = BackgroundUtils.SpritePosition(BackgroundConstants.treeCount,_backgroundResources.TreeFrames,_treeScale);

            // Textures, boids and player initialized
            _player = new PlayerEntity(_playerResources, new Vector2(Constants.ActiveWidth / 2, Constants.ActiveHeight / 2), new Vector2(0, 0),PlayerConstants.eatRadiusFactor);
            _boidManager = new BoidManager(_boidResources);
            for (int i = 0; i < 150; i++) _boidManager.SpawnBoid();

            // Syncing Canvas to re-size
            UIUtils.UpdateUISize(Window.ClientBounds.Width,Window.ClientBounds.Height);

            // Adding overlay UI      
            _timerUI.BuildTimerUI();
            _timerUI.ResetTimer();
            _timerUI.ShowUI();
            _boidCountUI.BuildBoidCountUI();
            _boidCountUI.ShowUI();

        }
        private void DrawBackground(SpriteBatch sb, Rectangle aspectBg, List<Rectangle> tiles, List<Rectangle> darkTiles)
        {

            sb.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,  
                    depthStencilState: null,
                    rasterizerState: null
                    );
            sb.Draw(_backgroundResources.Sky,aspectBg,Color.White);
            foreach (Rectangle tile in tiles)
            {
                sb.Draw(_backgroundResources.TreeBackground, tile, Color.White);
            }
            foreach (Rectangle tile in darkTiles)
            {
                sb.Draw(_backgroundResources.TreeDarkBackground, tile, Color.White);
            }
            foreach(var (frame,pos) in _staticTrees)
            {
                sb.Draw(_backgroundResources.TreeSheet,pos,frame,Color.White,0f,Vector2.Zero,_treeScale,SpriteEffects.None,0f);
            }
            _largeCloudPLManager.Draw(sb);
            _smallCloudPLManager.Draw(sb);
            sb.End();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Background setup here to align with window re-size
            Rectangle aspectBackground = BackgroundUtils.AspectBackground(_backgroundResources.Sky);
            float widthScale = 2f;
            float heightScale = 2f;
            List<Rectangle> tileTrees = BackgroundUtils.TileBackground(
                    _backgroundResources.TreeBackground,
                    widthScale: widthScale,    
                    heightScale: heightScale,
                    tileX: true,       // tile horizontally
                    tileY: false,      // don't tile vertically (since it's at the bottom)
                    xPos: 0,         // start at left edge
                    yPos: Constants.SHeight - _backgroundResources.TreeBackground.Height*heightScale
                    );
            widthScale = 1.5f;
            heightScale = 1.5f;
            List<Rectangle> tileDarkTrees = BackgroundUtils.TileBackground(
                    _backgroundResources.TreeDarkBackground,
                    widthScale: widthScale,    
                    heightScale: heightScale,
                    tileX: true,       // tile horizontally
                    tileY: false,      // don't tile vertically (since it's at the bottom)
                    xPos: 0,         // start at left edge
                    yPos: Constants.SHeight - _backgroundResources.TreeDarkBackground.Height*heightScale
                    );

            switch (_gamemode)
            {
                case GameMode.Simulation:
                    DrawBackground(_spriteBatch,aspectBackground,tileTrees,tileDarkTrees);
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
                    DrawBackground(_spriteBatch,aspectBackground,tileTrees,tileDarkTrees);
                    _spriteBatch.Begin(
                            SpriteSortMode.Deferred,
                            BlendState.AlphaBlend,
                            SamplerState.PointClamp,   
                            depthStencilState: null,
                            rasterizerState: null,
                            effect: null
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
                case GameMode.Player: 
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
