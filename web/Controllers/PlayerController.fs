namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

type PlayerController() =
    inherit Controller()

    [<Route("/Player/{lookup}/{name}")>]
    member this.GetPlayer(lookup : string, name : string) =
        let lowerLookup = lookup.ToLower()
        match lowerLookup with
        | "reference" | "investigate" -> this.Content("not yet implemented")
        | _ -> this.Content("Invalid lookup type.")