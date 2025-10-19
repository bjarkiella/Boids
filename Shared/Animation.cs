  using System.Collections.Generic;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  namespace Boids.Shared;

  internal class Animation(Texture2D texture, List<Rectangle> frames, float frameDuration, bool isLooping = true, int startFrame = 0)
  {
      private int _currentFrameIndex = startFrame;
      private float _elapsedTime = 0f;

      public Rectangle CurrentFrame => frames[_currentFrameIndex];
      public bool IsFinished => !isLooping && _currentFrameIndex == frames.Count - 1;
      public Texture2D Texture => texture;
      public int FrameWidth => CurrentFrame.Width;
      public int FrameHeight => CurrentFrame.Height;
      public List<Rectangle> Frames => frames;
      public float FrameDuration => frameDuration;
    
      protected static float Dt => Time.Delta;

      public void Update()
      {
          _elapsedTime += Dt;

          if (_elapsedTime >= frameDuration)
          {
              _elapsedTime -= frameDuration;
              _currentFrameIndex++;

              if (_currentFrameIndex >= frames.Count)
              {
                  if (isLooping)
                  {
                      _currentFrameIndex = 0;
                  }
                  else
                  {
                      _currentFrameIndex = frames.Count - 1;
                  }
              }
          }
      }

      public static List<Rectangle> LoadAnimation(int frameCount, int frameWidth, int frameHeight, int startX, int startY){
          List<Rectangle> listFrames = [];
          for (int i = 0; i < frameCount; i++) {
              listFrames.Add(new Rectangle(startX + i*frameWidth,startY,frameWidth,frameHeight));
          }
          return listFrames;
      } 
      public void Reset()
      {
          _currentFrameIndex = 0;
          _elapsedTime = 0f;
      }
  }

