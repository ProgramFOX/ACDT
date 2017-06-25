namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters

open AntichessCheatDetection.Modules.Api
open AntichessCheatDetection.Modules.Investigate

type ApiController(apiKeyDbRepo: IApiKeyDbRepo, queueDbRepo: IQueueDbRepo) =
    inherit Controller()

    override this.OnActionExecuting (context : ActionExecutingContext) =
        match context.HttpContext.Request.Method.ToUpperInvariant() with
        | "POST" ->
            let ok, apiKeyO = context.ActionArguments.TryGetValue("key")
            let apiKey = string apiKeyO
            match ok && apiKeyDbRepo.IsValidKey(apiKey) with
            | true ->
                base.OnActionExecuting(context)
            | _ ->
                context.Result <- this.BadRequest() :> IActionResult
        | _ ->
            context.Result <- this.BadRequest() :> IActionResult
    
    [<Route("/Api/QueueNext")>]
    member this.Queue(key: string) =
        let nextQueueItem = queueDbRepo.GetNextQueueItem()
        this.Content(if isNull nextQueueItem then "" else nextQueueItem.Name)