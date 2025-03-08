using System.Reflection;
using UetdsProgramiNet.Repositories;
using UetdsProgramiNet.Services;
using UetdsProgramiNet;
using Module = Autofac.Module;
using Autofac;

namespace Web.Modules
{
    public class RepoServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GenericRepo<>)).As(typeof(IGenericRepo<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Service<>)).As(typeof(IService<>)).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            var apiAssembly = Assembly.GetExecutingAssembly();
            var repoAssembly = Assembly.GetAssembly(typeof(AppDbContext));
            //var serviceAssembly = Assembly.GetAssembly(typeof(MapProfile));

            //InstancePerLifetimeScope = Scope
            //InstancePerDependency = Transient
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly).Where(x => x.Name.EndsWith("Repo")).AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
