using System;
using System.Collections.Generic;
using Boids.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boids.Background
{
    internal static class BackgroundUtils 
    {
        public static Rectangle AspectBackground(Texture2D background, int? xPos = null, int? yPos = null)
        {
            int width, height;

                float scale = MathF.Max((float)Constants.ActiveWidth / (float)background.Width, (float)Constants.ActiveHeight / (float)background.Height);
                width = (int)(background.Width * scale);
                height = (int)(background.Height * scale);

            // Position: null = center, provided = use exact position
            int x = xPos ?? (Constants.ActiveWidth - width) / 2;
            int y = yPos ?? (Constants.ActiveHeight - height) / 2;

            return new Rectangle(x, y, width, height);
        }

        public static List<Rectangle> LoadCloudFrames(int[] width, int[] heights, int[] offsetX, int[] offsetY)
        {
            List<Rectangle> frames = [];
            for (int i = 0; i<width.Length;i++)
            {
                frames.Add(new(offsetX[i],offsetY[i],width[i],heights[i])); 
            }
            return frames;
        }
        public static List<Rectangle> TileBackground( Texture2D background, float widthScale = 1.0f, float heightScale = 1.0f, bool tileX = true, bool tileY = true, float xPos= 0f, float yPos= 0f)      
        {
            // Validate scales (same as StretchBackground)
            if (widthScale <= 0 || float.IsNaN(widthScale) || float.IsInfinity(widthScale))
            {
                throw new ArgumentOutOfRangeException(nameof(widthScale), "Width scale needs to be a positive float number");
            }
            if (heightScale <= 0 || float.IsNaN(heightScale) || float.IsInfinity(heightScale))
            {
                throw new ArgumentOutOfRangeException(nameof(heightScale), "Height scale needs to be a positive float number");
            }

            // Calculate scaled dimensions
            int scaledWidth = (int)(background.Width * widthScale);
            int scaledHeight = (int)(background.Height * heightScale);

            // Convert float start positions to int when creating the base position
            int baseX = (int)xPos;
            int baseY = (int)yPos;

            List<Rectangle> tiles = [];

            // Calculate how many tiles needed
            int tilesX = tileX ? (int)Math.Ceiling((float)Constants.ActiveWidth / scaledWidth) : 1;
            int tilesY = tileY ? (int)Math.Ceiling((float)Constants.ActiveHeight / scaledHeight) : 1;

            // Create tiles
            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    tiles.Add(new Rectangle(
                                baseX + (x * scaledWidth),   // Offset by width
                                baseY + (y * scaledHeight),  // Offset by height
                                scaledWidth,
                                scaledHeight
                                ));
                }
            }

            return tiles;
        }

        public static Rectangle StretchBackground(Texture2D background, float widthScale = 1.0f, float heightScale = 1.0f, int? xPos = null, int? yPos = null)
        {
            int width, height;
            if (widthScale <= 0 || float.IsNaN(widthScale) || float.IsInfinity(widthScale)) 
            {
                throw new ArgumentOutOfRangeException(nameof(widthScale), "Width scale needs to be a positive float number");
            }
            if (heightScale<= 0  || float.IsNaN(heightScale) || float.IsInfinity(heightScale))
            {
                throw new ArgumentOutOfRangeException(nameof(heightScale), "Height scale needs to be a positive float number");
            }

            width = (int)(background.Width * widthScale);
            height = (int)(background.Height * heightScale);

            // Position: null = center, provided = use exact position
            int x = xPos ?? (Constants.ActiveWidth - width) / 2;
            int y = yPos ?? (Constants.ActiveHeight - height) / 2;

            return new Rectangle(x, y, width, height);
        }

        public static List<Rectangle> LoadSprites(int frameCount, int frameWidth, int frameHeight, int startX, int startY, int spacingX, int spacingY){
            List<Rectangle> listFrames = [];
            for (int i = 0; i < frameCount; i++) {
                listFrames.Add(new Rectangle(startX + i*(frameWidth+spacingX),startY+spacingY,frameWidth,frameHeight));
            }
            return listFrames;
        } 
        public static List<(Rectangle frame, Vector2 position)> SpritePosition(int noSprites, List<Rectangle> frames, float spriteScale)
        {
            List<(Rectangle frame, Vector2 position)> spritePos = [];
            for (int i = 0;i<noSprites;i++)
            {
                Rectangle randomFrame = frames[Utils.RandomIntRange(0,frames.Count-1)];
                float x = Utils.RandomFloatRange(0,Constants.ActiveWidth - (randomFrame.Width * spriteScale));
                float minY = Constants.ActiveHeight - (randomFrame.Height * spriteScale);  // Fully visible
                float maxY = Constants.ActiveHeight - (randomFrame.Height * spriteScale * 0.5f);  // Half cut off
                float y = Utils.RandomFloatRange(minY, maxY);
                spritePos.Add((randomFrame,new Vector2(x,y)));
            }
            return spritePos;
        }
        public static List<Rectangle> GetTreeBounds(
                List<(Rectangle frame, Vector2 position)> trees,
                float scale)
        {
            List<Rectangle> bounds = [];

            foreach (var (frame, position) in trees)
            {
                // Calculate scaled dimensions
                int scaledWidth = (int)(frame.Width * scale);
                int scaledHeight = (int)(frame.Height * scale);

                // Create collision box
                Rectangle treeBounds = new(
                        (int)position.X,
                        (int)position.Y,
                        scaledWidth,
                        scaledHeight
                        );

                bounds.Add(treeBounds);
            }

            return bounds;
        }
    }
}
