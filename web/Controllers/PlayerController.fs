namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

open AntichessCheatDetection.Modules.Reference
open AntichessCheatDetection.Modules.Player

type PlayerController(referenceDbRepo: IReferenceDbRepo) =
    inherit Controller()

    [<Route("/Player/{lookup}/{name}")>]
    member this.GetPlayer(lookup : string, name : string, speed : string, minRating : int, maxRating: int, engine : string, step : int) =
        let lowerLookup = lookup.ToLower()
        let lowerName = name.ToLower()
        let stepF = float step
        match lowerLookup with
        | "reference" ->
            referenceDbRepo.GetGamesByPlayer(lowerName) |>
            PlayerInfoProcessor.getPlayerDistribution speed minRating maxRating engine stepF |>
            Seq.map string |>
            String.concat "," |>
            this.Content
        | "investiage" -> this.Content("not yet implemented")
        | _ -> this.Content("Invalid lookup type.")