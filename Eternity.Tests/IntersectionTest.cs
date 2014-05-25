using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Eternity.DataStructures.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eternity.Tests
{
    [TestClass]
    public class IntersectionTest
    {
        [TestMethod]
        public void TestBoxLineIntersectionEntry()
        {
            var box = new Box(10, 10, 20, 20);
            var line = new Line(new Point(5, 5), new Point(15, 15));

            Point p1, p2;
            var intersect = line.Intersects(box, out p1, out p2);
            Assert.IsTrue(intersect);
            Assert.IsNotNull(p1);
            Assert.IsNull(p2);
            Assert.AreEqual(10, p1.X);
            Assert.AreEqual(10, p1.Y);
        }

        [TestMethod]
        public void TestBoxLineIntersectionExit()
        {
            var box = new Box(10, 10, 20, 20);
            var line = new Line(new Point(15, 15), new Point(5, 5));

            Point p1, p2;
            var intersect = line.Intersects(box, out p1, out p2);
            Assert.IsTrue(intersect);
            Assert.IsNull(p1);
            Assert.IsNotNull(p2);
            Assert.AreEqual(10, p2.X);
            Assert.AreEqual(10, p2.Y);
        }

        [TestMethod]
        public void TestBoxLineIntersectionLeftSideEntry()
        {
            var box = new Box(10, 10, 20, 20);
            var line = new Line(new Point(9, 10), new Point(10, 10));

            Point p1, p2;
            var intersect = line.Intersects(box, out p1, out p2);
            Assert.IsTrue(intersect);
            Assert.IsNotNull(p1);
            Assert.IsNull(p2);
            Assert.AreEqual(10, p1.X);
            Assert.AreEqual(10, p1.Y);
        }

        [TestMethod]
        public void TestBoxLineIntersectionRightSideEntry()
        {
            var box = new Box(0, 0, 1, 1); // 1 pixel box.
            var line = new Line(new Point(1, 0), new Point(0, 0));

            Point p1, p2;
            var intersect = line.Intersects(box, out p1, out p2);
            Assert.IsTrue(intersect);
            Assert.IsNotNull(p1);
            Assert.IsNull(p2);
            Assert.AreEqual(0, p1.X);
            Assert.AreEqual(0, p1.Y);
        }

        [TestMethod]
        public void TestBoxLineIntersectionLeftSideExit()
        {
            var box = new Box(10, 10, 20, 20);
            var line = new Line(new Point(10, 10), new Point(9, 10));

            Point p1, p2;
            var intersect = line.Intersects(box, out p1, out p2);
            Assert.IsTrue(intersect);
            Assert.IsNull(p1);
            Assert.IsNotNull(p2);
            Assert.AreEqual(10, p2.X);
            Assert.AreEqual(10, p2.Y);
        }

        [TestMethod]
        public void TestBoxLineIntersectionRightSideExit()
        {
            var box = new Box(10, 10, 20, 20);
            var line = new Line(new Point(30, 10), new Point(31, 10));

            Point p1, p2;
            var intersect = line.Intersects(box, out p1, out p2);
            Assert.IsTrue(intersect);
            Assert.IsNull(p1);
            Assert.IsNotNull(p2);
            Assert.AreEqual(30, p2.X);
            Assert.AreEqual(10, p2.Y);
        }

        [TestMethod]
        public void TestBoxLineNoDiagonalIntersection()
        {
            var box = new Box(10, 10, 20, 20);
            var line = new Line(new Point(9, 11), new Point(11, 9));

            Point p1, p2;
            var intersect = line.Intersects(box, out p1, out p2);
            Assert.IsFalse(intersect);
            Assert.IsNull(p1);
            Assert.IsNull(p2);
        }
    }
}
