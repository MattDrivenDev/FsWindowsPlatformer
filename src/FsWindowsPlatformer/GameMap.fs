namespace FsWindowsPlatformer

open System
open System.IO
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

/// Each tile of the game world will be represented as one of the following types.
type Tile = 
    | HeroSpawn
    | EnemySpawn
    | Empty
    | Full

/// Contains values and functions related to and for working with the game world.
module GameMap =

    /// Helper function to turn 2D arrays into a sequence for folds etc.
    let flatten (a: 'a[,]) = Seq.cast<'a> a
    
    /// Helper function to split a string by line breaks.
    let split (s:string) = s.Split([|Environment.NewLine|], StringSplitOptions.RemoveEmptyEntries)  

    /// Value of a completely empty map.
    let emptyMap = """
00000000000000000000
00000000000000000000
00000000000000000000
00000000000000000000
00000000000000000000
00000000000000000000
00000000000000000000
00000000000000000000
00000000000000000000
00000000000000000000
00000000000000000000
"""
    /// Width of the map measured in tiles.
    let mapW = 20

    /// Height of the map measured in tiles.
    let mapH = 11

    /// Width of a tile measured in pixels.
    let tileW = 64

    /// Height of a tile measured in pixels.
    let tileH = 64  

    /// Checks that a set of tile coords are within the accepted world size.
    let inRange (x,y) = x >= 0 && x < mapW && y >= 0 && y < mapH

    /// Will return Some tile when one exists at a specified position.
    let tileAt (tiles:Tile[,]) (pos:Vector2) = 
        let x, y = int pos.X / tileW, int pos.Y / tileH
        if inRange(x, y) then Some(tiles.[x,y]) else None

    /// Constructs a new tile from the string value recorded in the map.txt
    let tile x = 
        match x with
        | "1" -> Full
        | "0" -> Empty
        | "8" -> EnemySpawn
        | "9" -> HeroSpawn
        | _ -> Empty   

    /// Parses a string into a game map.
    let parse =    
        split
        >> (fun lines x y -> lines.[y].[x].ToString())
        >> Array2D.init mapW mapH 
        >> Array2D.map tile 

    /// A completely empty map of tiles.
    let empty = emptyMap |> parse

    /// Loads a new game map and associated texture from the content pipeline.
    let load (content:ContentManager) file =
        let maptext = File.ReadAllText(Path.Combine(content.RootDirectory, file))
        let texture = content.Load<Texture2D>("tilesheet")
        let map = parse maptext        
        map, texture

    /// Gets a position of the hero spawn point in a map.
    /// Will throw exception if no hero spawn marker is in map.
    let getHeroSpawnPosition =
        let coordsToPosition (x,y) = 
            ((new Vector2(float32 x, float32 y)) * (new Vector2(float32 tileW, float32 tileH)))
            + (new Vector2(float32 (Hero.frameW/2), float32 (Hero.frameH)))

        let heroSpawn (x,y,tile) = 
            match tile with
            | HeroSpawn -> Some(x,y)
            | EnemySpawn | Empty | Full -> None

        Array2D.mapi (fun x y tile -> x,y,tile)
        >> flatten
        >> Seq.pick heroSpawn
        >> coordsToPosition        

    /// Draws the map (background) to a spritebatch.
    let draw (spritebatch:SpriteBatch) (texture:Texture2D) tiles =
        let drawOne(position, source) =
            spritebatch.Draw(
                texture=texture,
                position=position,
                sourceRectangle=Nullable(source),
                color=Color.White)

        let positionAndSource x y tile =
            let position = 
                new Vector2(float32 x * float32 tileW, float32 y * float32 tileH)
            let source = 
                match tile with
                | Empty | HeroSpawn | EnemySpawn -> new Rectangle(0, 0, tileW, tileH)
                | Full -> new Rectangle(tileW, 0, tileW, tileH)            
            position, source            
              
        spritebatch.Begin()
        tiles |> (Array2D.mapi positionAndSource >> Array2D.iter drawOne)
        spritebatch.End()