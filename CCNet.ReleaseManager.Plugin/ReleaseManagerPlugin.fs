namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ReleaseManager

open Exortech.NetReflector
open ThoughtWorks.CruiseControl.WebDashboard.Dashboard

[<ReflectorType("ReleaseManagerPlugin")>]
type ReleaseManagerPlugin (actionInstantiator: IActionInstantiator) =
    let dashboardAction = actionInstantiator.InstantiateAction(typeof<ReleaseManagerDashboardAction>)
    interface IPlugin with
        override val LinkDescription = "Release Manager" with get
        override val NamedActions = [|
                ImmutableNamedActionWithoutSiteTemplate(ReleaseManagerDashboardAction.ActionName, dashboardAction) :> INamedAction
            |] with get
