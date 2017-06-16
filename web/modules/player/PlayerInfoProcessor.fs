namespace AntichessCheatDetection.Modules.Player

open AntichessCheatDetection.Modules.Reference
open AntichessCheatDetection.Modules.Investigate

open Microsoft.FSharp.Collections

module PlayerInfoProcessor =
    let getSpeedFromGame (game : obj) =
        match game with
        | :? Reference as r -> r.Speed
        | :? Investigation as i -> i.Speed
        | _ -> raise(System.Exception())
    
    let getRatingFromGame (game : obj) =
        match game with
        | :? Reference as r -> r.Rating
        | :? Investigation as i -> i.Rating
        | _ -> raise(System.Exception())
    let filterSpeed<'T> speed (gameList : seq<'T>) =
        match speed with
        | "all" -> gameList
        | _ -> Seq.filter (fun x -> getSpeedFromGame x = speed) gameList

    let filterRating<'T> minR maxR (gameList : seq<'T>) =
        Seq.filter (fun x -> minR <= getRatingFromGame x && maxR >= getRatingFromGame x) gameList