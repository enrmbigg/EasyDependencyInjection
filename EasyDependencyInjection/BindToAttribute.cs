using System;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceBase.DependencyInjection
{
    public sealed class BindToAttribute : Attribute
    {
        private const string BindingError = @"Object of type '{0}' can not be bound for dependency injection as it is not an interface. ";

        internal List<Type> InterfaceTypes { get; }
        public Scope Scope { get; set; }

        public BindToAttribute(Scope scope = Scope.Transient, params Type[] targetInterfaces)
        {
            foreach(var interfaceType in targetInterfaces)
            {
                if (!interfaceType.IsInterface)
                {
                    throw new InvalidOperationException(string.Format(BindingError, interfaceType.Name));
                }
            }

            InterfaceTypes = targetInterfaces.ToList();
            Scope = scope;
        }

    }
}
