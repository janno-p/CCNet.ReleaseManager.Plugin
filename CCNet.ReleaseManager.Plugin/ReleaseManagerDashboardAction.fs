namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ReleaseManager

open System.Collections
open System.Collections.Generic
open System.Xml
open ThoughtWorks.CruiseControl.WebDashboard.Configuration
open ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
open ThoughtWorks.CruiseControl.WebDashboard.MVC.View

type ProjectDetails () =
    member val Name = "" with get, set
    member val Id = "" with get, set
    member val WorkingDir = "" with get, set
    member val BranchDir = "" with get, set
    member val TagDir = "" with get, set
    member val MajorVersionNumber = 0us with get, set
    member val MinorVersionNumber = 0us with get, set

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

            let projects =
                match node |> getChildElements "Projects" with
                | [ projects ] ->
                    projects
                    |> getChildElements "Project"
                    |> List.fold (fun (acc: List<_>) project ->
                        let projectId = project |> getAttribute "id"
                        let details = ProjectDetails(Id=projectId.Value)
                        let projectName = project |> getAttribute "name"
                        if projectName.IsSome then
                            details.Name <- projectName.Value
                        acc.Add(details)
                        acc) (List<_>())
                | _ -> (List<_>())
            velocityContext.["projects"] <- projects

            let parseUShort str =
                match System.UInt16.TryParse str with
                | true, value -> value
                | _ -> 0us

            match cruiseRequest.Request.GetText("Project") with
            | null | "" -> ()
            | pid ->
                match projects |> Seq.tryFind (fun x -> x.Id = pid) with
                | Some p ->
                    let pNode = doc.SelectSingleNode(sprintf @"/dashboard/plugins/farmPlugins/ReleaseManagerPlugin/Projects/Project[@id='%s']" pid)
                    for cNode in pNode.ChildNodes do
                        if cNode.NodeType = XmlNodeType.Element then
                            match cNode.LocalName with
                            | "WorkingDir" -> p.WorkingDir <- cNode.Value
                            | "BranchDir" -> p.BranchDir <- cNode.Value
                            | "TagDir" -> p.TagDir <- cNode.Value
                            | "MajorVersionNumber" -> p.MajorVersionNumber <- parseUShort cNode.Value
                            | "MinorVersionNumber" -> p.MinorVersionNumber <- parseUShort cNode.Value
                            | _ -> ()
                    let localContext = Hashtable()
                    localContext.["activeProject"] <- p
                    let mainContent = viewGenerator.GenerateView("ReleaseManagerProjectSettings.vm", localContext)

                    velocityContext.["activeProject"] <- p
                    velocityContext.["mainContent"] <- mainContent.ResponseFragment
                | _ -> ()

            //upcast viewGenerator.GenerateView("ReleaseManagerDashboard.vm", velocityContext)
            upcast viewGenerator.GenerateView("ReleaseManagerSiteTemplate.vm", velocityContext)
