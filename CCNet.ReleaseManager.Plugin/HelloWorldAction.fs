namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ReleaseManager

open ThoughtWorks.CruiseControl.WebDashboard.MVC
open ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise

type HelloWorldAction () =
    interface ICruiseAction with
        override __.Execute(cruiseRequest) =
            upcast HtmlFragmentResponse("<h1>Hello, World!</h1>")
