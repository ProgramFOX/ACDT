namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

type GameController() =
    inherit Controller()

    [<Route("/Game/{lookup}/{id}")>]
    member this.GameLookup(lookup : string, id : string) =
        let lowerLookup = lookup.ToLower()
        match lowerLookup with
        | "reference" | "investigate" -> this.Content("not yet implemented")
        | _ -> this.Content("Invalid lookup type.")