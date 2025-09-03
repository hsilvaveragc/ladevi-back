using Xunit;

namespace Tests.Auth
{
    public class AuthWebAppWithUserFixture : WebAppFixture
    {
        public AuthWebAppWithUserFixture()
        {
            BasicDataSeed.Run(this);
            UsersSeed.Run(this);
        }
    }

    public class ContractFixture : WebAppFixture
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
        protected WebAppFixture Fixture { get; }

        protected AuthWebAppWithUserCollection(WebAppFixture fixture)
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
        protected WebAppFixture Fixture { get; }

        protected AuthWebAppWithContractCollection(WebAppFixture fixture)
        {
            Fixture = fixture;
        }
    }
}