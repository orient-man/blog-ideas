// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
#r "packages/FSharpLint.Fake/tools/FSharpLint.Core.dll"
#r "packages/FSharpLint.Fake/tools/FSharpLint.Fake.dll"
#r "System.Management.Automation"
open Fake
open Fake.DotCover
open FSharpLint.Fake
open System.Management.Automation

// Properties
let buildDir = "./build/"
let serviceBuildDir = buildDir + "service/"
let testDir = "./test/"
let deployDir = "./deploy/"

// for "fast" & "slow" builds
let excludedTestCategories =
    if hasBuildParam "SlowBuild"
    then "StressTest;FunctiontalTest"
    else "SlowTest;IntegrationTest;StressTest;FunctiontalTest"

// NuGet params
let NuGetWithDefaults projectName workingDir additionalFiles =
    NuGet (fun p ->
        { p with
            Project = projectName
            Description = projectName
            OutputPath = deployDir
            WorkingDir = workingDir
            Version = TeamCityHelper.TeamCityBuildNumber |> defaultArg <| "1.0.0.0"
            Files =
                List.append
                    [   ("*.dll", Some "lib\\net461", None)
                        ("*.exe", Some "lib\\net461", None)
                        ("..\\..\\projects\\HelloWorld.Service\\" + projectName + "\\config\\*.config", Some "configuration", None) ]
                    additionalFiles
        })
        "template.nuspec"

// example of calling PowerShell scripts
let tokenizeConfigFiles workingDir =
    let configProjDir = ".\\projects\\HelloWorld.Service\\HelloWorld.Service.Configuration"
    PowerShell.Create()
        .AddScript(
            "& '.\\packages\\BuildTools\\token.ps1' " +
            configProjDir + " " +
            workingDir + " " +
            configProjDir + "\\tokens.xml")
        .Invoke()
        |> Seq.iter (printfn "PowerShell: %O")

// Filesets
let serviceReferences = !! "projects/**/HelloWorld.ServiceHost.csproj"

let testReferences =
    !! "projects/**/*.Tests.fsproj"
    ++ "projects/**/*.Tests.csproj"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; deployDir]
)

Target "Lint" (fun _ ->
    !! "projects/**/*.fsproj"
    |> Seq.iter (FSharpLint (fun options -> { options with FailBuildIfAnyWarnings = true })))

Target "BuildService" (fun _ ->
    // XCopyHelper.XCopy "./packages/ServiceHost/lib/net46/WCFHost.exe" serviceBuildDir

    MSBuildReleaseExt serviceBuildDir [("PostBuildEvent", "")] "Build" serviceReferences
    |> Log "ServiceBuild-Output: "

    // tokenizeConfigFiles serviceBuildDir
)

Target "BuildTest" (fun _ ->
    let properties = [("Configuration", "Debug"); ("PostBuildEvent", "")]
    MSBuild testDir "Build" properties testReferences
    |> Log "TestBuild-Output: "
)

Target "RunTests" (fun _ ->
    !! (testDir + "/*.Tests.dll")
    |> NUnit (fun p ->
        { p with
            ExcludeCategory = excludedTestCategories
            DisableShadowCopy = true
            OutputFile = testDir + "TestResults.xml" })
)

Target "RunTestsWithCoverage" (fun _ ->
    let coverageOutput = testDir + "NUnitDotCover.snapshot"
    let nunitOutput = testDir + "TestResults.xml"

    !! (testDir + "/*.Tests.dll")
    |> DotCoverNUnit
        (fun dotCoverOptions ->
            { dotCoverOptions with
                ToolPath = "..\\..\\tools\\dotCover\\dotCover.exe"
                Filters = "+:HelloWorld.Service.*;-:*.Tests;-:HelloWorld.Service.TestHelper"
                Output = coverageOutput })
        (fun nUnitOptions ->
            { nUnitOptions with
                ExcludeCategory = excludedTestCategories
                DisableShadowCopy = true
                OutputFile = nunitOutput })

    (coverageOutput, TeamCityDotNetCoverageTool.DotCover)
    ||> TeamCityHelper.sendTeamCityDotNetCoverageImportForTool
    // Done already via Build Features see:
    // https://umbraco.com/follow-us/blog-archive/2013/1/16/make-teamcity-respect-failing-nunit-tests-and-stop-the-build/
    //nunitOutput |> TeamCityHelper.sendTeamCityNUnitImport
)

Target "DeployService" (fun _ ->
    // SPA application
    let files = [("..\\..\\projects\\HelloWorld.Service.Web\\www\\src\\**", Some "lib\\net461\\www", None)]
    NuGetWithDefaults "HelloWorld.ServiceHost" serviceBuildDir files
)

Target "Deploy" (fun () -> trace " --- Deploying app --- ")

// Build order
"Clean"
    =?> ("Lint", hasBuildParam "SlowBuild")
    ==> "BuildService"
    =?> ("BuildTest", not (hasBuildParam "SkipTests"))
    =?> ("RunTests", isLocalBuild && not (hasBuildParam "SkipTests"))
    =?> ("RunTestsWithCoverage", not isLocalBuild && not (hasBuildParam "SkipTests"))
    ==> "DeployService"
    ==> "Deploy"

// start build
RunTargetOrDefault "RunTests"