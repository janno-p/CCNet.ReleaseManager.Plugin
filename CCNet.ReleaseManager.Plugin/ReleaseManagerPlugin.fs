namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ReleaseManager

open Exortech.NetReflector
open System.Collections
open ThoughtWorks.CruiseControl.WebDashboard.Dashboard
open ThoughtWorks.CruiseControl.WebDashboard.MVC
open ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
open ThoughtWorks.CruiseControl.WebDashboard.MVC.View

[<ReflectorType("ReleaseManagerPlugin")>]
type ReleaseManagerPlugin (actionInstantiator: IActionInstantiator, viewGenerator: IVelocityViewGenerator) =
    interface IPlugin with
        override val LinkDescription = "Release Manager" with get
        override val NamedActions = [|
                ImmutableNamedAction(ReleaseManagerDashboardAction.ActionName,
                                     actionInstantiator.InstantiateAction(typeof<ReleaseManagerDashboardAction>)) :> INamedAction
            |] with get
