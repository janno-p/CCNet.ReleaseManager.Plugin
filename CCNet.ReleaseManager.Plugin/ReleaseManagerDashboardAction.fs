namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ReleaseManager

open System.Collections
open ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
open ThoughtWorks.CruiseControl.WebDashboard.MVC.View

type ReleaseManagerDashboardAction (viewGenerator: IVelocityViewGenerator) =
    static member val ActionName = "ReleaseManagerDashboard" with get
    interface ICruiseAction with
        override __.Execute(cruiseRequest) =
            upcast viewGenerator.GenerateView("ReleaseManagerDashboard.vm", Hashtable())
