# Generic OWIN host in F#

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

    let isAuthorized _ = true