using WinPure.AddressVerification.DependencyInjection;
using WinPure.Cleansing.DependencyInjection;
using WinPure.Configuration.DependencyInjection;
using WinPure.Matching.DependencyInjection;

namespace WinPure.API.DependencyInjection
{
    internal class WinPureApiDependencyResolver : WinPureConfigurationDependency
    {
        public new static WinPureConfigurationDependency Instance => Resolver ?? (Resolver = new WinPureApiDependencyResolver());

        public override void RegisterDependencies()
        {
            base.RegisterDependencies();
            ServiceCollection.RegisterVerification();
            ServiceCollection.RegisterCleansing();
            ServiceCollection.RegisterMatching();
        }
    }
}