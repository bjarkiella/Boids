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
        Texture2D texture = Content.Load<Texture2D>("circle");
        Texture2D playerTexture = Content.Load<Texture2D>("red_circle");
        _boidManager = new BoidManager(texture);

        // Events hooked on UI
        _ui.HookEvents(_boidManager);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState current = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (current.IsKeyDown(Keys.Up) && !_prevKeyboardState.IsKeyDown(Keys.Up))
        {
            // Move the player up
        }
        if (current.IsKeyDown(Keys.Down) && !_prevKeyboardState.IsKeyDown(Keys.Down))
        {
            
        }
        if (current.IsKeyDown(Keys.Right) && !_prevKeyboardState.IsKeyDown(Keys.Right))
        {

        }
        if (current.IsKeyDown(Keys.Left) && !_prevKeyboardState.IsKeyDown(Keys.Left))
        {

        }
        if (current.IsKeyDown(Keys.Space) && !_prevKeyboardState.IsKeyDown(Keys.Space))
        {
            
        }
        // Updating the boids and predator movement
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

        // Drawing the UI
        Gum.Draw();

        // Drawing the game
        base.Draw(gameTime);
    }
}
