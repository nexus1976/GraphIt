namespace GraphIt
{
    public class GraphNode
    {
        private readonly Dictionary<int, GraphNode> _neighbors;
        public int Value { get; private set; }
        public IEnumerable<GraphNode> Neighbors 
        { 
            get
            {
                return _neighbors.Values.AsEnumerable();
            }
        }

        public GraphNode(int value)
        {
            Value = value;
            _neighbors = new Dictionary<int, GraphNode>();
        }

        public bool AddNeighbor(GraphNode neighbor)
        {
            if (neighbor != null && !_neighbors.ContainsKey(neighbor.Value))
            {
                _neighbors.Add(neighbor.Value, neighbor);
                return true;
            }
            else
                return false;                   
        }

        public bool RemoveNeighbor(GraphNode neighbor)
        {
            if (neighbor != null && _neighbors.ContainsKey(neighbor.Value))
            {
                _neighbors.Remove(neighbor.Value);
                return true;
            }
            else
                return false;
        }
        public bool RemoveNeighbor(int value)
        {
            if (!_neighbors.ContainsKey(value))
                return false;
            _neighbors.Remove(value);
            return true;
        }
        public bool HasNeighbor(int value)
        {
            return _neighbors.ContainsKey(value);
        }
        public bool HasNeighbor(GraphNode node)
        {
            if (node == null) return false;
            return _neighbors.ContainsKey(node.Value);
        }
        public void ClearAllNeighbors()
        {
            _neighbors.Clear();
        }

        public override string ToString()
        {
            return $"GraphNode Value: {this.Value} | Neighbors Count: {this.Neighbors.Count()}";
        }
    }
}
