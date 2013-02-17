using System.IO;
using Eternity.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Eternity.Tests
{
    
    
    /// <summary>
    ///This is a test class for ResourceDefinitionTest and is intended
    ///to contain all ResourceDefinitionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResourceDefinitionTest
    {
        [TestMethod()]
        public void ParseTest()
        {
            var file = @"C:\Eternity\Eternity.Game.TurnBasedWarsGame\Resources\Terrain.etr";
            var definition = File.ReadAllText(file);
            var actual = ResourceDefinition.Parse(definition);
            Console.WriteLine(actual);
        }
    }
}
