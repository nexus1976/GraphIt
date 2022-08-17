namespace GraphIt
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Build graph based on GraphExample.webp image 
            List<int> nodes = new() { 0, 1, 2, 3, 4, 5, 6, 7 };
            List<(int node1, int node2)> edges = new()
            {
                ( 0, 1 ),
                ( 0, 2 ),
                ( 0, 7 ),
                ( 1, 4 ),
                ( 2, 4 ),
                ( 2, 3 ),
                ( 3, 5 ),
                ( 3, 6 ),
                ( 7, 6 )
            };
            var graph = BuildGraph(nodes, edges);

            var sbDFS = graph.PrintTraversal(0, TraversalType.DepthFirst);
            Console.WriteLine($"DFS: {sbDFS}");
            Console.WriteLine();
            
            var sbBFS = graph.PrintTraversal(0, TraversalType.BreadthFirst);
            Console.WriteLine($"BFS: {sbBFS}");
            Console.WriteLine();

            var sp = graph.GetShortestPath(1, 6);
            Console.WriteLine($"Shortest Path: {string.Join(" ", sp)}");
            Console.ReadLine();
        }

        public static Graph BuildGraph(List<int> nodes, List<(int node1, int node2)> edges)
        {
            var graph = new Graph();
            foreach (var value in nodes)
            {
                graph.AddNode(value);
            }
            foreach (var (node1, node2) in edges)
            {
                graph.AddEdge(node1, node2);
            }
            return graph;
        }
    }
}