namespace App.WebHost

open System
open System.Web.Http.Controllers
open System.Web.Http.Dispatcher

// hook for our composition code
type BasicHttpControllerActivator(createController: Type -> obj) =
    interface IHttpControllerActivator with
        member __.Create(_, _, controllerType) = controllerType |> createController :?> IHttpController