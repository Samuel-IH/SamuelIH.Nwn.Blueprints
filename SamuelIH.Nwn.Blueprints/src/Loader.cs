using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SamuelIH.Nwn.Blueprints
{
    public class Loader
    {
        public LoggingBridge Log { get; set; } = new LoggingBridge();

        private readonly IDeserializer _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        
        private readonly ISerializer _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        private readonly Dictionary<string, ItemBlueprint> _itemBlueprints = new Dictionary<string, ItemBlueprint>();
        private readonly Dictionary<string, ItemBlueprint> _rawItemBlueprints = new Dictionary<string, ItemBlueprint>();
        private readonly Dictionary<string, ProviderData> _providers = new Dictionary<string, ProviderData>();


        private void AssertProviderNamespaceValid(string ns)
        {
            if (_providers.ContainsKey(ns)) throw new Exception($"Namespace {ns} is already registered.");
            if (ns.Contains(":")) throw new Exception($"Namespace {ns} contains invalid characters. (colon)");
        }

        public void ReloadAllBlueprints()
        {
            var providers = _providers.Values.ToArray();
            _providers.Clear();
            foreach (var providerData in providers) RegisterProvider(providerData.provider);
        }

        public void ReloadProvider(IBlueprintProvider provider)
        {
            if (_providers.ContainsKey(provider.Namespace)) _providers.Remove(provider.Namespace);
            RegisterProvider(provider);
        }

        private void RecombineBlueprints()
        {
            _itemBlueprints.Clear();
            _rawItemBlueprints.Clear();
            foreach (var provider in _providers.Values)
            {
                foreach (var item in provider.itemBlueprints)
                {
                    _itemBlueprints.Add(item.Key, item.Value);
                }
                foreach (var item in provider.rawItemBlueprints)
                {
                    _rawItemBlueprints.Add(item.Key, item.Value);
                }
            }
        }

        public void RegisterProvider(IBlueprintProvider provider)
        {
            var ns = provider.Namespace;
            AssertProviderNamespaceValid(ns);

            var files = provider.GetBlueprints();
            var providerData = new ProviderData();
            providerData.provider = provider;

            // deserialize
            foreach (var file in files)
            {
                var blueprint = _deserializer.Deserialize<ItemBlueprint>(File.ReadAllText(file));
                blueprint.FilePath = file;
                blueprint.baseName = Path.GetFileNameWithoutExtension(file);
                var name = ns + ":" + blueprint.baseName;
                if (blueprint._Parent is string parentName && !parentName.StartsWith(ns + ":"))
                    blueprint._Parent = ns + ":" + parentName;
                blueprint.name = name;

                if (blueprint.BlueprintType != "item")
                {
                    Log.Error($"Blueprint {name} does not have a recognized type. Ignoring.");
                    continue;
                }

                if (providerData.itemBlueprints.ContainsKey(name))
                {
                    Log.Error($"Duplicate item {name} found. Ignoring.");
                    continue;
                }

                providerData.itemBlueprints.Add(name, blueprint);
                providerData.rawItemBlueprints.Add(name, blueprint.Clone());
            }
            
            LinkBlueprints(providerData.itemBlueprints);
            RemoveCircularReferences(providerData.itemBlueprints);
            ResolveBlueprints(providerData.itemBlueprints);

            _providers.Add(ns, providerData);
            RecombineBlueprints();
        }

        private void LinkBlueprints<T>(Dictionary<string, T> blueprints) where T : Blueprint
        {
            foreach (var blueprint in blueprints.Values)
                if (blueprint._Parent is string parentName)
                {
                    if (blueprints.ContainsKey(parentName))
                    {
                        var parent = blueprints[parentName];
                        if (parent.BlueprintType != blueprint.BlueprintType)
                        {
                            Log.Error(
                                $"Parent {parentName} for {blueprint.name} is of type {parent.BlueprintType} but {blueprint.name} is of type {blueprint.BlueprintType}. Ignoring parent.");
                            blueprint._Parent = null;
                            continue;
                        }

                        blueprint.Parent = blueprints[parentName];
                        Log.Info($"Linked {blueprint.name} to parent {parentName}");
                    }
                    else
                    {
                        Log.Error($"Parent {parentName} not found for {blueprint.name}. Ignoring parent.");
                        blueprint._Parent = null;
                    }
                }
        }

        private void RemoveCircularReferences<T>(Dictionary<string, T> blueprints) where T : Blueprint
        {
            var circularReferences = new List<T>();
            foreach (var blueprint in blueprints.Values)
            {
                Blueprint current = blueprint;
                while (current.Parent != null)
                {
                    if (current.Parent == blueprint)
                    {
                        circularReferences.Add(blueprint);
                        break;
                    }

                    current = current.Parent;
                }
            }

            foreach (var blueprint in circularReferences)
            {
                Log.Error($"Circular reference detected for {blueprint.name}. Removing parent.");
                blueprint._Parent = null;
            }
        }

        private void ResolveBlueprints<T>(Dictionary<string, T> blueprints) where T : Blueprint
        {
            var toRemove = new List<T>();

            foreach (var blueprint in blueprints.Values)
            {
                if (blueprint.ResolveProperties()) continue;

                Log.Error($"Failed to resolve properties for {blueprint.name}. Removing blueprint and all children.");
                toRemove.Add(blueprint);
            }

            foreach (var blueprint in toRemove) blueprints.Remove(blueprint.name);

            foreach (var blueprint in blueprints.Values)
            {
                var parent = blueprint.Parent;
                while (parent != null)
                {
                    if (toRemove.Contains(parent))
                    {
                        blueprints.Remove(blueprint.name);
                        break;
                    }

                    parent = parent.Parent;
                }
            }
        }

        public ItemBlueprint? GetRawBlueprint(string blueprintName)
        {
            return !_rawItemBlueprints.TryGetValue(blueprintName, out var blueprint) ? null : blueprint;
        }
        
        public ItemBlueprint? GetBlueprint(string blueprintName)
        {
            return !_itemBlueprints.TryGetValue(blueprintName, out var blueprint) ? null : blueprint;
        }
        
        public void SaveBlueprint<T>(T blueprint, string path) where T : Blueprint
        {
            var yaml = _serializer.Serialize(blueprint);
            File.WriteAllText(path, yaml);
        }
        
        public List<string> GetResolvedBlueprintList()
        {
            return _itemBlueprints.Keys.ToList();
        }

        private class ProviderData
        {
            public readonly Dictionary<string, ItemBlueprint> rawItemBlueprints =
                new Dictionary<string, ItemBlueprint>();

            public readonly Dictionary<string, ItemBlueprint> itemBlueprints = new Dictionary<string, ItemBlueprint>();
            public IBlueprintProvider provider = null!;
        }

        public void PostProcessItemBlueprints(Action<ItemBlueprint> processor)
        {
            foreach (var (key, value) in _itemBlueprints)
            {
                processor(value);
            }
        }
    }
}