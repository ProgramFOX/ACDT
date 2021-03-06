namespace AntichessCheatDetection.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Options

open System.Collections.Generic
open System.Net.Http

open AntichessCheatDetection.Modules.Configuration

open Newtonsoft.Json
open Newtonsoft.Json.Linq

type AuthController(settings : IOptions<Settings>) =
    inherit Controller()
    let extractedSettings = settings.Value

    static member httpClient = new HttpClient()

    [<Route("/Auth/Login")>]
    member this.Login(code : string, error : string, state : string) =
        this.ViewData.["error"] <- error
        match System.String.IsNullOrWhiteSpace(code) with
        | true -> this.View() :> IActionResult
        | false ->
            let oauthUrl = System.String.Format("https://slack.com/api/oauth.access?client_id={0}&client_secret={1}&code={2}", extractedSettings.ClientID, extractedSettings.ClientSecret, code)
            let authResult = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(AuthController.httpClient.GetAsync(oauthUrl).Result.Content.ReadAsStringAsync().Result)
            match authResult.["ok"].ToObject<bool>() with
            | false -> this.RedirectToAction("Login", "Auth") :> IActionResult
            | true ->
                match authResult.["team"].["id"].ToObject<string>() + "," + authResult.["team"].["domain"].ToObject<string>() with
                | "T0HL8V171,lichess" ->
                    this.HttpContext.Session.SetInt32("auth", 1)
                    this.RedirectToAction("Index", "Home") :> IActionResult
                | _ ->
                    this.ViewData.["error"] <- "Invalid team."
                    this.View() :> IActionResult