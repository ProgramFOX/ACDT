namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

open AntichessCheatDetection.Modules.Reference

type PlayerController(referenceDbRepo: IReferenceDbRepo) =
    inherit Controller()

    [<Route("/Player/{lookup}/{name}")>]
    member this.GetPlayer(lookup : string, name : string) =
        let lowerLookup = lookup.ToLower()
        let lowerName = name.ToLower()
        match lowerLookup with
        | "reference" -> this.Content(referenceDbRepo.GetGamesByPlayer(lowerName).Count.ToString())
        | "investiage" -> this.Content("not yet implemented")
        | _ -> this.Content("Invalid lookup type.")