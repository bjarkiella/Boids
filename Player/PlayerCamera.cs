using Microsoft.Xna.Framework;

namespace Boids.Player;

public class PlayerCamera(Vector2 camPosition)
{
    public Vector2 CamPosition { get; set; } = camPosition;

    public void Follow(Rectangle target, Vector2 screenSize){
        CamPosition = new Vector2(
                -target.X + (screenSize.X/2 - target.Width/2),
                -target.Y + (screenSize.Y/2 - target.Height/2));
    }
}
