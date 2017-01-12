namespace IntegrationTests

open NUnit.Framework

[<SetUpFixture>]
type SetUpFixture() =
    [<SetUp>]
    member __.RunBeforeAnyTests() = log4net.Config.BasicConfigurator.Configure()

    [<TearDown>]
    member __.RunAfterAllTests() = Common.Database.DropIfExist()