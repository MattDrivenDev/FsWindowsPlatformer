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

    /// Clamps the hero's downwards movement to avoid falling through floors.
    let checkFloor map hero = 
        let y = float32 hero.CollisionRectangle.Bottom + hero.Speed.Y
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