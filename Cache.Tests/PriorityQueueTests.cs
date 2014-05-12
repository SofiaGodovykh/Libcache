namespace Kontur.Cache.Tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class PriorityQueueTests
    {
        [Test]
        public void EnqueueOrderedItemsTest()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(3, queue.Count);
            Assert.That(queue, Is.EquivalentTo(new int[] { 1, 2, 3 }));
        }

        [Test]
        public void EnqueueTest()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(1);

            Assert.AreEqual(3, queue.Count);
            Assert.That(queue, Is.EquivalentTo(new int[] { 1, 2, 3 }));
        }

        [Test]
        public void EnqueueTest1()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(0);

            Assert.AreEqual(5, queue.Count);
            Assert.That(queue, Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4 }));
        }

        [Test]
        public void EnqueueTest2()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(0);
            queue.Enqueue(1);

            Assert.AreEqual(6, queue.Count);
            Assert.That(queue, Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 1 }));
        }

        [Test]
        public void EnqueueTest3()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(0);
            queue.Enqueue(1);
            queue.Enqueue(12);

            Assert.AreEqual(7, queue.Count);
            Assert.That(queue, Is.EquivalentTo(new int[] { 0, 1, 2, 3, 4, 1, 12 }));
        }

        [Test]
        public void PeekTest()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(0);
            queue.Enqueue(1);
            queue.Enqueue(0);

            Assert.AreEqual(0, queue.Peek());
        }

        [Test]
        public void PeekTest1()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(0);
            queue.Enqueue(0);
            queue.Enqueue(-10);
            queue.Enqueue(1);
            queue.Enqueue(0);

            Assert.AreEqual(-10, queue.Peek());
        }

        [Test]
        public void PeekTest2()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(-10);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(0);
            queue.Enqueue(0);
            queue.Enqueue(-10);
            queue.Enqueue(1); 
            queue.Enqueue(0);

            Assert.AreEqual(-10, queue.Peek());
        }

        [Test]
        public void PeekTest3()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(-10);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(-29);
            queue.Enqueue(0);
            queue.Enqueue(-10);
            queue.Enqueue(1);
            queue.Enqueue(0);

            Assert.AreEqual(-29, queue.Peek());
        }

        [Test]
        public void DequeueTest()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(-10);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(-29);
            queue.Enqueue(0);
            queue.Enqueue(-10);
            queue.Enqueue(1);
            queue.Enqueue(0);

            Assert.AreEqual(-29, queue.Dequeue());
            Assert.AreEqual(-10, queue.Dequeue());
            Assert.AreEqual(-10, queue.Dequeue());
            Assert.AreEqual(0, queue.Dequeue());
            Assert.AreEqual(0, queue.Dequeue());
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
            Assert.AreEqual(4, queue.Dequeue());
        }

        [Test]
        public void DequeueTest1()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);

            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
        }


        [Test]
        public void DequeueTest2()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
        }

        [Test]
        public void DequeueTest3()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(5);
            queue.Enqueue(4);
            queue.Enqueue(3);

            Assert.AreEqual(3, queue.Dequeue());
            Assert.AreEqual(4, queue.Dequeue());
            Assert.AreEqual(5, queue.Dequeue());
        }


        [Test]
        public void DequeueTest4()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(10);
            queue.Enqueue(9);
            queue.Enqueue(8);
            queue.Enqueue(7);
            queue.Enqueue(6);
            queue.Enqueue(5);
            queue.Enqueue(4);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(1);

            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
            Assert.AreEqual(4, queue.Dequeue());
            Assert.AreEqual(5, queue.Dequeue());
            Assert.AreEqual(6, queue.Dequeue());
            Assert.AreEqual(7, queue.Dequeue());
            Assert.AreEqual(8, queue.Dequeue());
            Assert.AreEqual(9, queue.Dequeue());
            Assert.AreEqual(10, queue.Dequeue());
        }

        public void RemoveTest()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(3);
            queue.Enqueue(2);
            queue.Enqueue(-10);
            queue.Enqueue(1);
            queue.Enqueue(4);
            queue.Enqueue(-29);
            queue.Enqueue(0);
            queue.Enqueue(-10);
            queue.Enqueue(1);
            queue.Enqueue(0);

            Assert.AreEqual(-29, queue.Dequeue());
            Assert.AreEqual(-10, queue.RemoveAt(queue.GetItems.IndexOf(-10)));
            Assert.AreEqual(2, queue.RemoveAt(queue.GetItems.IndexOf(2)));
            Assert.AreEqual(10, queue.Count);
            Assert.AreEqual(-10, queue.Dequeue());
            Assert.AreEqual(0, queue.Dequeue());
            Assert.AreEqual(0, queue.Dequeue());
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(1, queue.Dequeue());
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
            Assert.AreEqual(4, queue.Dequeue());
        }
    }
}