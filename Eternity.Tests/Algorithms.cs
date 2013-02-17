using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Eternity.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eternity.Tests
{
    [TestClass]
    public class Algorithms
    {
        [TestMethod]
        public void TestInformedSearch1()
        {
            var result = Search.InformedSearch(
                0,
                t => new List<int> { t + 1, t + 5, t + 50, t + 250, t - 1, t - 5, t - 50, t - 250, t * 2, t * 10 },
                t => Math.Abs(45512 - t),
                t => t == 45512);
            Console.WriteLine("Optimised path is of length: " + result.Count);
            result.ForEach(Console.WriteLine);
        }

        public class Is2Class : IEquatable<Is2Class>
        {
            public int Cost { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public Is2Class(int x, int y, int cost)
            {
                X = x;
                Y = y;
                Cost = cost;
            }

            public bool Equals(Is2Class other)
            {
                return X == other.X && Y == other.Y;
            }

            public override string ToString()
            {
                return "{" + X + ", " + Y + " (" + Cost + ")" + "}";
            }
        }

        public IEnumerable<Is2Class> Expand(Is2Class c, int[,] grid)
        {
            if (c.X > 0) yield return new Is2Class(c.X - 1, c.Y, c.Cost + grid[c.X - 1, c.Y]);
            if (c.X < 6) yield return new Is2Class(c.X + 1, c.Y, c.Cost + grid[c.X + 1, c.Y]);
            if (c.Y > 0) yield return new Is2Class(c.X, c.Y - 1, c.Cost + grid[c.X, c.Y - 1]);
            if (c.Y < 6) yield return new Is2Class(c.X, c.Y + 1, c.Cost + grid[c.X, c.Y + 1]);
        }

        [TestMethod]
        public void TestInformedSearch2()
        {
            var grid = new[,] {
                {1,1,1,1,1,1,6},
                {1,2,1,1,1,1,5},
                {1,1,3,1,1,1,4},
                {1,1,1,4,1,1,3},
                {1,1,1,1,5,1,2},
                {1,1,1,1,1,6,1},
                {6,5,4,3,2,1,7}
            };

            var result = Search.InformedSearch(
                new Is2Class(0, 0, 0),
                t => new List<Is2Class>(Expand(t, grid)),
                t => t.Cost + ((6 - t.X) + (6 - t.Y)),
                t => t.X == 6 && t.Y == 6);
            Console.WriteLine("Optimised path is of length: " + result.Count);
            result.ForEach(Console.WriteLine);
        }
    }
}
