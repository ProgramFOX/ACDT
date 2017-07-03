namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc

type AuthController() =
    inherit Controller()

    [<Route("/Auth/Login")>]
    member this.Login() =
        this.View()