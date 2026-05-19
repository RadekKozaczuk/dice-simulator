using System.Reflection;

namespace Core.DependencyInjector
{
    class DynamicInstanceDto
    {
        /// <summary>
        /// Constructor that creates the instance.
        /// </summary>
        internal readonly ConstructorInfo Constructor;

        /// <summary>
        /// Parameters need to construct the object by using the <see cref="Constructor"/>.
        /// </summary>
        internal readonly ParameterInfo[] Parameters;

        internal object Instance;

        internal DynamicInstanceDto(ConstructorInfo constructor, ParameterInfo[] parameters)
        {
            Constructor = constructor;
            Parameters = parameters;
        }
    }
}
