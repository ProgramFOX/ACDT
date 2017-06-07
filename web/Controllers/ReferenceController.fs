namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc
open AntichessCheatDetection.Modules.Reference

type ReferenceController(referenceDbRepo : IReferenceDbRepo) =
    inherit Controller()

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
        match cheatOrLegit with
        | "cheat" | "legit" ->
            match engine with
            | "sjeng" | "stockfish" | "max" ->
                match speed with
                | "bullet" | "blitz" | "classical" | "all" ->
                    match step with
                    | 5 ->
                        referenceDbRepo.GetDistribution cheatOrLegit engine step minRating maxRating speed |>
                        List.map (fun x -> x.ToString()) |>
                        String.concat "," |>
                        this.Content
                    | _ -> this.Content("")
                | _ -> this.Content("")
            | _ -> this.Content("")
        | _ -> this.Content("")