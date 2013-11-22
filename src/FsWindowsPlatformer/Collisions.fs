namespace FsWindowsPlatformer

open System
open Microsoft.Xna.Framework

module Collisions = 

    /// Clamps the hero's left/right movement to the bounds of the map.
    let clampToGameMapBoundaries hero = 
        let outofBounds = 
            hero.Position.X < float32 0 || hero.Position.X > float32 (GameMap.mapW * GameMap.tileW)

        if outofBounds then 
            { hero with Speed = new Vector2(float32 0, hero.Speed.Y) }
        else hero

    let checkCollisionOnX map x hero = 
        let collision = 
            seq { yield new Vector2(x, float32 (hero.CollisionRectangle.Top + 4))
                  yield new Vector2(x, float32 (hero.CollisionRectangle.Center.Y))
                  yield new Vector2(x, float32 (hero.CollisionRectangle.Bottom - 4)) }
            |> Seq.choose (GameMap.tileAt map)
            |> Seq.exists (function | Full -> true | _ -> false)

        if collision then
            { hero with
                Jumping = false
                Speed = new Vector2(float32 0 , hero.Speed.Y) }
        else hero 

    let checkCollisionOnY map hero y = 
        let collision = 
            seq { yield new Vector2(float32 hero.CollisionRectangle.Left + float32 4, y)
                  yield new Vector2(float32 hero.CollisionRectangle.Right - float32 4, y)
                  yield new Vector2(float32 hero.CollisionRectangle.Center.X, y) }
            |> Seq.choose (GameMap.tileAt map)
            |> Seq.exists (function | Full -> true | _ -> false)

        if collision then
            { hero with
                Jumping = false
                Speed = new Vector2(hero.Speed.X, float32 0) }
        else hero 

    let checkWalls map hero = 
        let left = float32 hero.CollisionRectangle.Left + hero.Speed.X
        let right = float32 hero.CollisionRectangle.Right + hero.Speed.X
        hero |> checkCollisionOnX map left |> checkCollisionOnX map right

    /// Clamps the hero's downwards movement to avoid falling through floors.
    let checkFloor map hero = 
        float32 hero.CollisionRectangle.Bottom + hero.Speed.Y
        |> checkCollisionOnY map hero        
            
    /// Clamps the hero's upwards movement to avoid jumping through ceilings
    let checkCeiling map hero =
        float32 hero.CollisionRectangle.Top + hero.Speed.Y
        |> checkCollisionOnY map hero