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