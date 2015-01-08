namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ReleaseManager

open Exortech.NetReflector
open ThoughtWorks.CruiseControl.WebDashboard.Dashboard

[<ReflectorType("ReleaseManagerPlugin")>]
type ReleaseManagerPlugin (actionInstantiator: IActionInstantiator) =
    interface IPlugin with
        override val LinkDescription = "Release Manager" with get
        override val NamedActions = [| ImmutableNamedAction("HelloWorld", actionInstantiator.InstantiateAction(typeof<HelloWorldAction>)) :> INamedAction |] with get
