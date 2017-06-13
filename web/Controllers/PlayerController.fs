namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

open AntichessCheatDetection.Modules.Reference
open AntichessCheatDetection.Modules.Player

type PlayerController(referenceDbRepo: IReferenceDbRepo) =
    inherit Controller()

    [<Route("/Player/{lookup}/{name}")>]
    member this.GetPlayer(lookup : string, name : string, speed : string, minRating : int, maxRating: int, engine : string, step : int) : IActionResult =
        let lowerLookup = lookup.ToLower()
        let lowerName = name.ToLower()
        let stepF = float step
        match lowerLookup with
        | "reference" ->
            referenceDbRepo.GetGamesByPlayer lowerName |>
            PlayerInfoProcessor.filterSpeed speed |>
            PlayerInfoProcessor.filterRating minRating maxRating |>
            this.Json :> IActionResult
        | "investigate" -> this.Content("not yet implemented") :> IActionResult
        | _ -> this.Content("Invalid lookup type.") :> IActionResult