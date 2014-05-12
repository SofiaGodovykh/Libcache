namespace Kontur.Cache.Distributed
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class TopologyRing
    {
        private readonly List<Topology> topologies;

        public TopologyRing(IEnumerable<Topology> topologies)
        {
            if (topologies == null)
            {
                throw new ArgumentNullException("topologies");
            }

            this.topologies = new List<Topology>(topologies);
        }

        public IEnumerable<RedisEndPoint> GetAllEndPoints()
        {
            return topologies.Select(x => x.EndPoint).Distinct();
        }

        public IEnumerable<RedisEndPoint> Resolve(int token)
        {
            return topologies.Where(x => x.Contains(token)).Select(x => x.EndPoint);
        }
    }
}