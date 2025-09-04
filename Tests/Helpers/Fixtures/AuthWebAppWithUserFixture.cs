using Xunit;

namespace Tests.Helpers.Fixtures
{
    public class AuthWebAppWithUserFixture : WebAppFixtureBase
    {
        public AuthWebAppWithUserFixture()
        {
            BasicDataSeed.Run(this);
            UsersSeed.Run(this);
        }
    }

    public class ContractFixture : WebAppFixtureBase
    {
        public ContractFixture()
        {
            BasicDataSeed.Run(this);
            UsersSeed.Run(this);
            // ContractDataSeed.Run(this);
        }
    }

    [CollectionDefinition("AuthWebAppWithUserCollection", DisableParallelization = true)]
    public class AuthWebAppWithUserCollectionDefinition : ICollectionFixture<AuthWebAppWithUserFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("AuthWebAppWithUserCollection")]
    public abstract class AuthWebAppWithUserCollection
    {
        protected WebAppFixtureBase Fixture { get; }

        protected AuthWebAppWithUserCollection(WebAppFixtureBase fixture)
        {
            Fixture = fixture;
        }
    }

    [CollectionDefinition("AuthWebAppWithContractCollection", DisableParallelization = true)]
    public class AuthWebAppWithContractCollectionDefinition : ICollectionFixture<ContractFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("AuthWebAppWithContractCollection")]
    public abstract class AuthWebAppWithContractCollection
    {
        protected WebAppFixtureBase Fixture { get; }

        protected AuthWebAppWithContractCollection(WebAppFixtureBase fixture)
        {
            Fixture = fixture;
        }
    }
}