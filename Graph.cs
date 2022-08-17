using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphIt
{
    public enum TraversalType
    {
        DepthFirst, BreadthFirst
    }
    public class Graph
    {
        private readonly Dictionary<int, GraphNode> _nodes;
        public IEnumerable<GraphNode> Nodes 
        { 
            get
            {
                return _nodes.Values.AsEnumerable();
            }
        }

        public Graph()
        {
            _nodes = new Dictionary<int, GraphNode>();
        }

        public bool AddNode(int value)
        {
            if (_nodes.ContainsKey(value))
                return false; // dupe
            GraphNode node = new(value);
            _nodes.Add(value, node);
            return true;
        }
        public bool RemoveNode(int value)
        {
            if (!_nodes.ContainsKey(value))
                return false; // not found
            foreach (var node in _nodes)
            {
                if (node.Value.HasNeighbor(value))
                    node.Value.RemoveNeighbor(value);
            }
            _nodes.Remove(value);
            return true;
        }
        public bool AddEdge(int value1, int value2)
        {
            if (!_nodes.TryGetValue(value1, out GraphNode node1) || !_nodes.TryGetValue(value2, out GraphNode node2))
                return false;
            if (!node1.AddNeighbor(node2) || !node2.AddNeighbor(node1)) // we add the node both ways since we're doing an undirected graph
                return false;
            return true;
        }
        public bool RemoveEdge(int value1, int value2)
        {
            if (!_nodes.TryGetValue(value1, out GraphNode node1) || !_nodes.TryGetValue(value2, out GraphNode node2))
                return false;
            if (!node1.RemoveNeighbor(node2) || !node2.RemoveNeighbor(node1)) // we remove the node both ways since we're doing an undirected graph
                return false;
            return true;
        }
        public StringBuilder PrintTraversal(int? rootNodeValue, TraversalType traversalType)
        {
            StringBuilder result = new();
            if (!_nodes.Any()) return result;
            int rootValue = rootNodeValue ?? _nodes.First().Key;
            GraphNode rootNode;
            if (_nodes.ContainsKey(rootValue))
                rootNode = _nodes[rootValue];
            else
                throw new ArgumentOutOfRangeException(nameof(rootNodeValue));

            if (traversalType == TraversalType.DepthFirst)
                PrintDepthFirst(rootNode, result);
            else
                PrintBreadthFirst(rootNode, result);
            return result;
        }

        public IEnumerable<int> GetShortestPath(int value1, int value2)
        {
            List<int> result = new();
            if (!_nodes.Any()) return result;
            GraphNode rootNode;
            if (_nodes.ContainsKey(value1))
                rootNode = _nodes[value1];
            else
                throw new ArgumentOutOfRangeException(nameof(value1));
            if (!_nodes.ContainsKey(value2))
                throw new ArgumentOutOfRangeException(nameof(value2));
            if (!rootNode.Neighbors.Any())
                return result;

            List<HashSet<int>> paths = new();
            ShortestPathRecursion(rootNode, value2, paths);
            int lastLength = int.MaxValue;
            foreach (var path in paths)
            {
                int length = path.Count;
                if (length < lastLength)
                {
                    result = path.ToList();
                    lastLength = length;
                }
            }
            
            return result;
        }        
        private void ShortestPathRecursion(GraphNode node, int destinationValue, List<HashSet<int>> paths, string currentPath = "")
        {
            var path = currentPath.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll<int>(x => Convert.ToInt32(x)).ToHashSet();
            if (!path.Contains(node.Value))
            {
                currentPath += $"{node.Value},";
                if (node.Value == destinationValue)
                {
                    path.Add(node.Value);
                    paths.Add(path);
                    return;
                }
                foreach (GraphNode neighbor in node.Neighbors)
                {
                    ShortestPathRecursion(neighbor, destinationValue, paths, currentPath);
                }
            }
        }

        private void PrintDepthFirst(GraphNode rootNode, StringBuilder log)
        {
            HashSet<int> visited = new();
            Dictionary<int, int> valueDepths = new()
            {
                { rootNode.Value, 0 }
            };
            PrintDepthFirstRecursion(rootNode, visited, log, 0, valueDepths);
        }
        private void PrintDepthFirstRecursion(GraphNode node, HashSet<int> visited, StringBuilder log, int currentDepth, Dictionary<int, int> valueDepths)
        {
            if (node == null || visited == null || log == null || valueDepths == null || visited.Contains(node.Value))
                return;
            log.Append($"{node.Value } ");
            int nextDepth = currentDepth + 1;
            if (node.Neighbors.Any())
            {
                foreach (var neighbor in node.Neighbors)
                {
                    if (!valueDepths.ContainsKey(neighbor.Value))
                    {
                        valueDepths.Add(neighbor.Value, nextDepth);
                        PrintDepthFirstRecursion(neighbor, visited, log, nextDepth, valueDepths);
                    }
                }
            }
            visited.Add(node.Value);
        }

        private void PrintBreadthFirst(GraphNode rootNode, StringBuilder log)
        {
            Dictionary<int, int> valueDepths = new()
            {
                { rootNode.Value, 0 }
            };
            Dictionary<int, HashSet<int>> depthQueues = new();
            PrintBreadthFirstRecursion(rootNode, 0, depthQueues, valueDepths);

            int depthCount = depthQueues.Count;
            for (int depth = 0; depth < depthCount; depth++)
            {
                var currentDepthQueue = depthQueues[depth];
                foreach (var nodeValue in currentDepthQueue)
                {
                    log.Append($"{nodeValue} ");
                }
            }

        }
        private void PrintBreadthFirstRecursion(GraphNode node, int currentDepth, Dictionary<int, HashSet<int>> depthQueues, Dictionary<int, int> valueDepths)
        {
            if (node == null || depthQueues == null || valueDepths == null)
                return;
            if (!depthQueues.ContainsKey(currentDepth))
                depthQueues.Add(currentDepth, new HashSet<int>());
            var currentDepthQueue = depthQueues[currentDepth];
            if (!currentDepthQueue.Contains(node.Value))
                currentDepthQueue.Add(node.Value);
            int nextDepth = currentDepth + 1;
            
            if (node.Neighbors.Any())
            {
                foreach (var neighbor in node.Neighbors)
                {
                    if (!valueDepths.ContainsKey(neighbor.Value))
                        valueDepths.Add(neighbor.Value, nextDepth);
                }
                foreach (var neighbor in node.Neighbors)
                {
                    valueDepths.TryGetValue(neighbor.Value, out int neighborLevel);
                    if (neighborLevel > nextDepth)
                    {
                        valueDepths[neighbor.Value] = nextDepth;
                        depthQueues[neighborLevel].Remove(neighbor.Value);
                        PrintBreadthFirstRecursion(neighbor, nextDepth, depthQueues, valueDepths);
                    }
                    else if (neighborLevel == nextDepth)
                        PrintBreadthFirstRecursion(neighbor, nextDepth, depthQueues, valueDepths);
                }
            }
        }
    }
}
