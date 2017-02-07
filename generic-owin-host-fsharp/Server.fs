namespace App.WebHost

open Owin
open System
open System.Web.Http
open System.Web.Http.Dispatcher
open Microsoft.Owin.FileSystems
open Microsoft.Owin.Hosting
open Microsoft.Owin.StaticFiles
open Newtonsoft.Json.Serialization
open WindowsAuthentication

type Server (wwwRoot: string, host: string, createController, isAuthorized) =
    let configureFileServer () =
        let fileServerOpt = FileServerOptions(FileSystem = PhysicalFileSystem(wwwRoot))
        fileServerOpt.StaticFileOptions.ServeUnknownFileTypes <- true
        fileServerOpt

    let configureWebApi () =
        let config = new HttpConfiguration()
        config.Services.Replace(
            typeof<IHttpControllerActivator>,
            BasicHttpControllerActivator(createController))
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <-
            CamelCasePropertyNamesContractResolver()
        config.MapHttpAttributeRoutes()
        config

    let authToStaticFiles =
        RequestAuth.authorizeToIndex isAuthorized "/unauthorized.html"

    let startup (app: IAppBuilder) =
        app
            .UseIntegratedWindowsAuthentication()
            .Use(handler = Func<_, _, _>(authToStaticFiles))
            .UseFileServer(configureFileServer())
            .MapSignalR()
            .UseWebApi(configureWebApi())
        |> ignore

    do
        WebApp.Start(host, startup) |> ignore
        printfn "WWW Server running on %s" host