namespace FsWindowsPlatformer

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

type Direction = 
    | Left
    | Right

type Action = 
    | Idle 
    | Walking
    | Jumping

type HeroState = {
    Direction : Direction
    Position : Vector2
    Speed : Vector2
    CollisionRectangle : Rectangle
    CurrentFrame : int
    CurrentFrametime : float32 }

module Hero = 
        
    let frameW, frameH = 64, 64
    let walkFrames = 9
    let firstWalkFrame = 1
    let idleFrame = 0
    let jumpFrame = 10
    let frameTime = float32 100
    let acceleration = 0.5
    let inertia = 0.1
    let jumpSpeed = 8.0

    let loadTexture (content:ContentManager) =
        content.Load<Texture2D>("hero")

    let spawn position =         
        { Direction = Left 
          Position = position
          Speed = Vector2.Zero 
          CollisionRectangle = new Rectangle(int position.X, int position.Y, frameW-20, frameH-10)
          CurrentFrame = 0
          CurrentFrametime = float32 0.0 }

    let draw (spritebatch:SpriteBatch) (texture:Texture2D) hero = 
        spritebatch.Begin()
        spritebatch.Draw(
            texture=texture,
            position=hero.Position,
            sourceRectangle=Nullable(new Rectangle(frameW * hero.CurrentFrame, 0, frameW, frameH)),
            color=Color.White,
            rotation=float32 0,
            origin=new Vector2(float32 (frameW / 2), float32 frameH),
            scale=float32 1,
            effect=(match hero.Direction with | Left -> SpriteEffects.None | Right -> SpriteEffects.FlipHorizontally),
            depth=float32 0)
        spritebatch.End()