namespace AntichessCheatDetection.Modules.Player

open AntichessCheatDetection.Modules.Reference

module PlayerInfoFetcher =
    let filterSpeed speed (gameList : Reference list) =
        match speed with
        | "all" -> gameList
        | _ -> List.filter (fun x -> x.Speed = speed) gameList

    let filterRating minR maxR (gameList : Reference list) =
        List.filter (fun (x : Reference) -> minR <= x.Rating && maxR >= x.Rating) gameList