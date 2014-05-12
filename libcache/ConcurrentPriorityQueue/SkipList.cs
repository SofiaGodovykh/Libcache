namespace Kontur.Cache.ConcurrentPriorityQueue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class SkipList<T>
    {
        private static uint randomSeed;
        private static Random random;
        private readonly int maxLevel;
        private readonly Node head;
        private readonly Node tail;

        public SkipList()
            : this(14)
        {
        }

        public SkipList(int maxLevel)
        {
            if (maxLevel < 1)
            {
                throw new ArgumentException("WTF?!", "maxLevel");
            }

            random = new Random();
            head = new Node(DateTime.MinValue, maxLevel);
            tail = new Node(DateTime.MaxValue, maxLevel);
            this.maxLevel = maxLevel;

            randomSeed = (uint)(DateTime.Now.Millisecond) | 0x0100;

            for (int i = 0; i < head.Next.Length; ++i)
            {
                head.Next[i] = new MarkedAtomicReference<Node>(tail, false);
            }
        }

        public bool Add(T value, DateTime score)
        {
            return Add(new Node(value, score, maxLevel));
        }

        public bool Add(Node node)
        {
            var preds = new Node[maxLevel + 1];
            var succs = new Node[maxLevel + 1];

            while (true)
            {
                var found = this.Find(node, ref preds, ref succs);
                //if (found)
                //{
                //   return false;
                //}
                //else
                {
                    var topLevel = node.TopLevel;
                    var bottomLevel = 0;

                    for (var level = bottomLevel; level <= topLevel; ++level)
                    {
                        var tempSucc = succs[level];
                        node.Next[level] = new MarkedAtomicReference<Node>(tempSucc, false); // todo check if this operation is equal to set in Java
                    }

                    var pred = preds[bottomLevel];
                    var succ = succs[bottomLevel];

                    node.Next[bottomLevel] = new MarkedAtomicReference<Node>(succ, false);

                    if (!pred.Next[bottomLevel].CompareAndExchange(node, false, succ, false))
                    {
                        continue;
                    }

                    for (int level = bottomLevel + 1; level <= topLevel; level++)
                    {
                        while (true)
                        {
                            pred = preds[level];
                            succ = succs[level];

                            if (pred.Next[level].CompareAndExchange(node, false, succ, false))
                            {
                                break;
                            }

                            this.Find(node, ref preds, ref succs);
                        }
                    }
                    return true;
                }
            }
        }

        public Node Peek()
        {
            Node res = this.head.Next[0].Value;
            if (res != null)
                return res;
            return null;
        }

        public bool Remove(Node node)
        {
            int bottomLevel = 0;
            var preds = new Node[maxLevel + 1];
            var succs = new Node[maxLevel + 1];
            Node succ;

            while (true)
            {
                bool found = this.Find(node, ref preds, ref succs);
                if (!found)
                {
                    return false;
                }

                else
                {
                    for (int level = node.TopLevel; level > bottomLevel; level--)
                    {
                        bool isMarked = false;
                        succ = node.Next[level].Get(ref isMarked);

                        while (!isMarked)
                        {
                            node.Next[level].CompareAndExchange(succ, true, succ, false);
                            succ = node.Next[level].Get(ref isMarked);
                        }
                    }

                    bool marked = false;
                    succ = node.Next[bottomLevel].Get(ref marked);

                    while (true)
                    {
                        bool iMarkedIt = node.Next[bottomLevel].CompareAndExchange(succ, true, succ, false);
                        succ = succs[bottomLevel].Next[bottomLevel].Get(ref marked);

                        if (iMarkedIt)
                        {
                            this.Find(node, ref preds, ref succs);
                            return true;
                        }
                        else
                        {
                            if (marked)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        public Node FindAndMarkMin()
        {
            var curr = this.head.Next[0].Value;

            while (curr != tail)
            {
                if (!curr.NodeValue.Marked.Value)
                {
                    //if (curr.Marked.CompareAndSet(false, true))
                    if (curr.NodeValue.Marked.CompareAndSet(false, true))
                    {
                        return curr;
                    }
                    else
                    {
                        curr = curr.Next[0].Value;
                    }
                }
            }
            return null;
        }

        private bool Find(Node node, ref Node[] preds, ref Node[] succs)
        {
            int bottomLevel = 0;
            bool marked = false;
            bool snip = false;
            Node pred = null;
            Node curr = null;
            Node succ = null;
        retry: //TODO: remove goto
            while (true)
            {
                pred = head;
                for (int level = maxLevel; level >= bottomLevel; level--)
                {
                    curr = pred.Next[level].Value;
                    while (true)
                    {
                        succ = curr.Next[level].Get(ref marked);
                        while (marked)
                        {
                            snip = pred.Next[level].CompareAndExchange(succ, false, curr, false);
                            if (!snip)
                            {
                                goto retry;
                            }

                            curr = pred.Next[level].Value;
                            succ = curr.Next[level].Get(ref marked);
                        }

                        if (curr.NodeKey < node.NodeKey)
                        {
                            pred = curr;
                            curr = succ;
                        }

                        else
                        {
                            break;
                        }
                    }

                    preds[level] = pred;
                    succs[level] = curr;
                }
                return (curr.NodeKey == node.NodeKey);
            }
        }

        private static int RandomLevel(int maxLevel)
        {
            uint x = randomSeed;
            x ^= x << 13;
            x ^= x >> 17;
            randomSeed = x ^= x << 5;
            if ((x & 0x80000001) != 0)
            {
                return 0;
            }

            int level = 1;
            while (((x >>= 1) & 1) != 0)
            {
                level++;
            }

            return Math.Min(level, maxLevel);
        }

        /*public bool Contains(T value)
        {
            int bottomLevel = 0;
            int key = value.GetHashCode();
            bool marked = false;
            Node pred = head;
            Node curr = null;
            Node succ = null;

            for (int level = MAX_LEVEL; level >= bottomLevel; level--)
            {
                curr = pred.Next[level].Value;
                while (true)
                {
                    succ = curr.Next[level].Get(ref marked);
                    while (marked)
                    {
                        curr = pred.Next[level].Value;
                        succ = curr.Next[level].Get(ref marked);
                    }

                    if (curr.NodeKey < key)
                    {
                        pred = curr;
                        curr = succ;
                    }

                    else
                    {
                        break;
                    }
                }
            }
            return (curr.NodeKey == key);
        }*/

        public class Node
        {
            private readonly MarkedAtomicReference<T> nodeValue;
            private readonly DateTime nodeKey;
            private readonly MarkedAtomicReference<Node>[] next;
            private readonly int topLevel;

            public MarkedAtomicReference<T> NodeValue
            {
                get { return nodeValue; }
            }

            public DateTime NodeKey
            {
                get { return nodeKey; }
            }

            public MarkedAtomicReference<Node>[] Next
            {
                get { return next; }
            }

            public int TopLevel
            {
                get { return topLevel; }
            }

            public Node(DateTime key, int maxLevel)
            {
                nodeValue = new MarkedAtomicReference<T>(default(T), false);
                this.nodeKey = key;
                next = new MarkedAtomicReference<Node>[maxLevel + 1];
                for (int i = 0; i < next.Length; ++i)
                {
                    next[i] = new MarkedAtomicReference<Node>(null, false);
                }
                topLevel = maxLevel;
            }

            public Node(T value, DateTime key, int maxLevel)
            {
                nodeValue = new MarkedAtomicReference<T>(value, false);
                this.nodeKey = key;
                var height = RandomLevel(maxLevel);
                next = new MarkedAtomicReference<Node>[height + 1];
                for (int i = 0; i < next.Length; ++i)
                {
                    next[i] = new MarkedAtomicReference<Node>(null, false);
                }
                topLevel = height;
            }
        }
    }
}
    
