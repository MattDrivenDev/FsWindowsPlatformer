namespace FsWindowsPlatformer

open System
open System.IO
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

type Tile = 
    | Empty
    | Full

module GameMap =

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

    let mapW = 20
    let mapH = 11
    let tileW = 64
    let tileH = 64
    let gravity = 0.2    
    
    let split (s:string) = s.Split([|Environment.NewLine|], StringSplitOptions.RemoveEmptyEntries)  

    let inRange (x,y) = x >= 0 && x < mapW && y >= 0 && y < mapH

    let tileAt (tiles:Tile[,]) (pos:Vector2) = 
        let x, y = int pos.X / tileW, int pos.Y / tileH
        if inRange(x, y) then Some(tiles.[x,y]) else None

    let tile x = 
        match x with
        | "1" -> Full
        | "0" -> Empty
        | _ -> Empty   

    let parse =    
        split
        >> (fun lines x y -> lines.[y].[x].ToString())
        >> Array2D.init mapW mapH 
        >> Array2D.map tile 

    let empty = emptyMap |> parse

    let load (content:ContentManager) file =
        let maptext = File.ReadAllText(Path.Combine(content.RootDirectory, file))
        let texture = content.Load<Texture2D>("tilesheet")
        let map = parse maptext        
        map, texture

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
                | Empty -> new Rectangle(0, 0, tileW, tileH)
                | Full -> new Rectangle(tileW, 0, tileW, tileH)            
            position, source            
              
        spritebatch.Begin()
        tiles |> (Array2D.mapi positionAndSource >> Array2D.iter drawOne)
        spritebatch.End()