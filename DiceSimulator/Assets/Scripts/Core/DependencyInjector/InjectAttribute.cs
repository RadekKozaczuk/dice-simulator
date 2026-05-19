using JetBrains.Annotations;
using System;

namespace Core.DependencyInjector
{
    /// <summary>
    /// Indicates that this field will be populated by <see cref="DependencyInjector{TScriptableObject}"/>.
    /// The field should be always private, static, and readonly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class InjectAttribute : Attribute { }
}