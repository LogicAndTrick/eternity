using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eternity.Resources
{
    public static class ResourceManager
    {
        private static readonly List<ResourceDefinition> ResourceDefinitions;
        private static readonly List<string> ResourcePaths;

        static ResourceManager()
        {
            ResourceDefinitions = new List<ResourceDefinition>();
            ResourcePaths = new List<string>();
        }

        public static void AddResourcePath(string path)
        {
            ResourcePaths.Insert(0, path);
        }

        public static string GetResource(string name)
        {
            return ResourcePaths.Select(x => Path.Combine(x, name)).FirstOrDefault(File.Exists);
        }

        public static ResourceDefinition GetResourceDefinition(string type, string name)
        {
            return ResourceDefinitions.FirstOrDefault(x => x.DefinitionType == type && x.GetData("Name") == name);
        }

        public static IEnumerable<ResourceDefinition> GetResourceDefinitionGroup(string group)
        {
            return ResourceDefinitions.Where(x => x.GetData("Group") == group);
        }

        public static void AddResource(string file)
        {
            ResourceDefinitions.AddRange(ResourceDefinition.Parse(File.ReadAllText(file)));
        }

        public static void LoadResources(DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles("*.etr"))
            {
                AddResource(file.FullName);
            }
        }

        public static IEnumerable<ResourceDefinition> GetResourceDefinitions(string key, string value)
        {
            return GetResourceDefinitions(ResourceDefinitions, new Dictionary<string, string>
                                                                   {
                                                                       {key, value}
                                                                   });
        }

        public static IEnumerable<ResourceDefinition> GetResourceDefinitions(Dictionary<string, string> values)
        {
            return GetResourceDefinitions(ResourceDefinitions, values);
        }

        private static IEnumerable<ResourceDefinition> GetResourceDefinitions(IEnumerable<ResourceDefinition> list, Dictionary<string, string> values)
        {
            var defs = new List<ResourceDefinition>();
            foreach (var rd in list)
            {
                var match = true;
                foreach (var kv in values)
                {
                    if (rd.GetData(kv.Key) != kv.Value) match = false;
                }
                if (match) defs.Add(rd);
                defs.AddRange(GetResourceDefinitions(rd.ChildrenDefinitions, values));
            }
            return defs;
        }
    }
}
