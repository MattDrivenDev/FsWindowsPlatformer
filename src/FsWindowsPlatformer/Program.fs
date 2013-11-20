namespace FsWindowsPlatformer

module Program = 

    [<EntryPoint>]
    let main args = 
        let game = new FsWindowsPlatformerGame()
        game.Run()
        0