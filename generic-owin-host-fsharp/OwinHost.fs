namespace App.WebHost

open System
open System.IO
open System.Reflection
open log4net

type OwinHost(url, rootWww, createController, isAuthorized) =
    static let log = LogManager.GetLogger(typeof<OwinHost>)

    let wwwRoot =
        Assembly.GetExecutingAssembly().Location
        |> Path.GetDirectoryName
        |> fun dir -> Path.Combine(dir, rootWww)

    do
        "Starting OWIN host..." |> log.Info
        try Server(wwwRoot, url, createController, isAuthorized) |> ignore
        with ex -> log.Info("Error initializing OWIN server", ex); reraise()

    interface IDisposable with
        member __.Dispose() = "Stopping OWIN host..." |> log.Info