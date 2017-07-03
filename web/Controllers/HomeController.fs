namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

type HomeController() =
    inherit NeedsAuthController()

    [<Route("/")>]
    member this.Index() =
        this.View()