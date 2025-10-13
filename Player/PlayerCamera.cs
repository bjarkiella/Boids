using Microsoft.Xna.Framework;

namespace Boids.Player;

public class PlayerCamera(Vector2 camPosition)
{
    public Vector2 CamPosition { get; set; } = camPosition;
    public Matrix Transform {get; private set;}

    public void Follow(Rectangle target, Vector2 screenSize){
        CamPosition = new Vector2(
                target.X + target.Width/2,
                target.Y + target.Height/2);
        Transform = Matrix.CreateTranslation(
                -CamPosition.X + screenSize.X/2, 
                -CamPosition.Y + screenSize.Y/2,
                0f);
    }
}
