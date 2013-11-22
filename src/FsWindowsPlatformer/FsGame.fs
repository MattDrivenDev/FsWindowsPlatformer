namespace FsWindowsPlatformer

open System
open Microsoft.Xna.Framework

/// Small module for working with Vector2
module V2 = 

    /// Creates a new Vector2 instance    
    let create x y = new Vector2(x, y)
    
    /// Adds two Vector2 instances together
    let add (vector:Vector2) (vector':Vector2) = vector + vector'

    /// Adds a specified value to a Vector2.Y
    let addY y = create (float32 0) y |> add

    /// Adds a specified value to a Vector2.X
    let addX x = create x (float32 0) |> add

    /// Adds specified values to both Vector2.X and Vector2.Y
    let addXY x y = create x y |> add  