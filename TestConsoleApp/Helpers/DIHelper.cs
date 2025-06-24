using Autofac;
using Common.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace TestConsoleApp.Helpers
{
    public class DIHelper
    {
        private readonly ILog _log;
        private ContainerBuilder _builder;

        public DIHelper(ContainerBuilder builder)
        {
            _log = LogManager.GetLogger(nameof(DIHelper));
            _builder = builder ?? new ContainerBuilder();
        }

        public DIHelper() : this(new ContainerBuilder())
        {
            _log = LogManager.GetLogger(nameof(DIHelper));
        }

        public DIHelper RegisterInterface<TInterface, TImplementation>(bool interceptCall = true)
            where TImplementation : class
        {
            try
            {
                _builder.RegisterType<TImplementation>().As<TInterface>();

                return this;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        public DIHelper RegisterInterfaceInstancePerRequest<TInterface, TImplementation>(bool interceptCall = true)
            where TImplementation : class
        {
            try
            {
                _builder.RegisterType<TImplementation>().As<TInterface>().InstancePerRequest();
                
                return this;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        public DIHelper RegisterAssemblyModules(string assembliesSearchPattern = "PL.*")
        {
            Assembly[] assemblies = AssemblyHelper.GetAssemblies(assembliesSearchPattern).ToArray();
            RegisterModules(assemblies);
            return this;
        }
        public DIHelper RegisterModules(Assembly[] assemblies)
        {
            _builder.RegisterAssemblyModules(assemblies);
            return this;
        }
        public DIHelper RegisterAssemblyInterfaces(string assembliesSearchPattern = "PL.*")
        {
            Assembly[] assemblies = AssemblyHelper.GetAssemblies(assembliesSearchPattern).ToArray();
            RegisterAllInterfaces(assemblies);
            return this;
        }
        public DIHelper RegisterAllInterfaces(Assembly[] assemblies)
        {
            try
            {
                
                _builder.RegisterAssemblyTypes(assemblies).AsImplementedInterfaces().PropertiesAutowired();
                return this;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }


        public IContainer Build(Func<ContainerBuilder, ContainerBuilder> customBuilder = null)
        {
            try
            {
                if (customBuilder != null)
                {
                    _builder = customBuilder.Invoke(_builder);
                }

                _builder.Register(c => LogManager.GetLogger<DIHelper>()).As<ILog>();
                var container = _builder.Build();

                _log.Trace("Dependencies successfully registered...");

                return container;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }
    }
}
