using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine.Assertions;

namespace Core.DependencyInjector
{
    /// <summary>
    /// This class scans through all assemblies and injects <see cref="InjectAttribute"/> fields and configs (ScriptableObjects).
    /// Assembly names are hardcoded: Boot, Core, DataOriented, GameLogic, Presentation, Shared, and UI.
    /// </summary>
    /// <typeparam name="TScriptableObject">Always ScriptableObject type</typeparam>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public static class DependencyInjector<TScriptableObject> where TScriptableObject : class
    {
        /// <summary>
        /// These instances are created once and last for the whole time.
        /// If the controller is also bound to an interface it will be present on that list twice.
        /// Key is the instance's type and value is the instance itself.
        /// </summary>
        static readonly List<StaticInstanceDto> _staticInstances = new();

        /// <summary>
        /// Key: type, Value: instance.
        /// In the first pass instances are null.
        /// Instances are constructed later.
        /// In the final pass instance will never be null.
        /// </summary>
        static readonly Dictionary<Type, DynamicInstanceDto> _dynamicInstances = new();

        [UsedImplicitly]
        public static void Inject(Func<Type, TScriptableObject> findConfig, List<string> assemblyNames)
        {
            var assemblies = new Assembly[assemblyNames.Count];
            for (int i = 0; i < assemblyNames.Count; i++)
                assemblies[i] = Assembly.Load(assemblyNames[i]);

            // this creates signal queues
            BindSignals(assemblies);

            // this injects configs and creates react method for services
            BindConfigsAndReactiveServices(assemblies, findConfig);

            // this instantiates all controllers/viewmodels that do not have parametrized constructors
            FirstPass(assemblies);

            // goes through all static instances and inject into fields that need other static instances
            // todo: when we are at GameLogicViewModel
            // todo: it has one field GameLogicMainController _mainController
            // todo: this field is then identified as static even tho it is not - it has a parameterized constructor
            SecondPass(assemblies);
        }

        public static void ResolveBindings()
        {
            // create instances
            foreach (KeyValuePair<Type, DynamicInstanceDto> kvp in _dynamicInstances)
            {
                ParameterInfo[] ctorParams = kvp.Value.Parameters;
                ConstructorInfo ctor = kvp.Value.Constructor;
                object[] paramValues = new object[ctorParams.Length];
                object instance = ctor.Invoke(paramValues);
                kvp.Value.Instance = instance;
            }

            // todo: something is wrong here
            // inject instances
            foreach (StaticInstanceDto staticInstance in _staticInstances)
                foreach (FieldInfo info in staticInstance.DynamicDependencies)
                    if (_dynamicInstances.TryGetValue(info.FieldType, out DynamicInstanceDto dynamicInstance))
                        // field is static therefore we pass null as the instance
                        info.SetValue(null, dynamicInstance.Instance);
        }

        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        /// Goes through 'Core' assembly, searches for <see cref="ISignal"/> and binds all the methods.
        /// </summary>
        static void BindSignals(Assembly[] assemblies)
        {
            Assembly core = null!;

            // find Core
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Assembly asm in assemblies)
                // ReSharper disable once InvertIf
                if (asm.GetName().Name == "Core")
                {
                    core = asm;
                    break;
                }

            foreach (Type type in core.GetTypes())
            {
                // ignore internal classes, enums
                if (type.IsEnum || type.IsNested)
                    continue;

                if (type is not { IsInterface: true, Name: "ISignal" })
                    continue;

                Services.SignalService.BindSignals(type.GetMethods());

                return;
            }

            throw new Exception("Impossible state - ISignal class not found.");
        }

