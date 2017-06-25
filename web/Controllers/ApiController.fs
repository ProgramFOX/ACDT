namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters

open AntichessCheatDetection.Modules.Api

type ApiController(apiKeyDbRepo: IApiKeyDbRepo) =
    inherit Controller()

    override this.OnActionExecuting (context : ActionExecutingContext) =
        match context.HttpContext.Request.Method.ToUpperInvariant() with
        | "POST" ->
            let ok, apiKeyO = context.RouteData.Values.TryGetValue("key")
            let apiKey = string apiKeyO
            match ok && apiKeyDbRepo.IsValidKey(apiKey) with
            | true ->
                base.OnActionExecuting(context)
            | _ ->
                context.Result <- this.BadRequest() :> IActionResult
        | _ ->
            context.Result <- this.BadRequest() :> IActionResult
    
    [<Route("/Api/Queue")>]
    member this.Queue() =
        this.Content("NYI")