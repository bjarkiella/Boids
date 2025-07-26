using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Gum.DataTypes.Variables;
using Gum.Wireframe;
using Microsoft.VisualBasic;
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
    PlayerEntity _player;
    private UI _ui;

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
        _ui = new UI();
        _ui.drawUI(this);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Boids initialized
        Texture2D texture = Content.Load<Texture2D>("circle");
        _boidManager = new BoidManager(texture);

        // Player created
        Texture2D playerTexture = Content.Load<Texture2D>("red_circle");
        _player = new PlayerEntity(playerTexture,new Vector2(Constants.ActiveWidth/2,Constants.ActiveHeight/2),new Vector2(0,0),PlayerConstants.visionFactor);

        // Events hooked on UI
        _ui.HookEvents(_boidManager);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState current = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // Updating the boids and player movement
        _boidManager.Update(gameTime);
        _player.Update(gameTime, current, _prevKeyboardState);
        _prevKeyboardState = current;

        Gum.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        _player.Draw(_spriteBatch);
        _boidManager.Draw(_spriteBatch);
        _spriteBatch.End();

        // Drawing the UI
        Gum.Draw();

        // Drawing the game
        base.Draw(gameTime);
    }
}
