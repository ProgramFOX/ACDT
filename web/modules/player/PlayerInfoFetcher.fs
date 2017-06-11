namespace AntichessCheatDetection.Modules.Player

open AntichessCheatDetection.Modules.Reference

module PlayerInfoFetcher =
    let filterSpeed speed (gameList : Reference list) =
        match speed with
        | "all" -> gameList
        | _ -> List.filter (fun x -> x.Speed = speed) gameList