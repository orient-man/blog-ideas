module App.WebHost.WindowsAuthentication

open System.Net

type Owin.IAppBuilder with
    member app.UseIntegratedWindowsAuthentication() =
        let listener = app.Properties.["System.Net.HttpListener"] :?> HttpListener
        listener.AuthenticationSchemes <- AuthenticationSchemes.IntegratedWindowsAuthentication
        app