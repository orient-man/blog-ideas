# Nice features

* fast & slow builds
* NuGet packages generation
* calling PowerShell (tokenization example)
* TeamCity integration: build number, test results, dotCover
* TeamCity is dumb ie. only calls build.cmd (["any CI doing anything other than calling scripts in your project's repo sucks"](https://twitter.com/ashic/status/795956927993958401))
* running NUnit tests (both unit & integration tests, handling categories)
* failed test raported properly in TeamCity
* FSharpLint
* disabling PostBuildEvent
* XCopyHelper.XCopy for additional dependencies

# Usage

Compilation + unit tests:

    build.cmd

Compilation + F# Lint + integration tests + NuGet packages:

    build.cmd Deploy SlowBuild=true

Other examples:

    build.cmd Deploy SkipTests=true