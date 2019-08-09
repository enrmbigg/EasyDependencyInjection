using Ninject;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace EcommerceBase.DependencyInjection
{
    public static class InjectionContainer
    {
        private static readonly Lazy<IKernel> _kernel = new Lazy<IKernel>(SetKernel, LazyThreadSafetyMode.ExecutionAndPublication);
        internal static IKernel Kernel => _kernel.Value;

        private static bool _isInitialised { get; set; }

        private static IKernel SetKernel()
        {
            INinjectSettings settings = new NinjectSettings
            {
                InjectNonPublic = true
            };

            return BindImplementations(new StandardKernel(settings));
        }

        private static IKernel BindImplementations(IKernel kernel)
        {
            BuildManager.GetReferencedAssemblies()
                .Cast<Assembly>()
                .ToList()
                .AsParallel()
                .ForAll(a => BindAssembly(kernel, a));

            _isInitialised = true;

            return kernel;
        }

        private static void BindAssembly(IKernel kernel, Assembly assembly)
        {
            var classes = assembly
                .GetTypes()
                .Where(t => t.GetCustomAttribute<BindToAttribute>() != null)
                .AsParallel();

            Parallel.ForEach(classes, (targetType) => BindClass(targetType, kernel));
        }

        private static void BindClass(Type targetType, IKernel kernel)
        {
            var binding = targetType.GetCustomAttribute<BindToAttribute>();

            foreach (var interfaceType in binding.InterfaceTypes)
            {
                if (kernel.CanResolve(interfaceType))
                {
                    continue;
                }

                switch (binding.Scope)
                {
                    case Scope.Transient:
                        kernel.Bind(interfaceType).To(targetType).InTransientScope();
                        break;

                    case Scope.Thread:
                        kernel.Bind(interfaceType).To(targetType).InThreadScope();
                        break;

                    case Scope.Singleton:
                        kernel.Bind(interfaceType).To(targetType).InSingletonScope();
                        break;

                    case Scope.Undefined:
                    default:
                        throw new NotSupportedException("Defined Scope is not supported");
                }
            }
        }
    

        public static void Start()
        {
        }

        public static T Resolve<T>()
        {
            return Kernel.Get<T>();
        }
    }
}