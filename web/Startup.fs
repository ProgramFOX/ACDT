namespace AntichessCheatDetection

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging


open AntichessCheatDetection.Modules.Configuration
open AntichessCheatDetection.Modules.Reference
open AntichessCheatDetection.Modules.Investigate
open AntichessCheatDetection.Modules.Api


type Startup private () =
    new(env: IHostingEnvironment) as this =
        Startup() then

        let builder =
            ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("config.json")
        this.Configuration <- builder.Build()

    member this.ConfigureServices(services: IServiceCollection) =
        services.AddMvc() |> ignore

        services.AddOptions() |> ignore
        if isNull(this.Configuration) then
            raise(System.Exception("Warning: Configuration is null."))

        services.Configure<Settings>(this.Configuration) |> ignore

        services.AddSingleton<IReferenceDbRepo, ReferenceDbRepo>() |> ignore
        services.AddSingleton<IInvestigateDbRepo, InvestigateDbRepo>() |> ignore
        services.AddSingleton<IQueueDbRepo, QueueDbRepo>() |> ignore
        services.AddSingleton<IApiKeyDbRepo, ApiKeyDbRepo>() |> ignore

    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment, loggerFactory: ILoggerFactory) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
            app.UseBrowserLink() |> ignore

        app.UseStaticFiles() |> ignore

        app.UseMvc() |> ignore
    
    member val Configuration : IConfigurationRoot = null with get, set
