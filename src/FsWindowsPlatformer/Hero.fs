namespace FsWindowsPlatformer

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

/// The hero can face one of two directions only.
type Direction = 
    | Left
    | Right

/// The state of the hero that will be remembered between updates/frames.
type HeroState = {
    Jumping : bool
    Direction : Direction
    Position : Vector2
    Speed : Vector2
    CollisionRectangle : Rectangle
    CurrentFrame : int
    CurrentFrametime : float32 }

/// Module for working with the hero.
module Hero = 
        
    let frameW, frameH = 64, 64
    let walkFrames = 9
    let firstWalkFrame = 1
    let idleFrame = 0
    let jumpFrame = 10
    let frameTime = float32 100
    let acceleration = float32 0.5
    let inertia = float32 0.1
    let jumpSpeed = float32 8.0
    let gravity = float32 0.2

    /// Loads the hero texture from the content pipeline.
    let loadTexture (content:ContentManager) =
        content.Load<Texture2D>("hero")

    /// Appies jumping movement to the hero's speed.
    let jump hero =
        if not hero.Jumping then
            { hero with
                Jumping = true
                Speed = new Vector2(hero.Speed.X, hero.Speed.Y - jumpSpeed) }
        else hero

    /// Applies directional movement to the hero's speed.
    let move direction hero =
        let diff = 
            acceleration * (match direction with | Left -> float32 -1 | Right -> float32 1)             
        { hero with
            Direction = direction
            Speed = new Vector2(hero.Speed.X + diff, hero.Speed.Y) }

    /// Runs a complete state-update on the hero.
    let update (gametime:GameTime) collisions =

        /// Updates the hero's collision rectangle based on the current position.
        let updateCollisionRectangle hero = 
            let rect = 
                new Rectangle(int hero.Position.X - (hero.CollisionRectangle.Width / 2),
                              int hero.Position.Y - hero.CollisionRectangle.Height,
                              frameW-20, frameH-10)
            { hero with CollisionRectangle = rect }

        /// Applies inertia to the hero's left/right movement.
        let applyInertia hero = 
            { hero with 
                Speed = new Vector2(MathHelper.Lerp(hero.Speed.X, float32 0.0, inertia)) }

        /// ???
        let clampMove hero = 
            { hero with Speed = if hero.Speed.X > float32 -0.1 && hero.Speed.X < float32 0.1
                                then new Vector2(float32 0, hero.Speed.Y)
                                else hero.Speed }
    
        /// Always apply gravity to the hero (so that he falls).
        let applyGravity hero =
            { hero with Speed = new Vector2(hero.Speed.X, hero.Speed.Y + gravity) }            

        /// Applies the hero's speed to their position to create movement.
        let applyMovement hero = 
            { hero with Position = hero.Position + hero.Speed }            

        /// Apply all the collision detection functions to the hero.
        let applyCollisions = collisions |> Seq.reduce (>>)

        // Compose it all together and wait for a hero!
        updateCollisionRectangle
        >> applyInertia
        >> clampMove
        >> applyGravity
        >> applyCollisions
        >> applyMovement

    /// Spawns a hero at a given position.
    let spawn position =         
        { Jumping = false
          Direction = Left 
          Position = position
          Speed = Vector2.Zero 
          CollisionRectangle = new Rectangle(int position.X, int position.Y, frameW-20, frameH-10)
          CurrentFrame = 0
          CurrentFrametime = float32 0.0 }

    /// Draws a hero to a spritebatch.
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