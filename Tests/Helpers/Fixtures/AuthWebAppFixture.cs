using Xunit;

namespace Tests.Helpers.Fixtures
{
    public class AuthWebAppFixture : WebAppFixtureBase
    {
        public AuthWebAppFixture()
        {
            BasicDataSeed.Run(this);
        }
    }

    [CollectionDefinition("AuthWebAppCollection", DisableParallelization = true)]
    public class AuthWebAppCollectionDefinition : ICollectionFixture<AuthWebAppFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("AuthWebAppCollection")]
    public abstract class AuthWebAppCollection
    {
        protected WebAppFixtureBase Fixture { get; }

        protected AuthWebAppCollection(WebAppFixtureBase fixture)
        {
            Fixture = fixture;
        }
    }
}