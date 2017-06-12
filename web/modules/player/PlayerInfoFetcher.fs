namespace AntichessCheatDetection.Modules.Player

open AntichessCheatDetection.Modules.Reference

module PlayerInfoFetcher =
    let filterSpeed speed (gameList : Reference list) =
        match speed with
        | "all" -> gameList
        | _ -> List.filter (fun x -> x.Speed = speed) gameList

    let filterRating minR maxR (gameList : Reference list) =
        List.filter (fun (x : Reference) -> minR <= x.Rating && maxR >= x.Rating) gameList
    
    let takeEngineValues engine gameList =
        match engine with
        | "sjeng" -> List.map (fun (x : Reference) -> x.Sjeng) gameList
        | "stockfish" -> List.map (fun (x : Reference) -> x.Stockfish) gameList
        | "max" -> List.map (fun (x : Reference) -> x.Max) gameList
        | _ -> raise(System.Exception())
    
    let roundToClosestStepMultiple step gameList = 
        List.map (fun x -> floor(x / step) * step) gameList