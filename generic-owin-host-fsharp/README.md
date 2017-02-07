# Generic OWIN host in F#

Features:

* WebApi + SignalR
* static files (your SPA application)
* windows authentication
* basic authorization
* basic DI for WebApi controllers

Usage:

    new OwinHost(
        "https://+:8086/",   // "http://localhost:8086/"
        "./www/",
        createController,
        isAuthorized);

    // poor man's DI defined in CompositionRoot
    let createController = function
        | serviceType when serviceType = typeof<MyApiController> ->
            new MyApiController(...) :> obj
        | _ -> null

    let isAuthorized user = true