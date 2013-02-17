using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eternity.Resources
{
    public class ResourceDefinition
    {
        public static IEnumerable<ResourceDefinition> Parse(string definition)
        {
            var lines = definition.Split('\n');
            var i = 0;
            ResourceDefinition def;
            while ((def = ParseHelper(lines, i, out i)) != null)
            {
                yield return def;
            }
        }

        // TODO: this is somewhat messy
        private static ResourceDefinition ParseHelper(string[] lines, int startIndex, out int endIndex)
        {
            var inDef = false; // Are we inside a { structure } ?
            ResourceDefinition cDef = null;
            while (startIndex < lines.Length)
            {
                var line = lines[startIndex];
                startIndex++;
                // Trim single-line // comments from the line
                var index = line.IndexOf("//");
                if (index >= 0) line = line.Substring(0, index);
                // Remove whitespace
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line == "{") // Opening the structure
                {
                    inDef = true;
                    continue; // All good
                }
                if (line == "}") // Closing the structure
                {
                    break; // We're done here
                }
                index = line.IndexOf(':'); // Key: Value  - OR - NewStructureName
                if (index < 0 && !inDef) // NewStructureName when not in a structure (create node)
                {
                    cDef = new ResourceDefinition(line);
                }
                else if (index < 0) // NewStructureName when in a structure (add as child)
                {
                    var helper = ParseHelper(lines, startIndex - 1, out startIndex);
                    cDef.MergeChild(helper);
                }
                else // Key: Value (should be in a structure, add data)
                {
                    var key = line.Substring(0, index).Trim();
                    var value = line.Substring(index + 1).Trim();
                    var ob = key.IndexOf('[');
                    var cb = key.IndexOf(']');
                    if (ob > 0 && cb > ob)
                    {
                        var fmt = key.Substring(ob + 1, cb - ob - 1);
                        key = key.Substring(0, ob);
                        value = string.Join(" ", value.Split(' ').Select(x => x == "." ? "." : string.Format(fmt, x)));
                    }
                    if (cDef.DefinitionData.ContainsKey(key)) value = cDef.GetData(key) + " " + value;
                    cDef.DefinitionData[key] = value;
                }
            }
            // Set the ending index
            endIndex = startIndex;
            return cDef;
        }

        public string DefinitionType { get; private set; }
        public Dictionary<string, string> DefinitionData { get; private set; }
        public List<ResourceDefinition> ChildrenDefinitions { get; private set; }

        public ResourceDefinition(string definitionType)
        {
            DefinitionType = definitionType;
            DefinitionData = new Dictionary<string, string>();
            ChildrenDefinitions = new List<ResourceDefinition>();
        }

        public bool HasData(string key)
        {
            return DefinitionData.ContainsKey(key);
        }

        public string GetData(string key)
        {
            return HasData(key) ? DefinitionData[key] : "";
        }

        public string GetData(string key, string defaultVal)
        {
            return HasData(key) ? DefinitionData[key] : defaultVal;
        }

        public void MergeChild(ResourceDefinition child)
        {
            var list = new List<ResourceDefinition> { child };
            if (child.DefinitionData.ContainsKey("Name"))
            {
                list.Clear();
                foreach (var split in child.DefinitionData["Name"].Split(' '))
                {
                    var clone = child.Clone();
                    clone.DefinitionData["Name"] = split;
                    list.Add(clone);
                }
            }
            foreach (var ch in list)
            {
                var node = ChildrenDefinitions.FirstOrDefault(x => x.GetData("Name") == ch.GetData("Name"));
                if (ch.HasData("Name") && node != null)
                {
                    // Merge it in
                    foreach (var kv in ch.DefinitionData)
                    {
                        node.DefinitionData[kv.Key] = kv.Value;
                    }
                    foreach (var cch in ch.ChildrenDefinitions)
                    {
                        node.MergeChild(cch);
                    }
                }
                else
                {
                    // Just add it
                    ChildrenDefinitions.Add(ch);
                }
            }
        }

        public ResourceDefinition Clone()
        {
            var rd = new ResourceDefinition(DefinitionType);
            foreach (var kv in DefinitionData)
            {
                rd.DefinitionData[kv.Key] = kv.Value;
            }
            foreach (var ch in ChildrenDefinitions)
            {
                rd.ChildrenDefinitions.Add(ch.Clone());
            }
            return rd;
        }
    }
}
