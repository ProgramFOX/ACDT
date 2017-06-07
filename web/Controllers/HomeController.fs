namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

type HomeController() =
    inherit Controller()

    [<Route("/")>]
    member this.Index() =
        this.View()