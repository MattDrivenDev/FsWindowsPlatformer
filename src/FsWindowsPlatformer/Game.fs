namespace FsWindowsPlatformer

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type FsWindowsPlatformerGame() as game = 
    inherit Game()

    let random = new Random()
    let graphics = new GraphicsDeviceManager(game)
    let mutable spritebatch = Unchecked.defaultof<_>

    let mutable gamemap = GameMap.empty, Unchecked.defaultof<_>  
    let mutable heroTexture = Unchecked.defaultof<_>
    let mutable hero = Hero.spawn(new Vector2(float32 -100, float32 -100))

    do 
        game.Content.RootDirectory <- "Content"
        graphics.PreferredBackBufferWidth <- 1280
        graphics.PreferredBackBufferHeight <- 720
        graphics.IsFullScreen <- false
        graphics.ApplyChanges()
        game.Initialize()

    override game.Initialize() = 
        base.Initialize()

    override game.LoadContent() = 
        spritebatch <- new SpriteBatch(game.GraphicsDevice)
        gamemap <- GameMap.load game.Content "map.txt"
        hero <- fst gamemap |> GameMap.getHeroSpawnPosition |> Hero.spawn
        heroTexture <- Hero.loadTexture game.Content

    override game.Update(gametime) = 
        ()

    override game.Draw(gametime) = 
        game.GraphicsDevice.Clear(Color.CornflowerBlue)

        // tie-fighter!
        GameMap.draw spritebatch (snd gamemap) (fst gamemap) 
        Hero.draw spritebatch heroTexture hero