namespace IT4W_Challenge.Helpers
{
    public class Graph
    {
        public int Vertices { get; }
        public List<(int u, int v, int cost)> Edges { get; }
        public List<(int u, int v, int time)> TimeEdges { get; }

        public Graph(int vertices)
        {
            Vertices = vertices;
            Edges = new List<(int, int, int)>();
            TimeEdges = new List<(int, int, int)>();
        }

        public void AddEdge(int u, int v, int cost)
        {
            Edges.Add((u, v, cost));
        }

        // Método para agregar aristas con tiempos
        public void AddTimeEdge(int u, int v, int time)
        {
            TimeEdges.Add((u, v, time));
        }

        // Algoritmo Bellman-Ford para encontrar las rutas más cortas y detectar ciclos negativos
        public (int[] dist, bool hasNegativeCycle) BellmanFord(int source)
        {
            int[] dist = new int[Vertices];
            Array.Fill(dist, int.MaxValue);
            dist[source] = 0;

            // Relajar todas las aristas |V| - 1 veces
            for (int i = 0; i < Vertices - 1; i++)
            {
                foreach (var (u, v, cost) in Edges)
                {
                    if (dist[u] != int.MaxValue && dist[u] + cost < dist[v])
                    {
                        dist[v] = dist[u] + cost;
                    }
                }
            }

            // Detectar ciclos negativos
            foreach (var (u, v, cost) in Edges)
            {
                if (dist[u] != int.MaxValue && dist[u] + cost < dist[v])
                {
                    return (dist, true);  // Ciclo negativo encontrado
                }
            }

            return (dist, false);
        }

        // Método para encontrar las rutas de menor costo a múltiples destinos
        public Dictionary<int, int> FindMultipleDestinations(int source, List<int> destinations)
        {
            var (distances, hasNegativeCycle) = BellmanFord(source);
            if (hasNegativeCycle)
            {
                throw new Exception("El grafo contiene ciclos negativos.");
            }

            // Retorna un diccionario con el destino y su distancia desde el origen
            return destinations.ToDictionary(destination => destination, destination => distances[destination]);
        }

        // Variante de Bellman-Ford para minimizar tiempos de entrega
        public (int[] timeDist, bool hasNegativeCycle) BellmanFordForTime(int source, List<(int u, int v, int time)> timeEdges)
        {
            int[] timeDist = new int[Vertices];
            Array.Fill(timeDist, int.MaxValue);
            timeDist[source] = 0;

            // Relajar todas las aristas |V| - 1 veces
            for (int i = 0; i < Vertices - 1; i++)
            {
                foreach (var (u, v, time) in timeEdges)
                {
                    if (timeDist[u] != int.MaxValue && timeDist[u] + time < timeDist[v])
                    {
                        timeDist[v] = timeDist[u] + time;
                    }
                }
            }

            // Verificar ciclos negativos
            foreach (var (u, v, time) in timeEdges)
            {
                if (timeDist[u] != int.MaxValue && timeDist[u] + time < timeDist[v])
                {
                    // Ciclo negativo detectado
                    return (timeDist, true);
                }
            }

            return (timeDist, false);
        }

    }

}
