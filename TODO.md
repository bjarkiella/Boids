## TODO stuff
- Add a hide mechanism for the clouds, so if a player is within a rect of a cloud, he becomes "invisible". 
-- Perhaps lowering the vision radius so the birds have to get really close to see the player
-- Add transperancy to the player
- Add particles for the player when sprinting
- Add particles for the birds when fleeing (! mark or something like that)
- Add particles for when a bird is eaten
- Add slow down effect when birds and players go through trees
- Refactor some of the background thingy in Game1.cs and the background classes

## Implementation Approach - Cloud transperancy

Key findings:

1. Player draws at PlayerEntity.Draw() line 124-135 using Color.White
2. Clouds are ParallaxEntity objects stored in ParallaxLayer._parralaxEntity (line 17)
3. Player has PlayerBox property (line 28-32) that returns a Rectangle for collision detection
4. Clouds have position (_position) and dimensions (_sourceRect with _scale)

## What You Need to Do

### 1. Add transparency state to PlayerEntity

Add a field to track the current opacity:

private float _opacity = 1.0f; // 1.0 = fully visible, 0.5 = 50% transparent

### 2. Expose cloud rectangles from ParallaxManager

You'll need a way to check which clouds overlap the player. Add a method to ParallaxManager:

public List<Rectangle> GetCloudBounds()
{
    List<Rectangle> cloudBounds = [];
    foreach (var layer in _layers)
        cloudBounds.AddRange(layer.GetCloudBounds());
    return cloudBounds;
}

And in ParallaxLayer:

public List<Rectangle> GetCloudBounds()
{
    List<Rectangle> bounds = [];
    foreach (var entity in _parralaxEntity)
        bounds.Add(entity.GetBounds());
    return bounds;
}

And in ParallaxEntity:

public Rectangle GetBounds()
{
    return new Rectangle(
        (int)(_position.X - _sourceRect.Width * _scale / 2f),
        (int)(_position.Y - _sourceRect.Height * _scale / 2f),
        (int)(_sourceRect.Width * _scale),
        (int)(_sourceRect.Height * _scale)
    );
}

### 3. Check for overlaps in PlayerEntity.Update()

After movement calculations, check if player is under any clouds:

// At the end of Update(), add:
public void UpdateOpacity(List<Rectangle> cloudBounds)
{
    bool underCloud = false;
    Rectangle playerBox = PlayerBox;

    foreach (var cloudRect in cloudBounds)
    {
        if (playerBox.Intersects(cloudRect))
        {
            underCloud = true;
            break;
        }
    }

    // Smoothly transition opacity
    float targetOpacity = underCloud ? 0.4f : 1.0f; // 40% transparent when hidden
    float transitionSpeed = 3.0f; // How fast it transitions

    if (_opacity < targetOpacity)
        _opacity = Math.Min(_opacity + transitionSpeed * Dt, targetOpacity);
    else if (_opacity > targetOpacity)
        _opacity = Math.Max(_opacity - transitionSpeed * Dt, targetOpacity);
}

### 4. Update PlayerEntity.Draw() to use opacity

Change line 129-131:

Color drawColor = Color.White * _opacity; // This multiplies alpha

if (currentFace == DirFace.Right)
    sb.Draw(_animation.Texture, Position, _animation.CurrentFrame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
else if (currentFace == DirFace.Left)
    sb.Draw(_animation.Texture, Position, _animation.CurrentFrame, drawColor, 0f, origin, scale, SpriteEffects.
FlipHorizontally, 0f);

### 5. Wire it up in Game1.cs Update()

In the GameMode.Player case (around line 166-171):

case GameMode.Player:
    _largeCloudPLManager.Update();
    _smallCloudPLManager.Update();
    _player.Update(current, _prevKeyboardState);

    // Get all cloud bounds and update player opacity
    List<Rectangle> allClouds = [];
    allClouds.AddRange(_largeCloudPLManager.GetCloudBounds());
    allClouds.AddRange(_smallCloudPLManager.GetCloudBounds());
    _player.UpdateOpacity(allClouds);

    _boidManager.Update(_player.Position, _player.EatRadius, _player.EatBoid);
    break;

## Optional Enhancement: Reduce Vision Radius Too

If you also want to reduce the player's vision radius (so boids have to get closer), you could:

• Add public float OpacityFactor => _opacity; to PlayerEntity
• Multiply _player.EatRadius * _player.OpacityFactor when passing it to _boidManager.Update()

This would make the player's detection radius shrink proportionally with transparency!

Does this approach make sense? Would you like me to help you implement any specific part?
Great! Now let me create a summary of what MonoGame.Extended offers and how to install it:

## What's in MonoGame.Extended?

Based on the documentation, here are the main features you'll get:

### Most Relevant to Your Project:

1. Shapes & Primitives - Draw circles, rectangles, lines, polygons without textures
2. Camera - Better 2D camera with zoom, rotation, follow mechanics
3. Input Extensions - Enhanced keyboard/mouse handling
4. Sprite Batch Extensions - More drawing utilities

### Other Cool Features:

• Texture Handling - Sprite sheets, texture atlases
• 2D Animations - Sprite animation system
• Bitmap Fonts - Custom fonts with BMFont
• Particles - Particle system (you mentioned wanting this!)
• Collision Detection - Spatial partitioning, quadtrees
• Tiled Map Support - Load Tiled maps
• ECS (Entity Component System) - Alternative architecture
• Object Pooling - Performance optimization
• Tweening - Smooth animations/transitions

## How to Install

Since you're on READ-ONLY mode, here's what you'll need to do:

### 1. Add the NuGet package to your project:

dotnet add package MonoGame.Extended --version 5.1.1

(Latest stable version is 5.1.1 as of October 2025)

### 2. Add the using statement in PlayerEntity.cs:

using MonoGame.Extended;

### 3. Use it to draw a debug circle:

// In PlayerEntity.Draw() method
if (_showDebugVision)
{
    _spriteBatch.DrawCircle(Position, VisionRadius, 32, Color.Red * 0.3f, 2f);
}

That's it! The DrawCircle extension method will be available on SpriteBatch.

Parameters:

• Position - Center point
• VisionRadius - Radius
• 32 - Number of segments (higher = smoother circle)
• Color.Red * 0.3f - Color with transparency
• 2f - Line thickness
