namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ReleaseManager

open System.Collections
open System.Collections.Generic
open System.Xml
open ThoughtWorks.CruiseControl.WebDashboard.Configuration
open ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
open ThoughtWorks.CruiseControl.WebDashboard.MVC.View

type ProjectDetails () =
    member val Name = "" with get, set

type ReleaseManagerDashboardAction (viewGenerator: IVelocityViewGenerator) =
    let configPath = DashboardConfigurationLoader.CalculateDashboardConfigPath()

    let loadConfig () =
        let document = XmlDocument()
        document.Load(configPath)
        document

    let saveConfig (document: XmlDocument) =
        document.Save(configPath)

    let getChildElements name (node: XmlNode) =
        [for n in node.ChildNodes -> n]
        |> List.choose (fun n -> match n.NodeType, n.LocalName with
                                 | XmlNodeType.Element, nm when nm = name -> Some n
                                 | _ -> None)

    let getAttribute name (node: XmlNode) =
        match [for a in node.Attributes -> a] |> List.tryFind (fun a -> a.LocalName = name) with
        | Some attr -> Some attr.Value
        | _ -> None

    static member val ActionName = "ReleaseManagerDashboard" with get

    interface ICruiseAction with
        override __.Execute(cruiseRequest) =
            let velocityContext = Hashtable()

            let doc = loadConfig()
            let node = doc.SelectSingleNode(@"/dashboard/plugins/farmPlugins/ReleaseManagerPlugin")

            velocityContext.["projects"] <-
                match node |> getChildElements "Projects" with
                | [ projects ] ->
                    projects
                    |> getChildElements "Project"
                    |> List.fold (fun (acc: List<_>) project ->
                        let projectName = project |> getAttribute "name"
                        if projectName.IsSome then
                            acc.Add(ProjectDetails(Name=projectName.Value))
                        acc) (List<_>())
                | _ -> (List<_>())

            //upcast viewGenerator.GenerateView("ReleaseManagerDashboard.vm", velocityContext)
            upcast viewGenerator.GenerateView("ReleaseManagerSiteTemplate.vm", velocityContext)
