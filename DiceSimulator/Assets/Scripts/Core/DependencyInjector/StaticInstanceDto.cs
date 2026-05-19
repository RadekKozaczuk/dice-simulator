using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.DependencyInjector
{
    class StaticInstanceDto
    {
        internal readonly Type Type;
        internal readonly object Instance;

        internal readonly List<FieldInfo> DynamicDependencies = new();

        internal StaticInstanceDto(Type type, object instance)
        {
            Type = type;
            Instance = instance;
        }
    }
}
