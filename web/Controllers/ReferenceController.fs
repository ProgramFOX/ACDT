namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc
open AntichessCheatDetection.Modules.Reference

type ReferenceController(referenceDbRepo : IReferenceDbRepo) =
    inherit Controller()

    let validCheatOrLegitInput input error =
        match error with
        | "" ->
            if input = "cheat" || input = "legit" then "" else "Invalid cheat/legit selector."
        | _ -> error
    
    let validEngineInput input error = 
        match error with
        | "" ->
            if input = "sjeng" || input = "stockfish" || input = "max" then "" else "Invalid engine."
        | _ -> error
    
    let validMinRatingInput input error =
        match error with
        | "" ->
            match input with
            | 0 | 2100 | 2200 | 2300 | 2400 | 2500 | 2600 | 2700 | 2800 | 2900 -> ""
            | _ -> "Invalid min rating."
        | _ -> error
    
    let validMaxRatingInput input error =
        match error with
        | "" ->
            match input with
            | 2100 | 2200 | 2300 | 2400 | 2500 | 2600 | 2700 | 2800 | 2900 | 3000 -> ""
            | _ -> "Invalid max rating."
        | _ -> error
    
    let minRatingLessThanMax min max error =
        match error with
        | "" -> if min < max then "" else "Max rating has to be greater than min rating."
        | _ -> error
    
    let validSpeedInput input error =
        match error with
        | "" ->
            match input with
            | "bullet" | "blitz" | "classical" | "all" -> ""
            | _ -> "Invalid speed."
        | _ -> error
    
    let validStepInput input error = 
        match error with
        | "" -> if input = 5 then "" else "Invalid step."
        | _ -> error
    

    [<Route("/Reference")>]
    member this.Index([<FromQuery>] cheatOrLegit : string, [<FromQuery>] engine : string) =
        this.View()
    
    [<Route("/Reference/Select")>]
    member this.Select([<FromQuery>] cheatOrLegit : string,
                       [<FromQuery>] engine : string,
                       [<FromQuery>] minRating : int,
                       [<FromQuery>] maxRating: int,
                       [<FromQuery>] speed: string,
                       [<FromQuery>] step : int) =
        let inputErrorHandling = validCheatOrLegitInput cheatOrLegit "" |>
                                 validEngineInput engine |>
                                 validMinRatingInput minRating |>
                                 validMaxRatingInput maxRating |>
                                 minRatingLessThanMax minRating maxRating |>
                                 validSpeedInput speed |>
                                 validStepInput step
        match inputErrorHandling with
        | "" ->
            referenceDbRepo.GetDistribution cheatOrLegit engine step minRating maxRating speed |>
            List.map (fun x -> x.ToString()) |>
            String.concat "," |>
            this.Content
        | _ -> inputErrorHandling |> this.Content