using Xunit;

namespace Tests.Auth
{
    public class AuthWebAppFixture : WebAppFixture
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
        protected WebAppFixture Fixture { get; }

        protected AuthWebAppCollection(WebAppFixture fixture)
        {
            Fixture = fixture;
        }
    }
}