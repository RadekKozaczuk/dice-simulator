#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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