        /// <summary>
        /// Go through all types and inject configs into all qualified fields.
        /// Additionally, for static controllers (services) - register it in SignalService.
        /// </summary>
        // todo: could be merged with FirstPass
        static void BindConfigsAndReactiveServices(Assembly[] assemblies, Func<Type, TScriptableObject> findConfig)
        {
            foreach (Assembly asm in assemblies)
                foreach (Type type in asm.GetTypes())
                {
                    // ignore internal classes, enums
                    if (type.IsEnum || type.IsNested)
                        continue;

                    // todo: there are some types created by the compiler f.e. "PrivateImplementationDetails" that we want to filter out here
                    // todo: I don't know how to do it and this method is kinda too generic
                    // todo: however it works well in our case because our convention assumes we add a namespace everywhere
                    if (type.Namespace == null)
                        continue;

                    foreach (FieldInfo field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly))
                    {
                        if (field.IsConst())
                            continue;

                        // is config
                        if (!field.FieldType.IsSubclassOf(typeof(TScriptableObject)) || field.FieldType.Name[^6..] != "Config")
                            continue;

                        TScriptableObject config = findConfig.Invoke(field.FieldType)
                            ?? throw new Exception($"No Config found for the field named: '{field.Name}', "
                                + $"of type: {field.FieldType}, located in: {type.Name}");

                        field.SetValue(type, config);
                    }

                    // Services are never abstract
                    if (type.IsAbstract)
                        continue;

                    // todo: in the future, make suffix "Service" a requirement
                    if (type.IsStatic()
                        && (type.Namespace.EndsWith("Services", StringComparison.Ordinal) || type.Name.EndsWith("Service", StringComparison.Ordinal)))
                        Services.SignalService.AddReactiveService(type);
                }
        }

        /// <summary>
        /// Creates static instances.
        /// Calls SignalService.AddReactiveInstantiatable(instance); on these instances.
        /// Adds them to Initializable list
        /// For dynamic instances only creates entries without the instance.
        /// </summary>
        static void FirstPass(Assembly[] assemblies)
        {
            var awaitingStaticInstances = new List<Type>();

            foreach (Assembly asm in assemblies)
                foreach (Type type in asm.GetTypes())
                {
                    if (IgnoreCheck(type))
                        continue;

                    bool isControllerOrViewModel = type.Namespace!.EndsWith("Controllers", StringComparison.Ordinal)
                        || type.Namespace.EndsWith("ViewModels", StringComparison.Ordinal)
                        || type.Name.EndsWith("Controller", StringComparison.Ordinal);

                    // ReSharper disable once MergeIntoPattern
                    bool isStatic = type.IsAbstract && type.IsSealed;

                    // statics should not be constructed
                    // ReSharper disable once MergeIntoPattern
                    if (isControllerOrViewModel)
                    {
                        // constructor zero
                        ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

                        if (constructors.Length == 0) // must be present
                            throw new Exception(
                                $"{type.Name} has no parameterless constructor. Please add one with the attribute [Preserve].");

                        ConstructorInfo constructor = constructors[0];
                        ParameterInfo[] ctorParams = constructor.GetParameters();

                        // check if constructor injection
                        if (ctorParams.Length > 0)
                            // we know this type is dynamic, but we do not have the instance yet
                            _dynamicInstances.Add(type, new DynamicInstanceDto(constructor, ctorParams));
                        else
                            awaitingStaticInstances.Add(type);
                    }
                    else if (isStatic)
                    {
                        // if we are dealing with a static then it handled in the previous calls (consider merging)
                    }
                }

            // iterate over injectable fields whose type is an interface
            foreach (Type type in awaitingStaticInstances)
            {
                // must be one
                ConstructorInfo constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];

                object instance = constructor.Invoke(new object[] { });
                StaticInstanceDto staticInstance = _staticInstances.Find(si => si.Type == type);

                if (staticInstance != null)
                    throw new ArgumentException("Binding the same element twice is not allowed.");

                _staticInstances.Add(new StaticInstanceDto(type, instance));
                Services.SignalService.AddReactiveInstantiatable(instance);
            }
        }

        static bool IgnoreCheck(Type type)
        {
            // ignore internal classes, enums
            if (type.IsEnum || type.IsNested || type.IsInterface)
                return true;

            // filter out types created by the compiler f.e. "PrivateImplementationDetails"
            if (type.Namespace == null)
                return true;

            // ignore attributes f.e. EmbeddedAttribute or NullableContextAttribute
            if (type.IsSubclassOf(typeof(Attribute)))
                return true;

            // ignore dtos
            if (type.Name.EndsWith("Dto", StringComparison.Ordinal))
                return true;

            bool isSo = type.IsSubclassOf(typeof(TScriptableObject));

            // ignore configs
            if (isSo && type.Name.EndsWith("Config", StringComparison.Ordinal))
                return true;

            // ignore data
            return isSo && type.Name.EndsWith("Data", StringComparison.Ordinal);
        }

        /// <summary>
        /// Inject static instances into fields that needs them.
        /// Other fields are ignored.
        /// </summary>
        static void SecondPass(Assembly[] assemblies)
        {
            Type injectAttribute = typeof(InjectAttribute);

            foreach (Assembly asm in assemblies)
                foreach (Type type in asm.GetTypes())
                {
                    // go through all fields
                    FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (int i = 0; i < fields.Length; i++)
                    {
                        // we go through all injectable fields
                        FieldInfo info = fields[i];

                        // is injectable
                        if (Attribute.GetCustomAttributes(info, injectAttribute, false).Length == 0)
                            continue;

                        Type fieldType = info.FieldType;
                        List<StaticInstanceDto> instances = _staticInstances.FindAll(si => si.Type == fieldType);

                        // zero: dynamic, 1: static, more than one: invalid state
                        Assert.IsTrue(instances.Count is 0 or 1, "Found more than one matching instances. Should be one.");

                        if (instances.Count == 0) // must be dynamic
                            AddDynamicDependency(type, info);
                        else
                            info.SetValue(type, instances[0].Instance);
                    }
                }
        }

        static void AddDynamicDependency(Type type, FieldInfo fieldInfo)
        {
            // the field is not a dynamic dependency
            if (!_dynamicInstances.TryGetValue(fieldInfo.FieldType, out DynamicInstanceDto _))
                throw new Exception("Invalid program state.");

            // check if the class is a dynamic dependency
            if (_dynamicInstances.TryGetValue(type, out DynamicInstanceDto _))
                return;

            // must be in static
            StaticInstanceDto staticInstance = _staticInstances.Find(si => si.Type == type);
            staticInstance.DynamicDependencies.Add(fieldInfo);
        }
    }
}