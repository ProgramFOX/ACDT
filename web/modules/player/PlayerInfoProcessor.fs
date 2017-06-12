namespace AntichessCheatDetection.Modules.Player

open AntichessCheatDetection.Modules.Reference

open Microsoft.FSharp.Collections

module PlayerInfoProcessor =
    let filterSpeed speed (gameList : seq<Reference>) =
        match speed with
        | "all" -> gameList
        | _ -> Seq.filter (fun x -> x.Speed = speed) gameList

    let filterRating minR maxR (gameList : seq<Reference>) =
        Seq.filter (fun (x : Reference) -> minR <= x.Rating && maxR >= x.Rating) gameList
    
    let takeEngineValues engine gameList =
        match engine with
        | "sjeng" -> Seq.map (fun (x : Reference) -> x.Sjeng) gameList
        | "stockfish" -> Seq.map (fun (x : Reference) -> x.Stockfish) gameList
        | "max" -> Seq.map (fun (x : Reference) -> x.Max) gameList
        | _ -> raise(System.Exception())
    
    let roundToClosestStepMultiple step engineValues = 
        Seq.map (fun x -> floor(x / step) * step) engineValues
    
    let countByStep engineValues =
        Seq.countBy id engineValues
    
    let sortCountsByStep counts =
        Seq.sortBy fst counts
    
    let onlyGetCountValues counts =
        Seq.map snd counts
    
    let getPlayerDistribution speed minRating maxRating engine step gameList =
        gameList |> filterSpeed speed
                 |> filterRating minRating maxRating
                 |> takeEngineValues engine
                 |> roundToClosestStepMultiple step
                 |> countByStep
                 |> sortCountsByStep
                 |> onlyGetCountValues