namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

open AntichessCheatDetection.Modules.Investigate

type InvestigateController(investigateDbRepo: IInvestigateDbRepo, queueDbRepo: IQueueDbRepo) =
    inherit Controller()

    [<Route("/Investigate")>]
    member this.Index() =
        this.View(queueDbRepo.GetUnprocessedNames())
    
    [<Route("/Investigate/StartInvestigation")>]
    member this.StartInvestigation(player : string) =
        let playerNameLower = player.ToLowerInvariant()
        let validNameRegex = System.Text.RegularExpressions.Regex("^[a-z0-9_-]+$")
        match validNameRegex.IsMatch playerNameLower with
        | true ->
            if not (queueDbRepo.IsInQueueAndNotProcessed playerNameLower) then
                queueDbRepo.AddToQueue playerNameLower
            this.RedirectToAction("Index") :> IActionResult
        | _ -> this.Content("That is an invalid username.") :> IActionResult