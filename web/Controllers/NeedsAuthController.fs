namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters

type NeedsAuthController() =
    inherit Controller()

    override this.OnActionExecuting (context: ActionExecutingContext) =
        match context.HttpContext.Session.GetInt32("auth").HasValue with
        | false -> 
            context.Result <- this.RedirectToAction("Login", "Auth") :> IActionResult
        | _ ->
            let authed = context.HttpContext.Session.GetInt32("auth").Value
            match authed with
            | 1 -> base.OnActionExecuting context
            | _ -> context.Result <- this.RedirectToAction("Login", "Auth") :> IActionResult