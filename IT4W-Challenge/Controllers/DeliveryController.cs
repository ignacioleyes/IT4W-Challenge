using IT4W_Challenge.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace IT4W_Challenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController: ControllerBase
    {
        private readonly Graph _graph;

        public DeliveryController()
        {
            // Inicializa el grafo
            _graph = new Graph(5);
            _graph.AddEdge(0, 1, 5);
            _graph.AddEdge(0, 2, 10);
            _graph.AddEdge(1, 2, 2);
            _graph.AddEdge(1, 3, 4);
            _graph.AddEdge(1, 4, 6);
            _graph.AddEdge(3, 2, 3);
            _graph.AddEdge(3, 1, 2);
            _graph.AddEdge(4, 3, 1);

            // Configura aristas con tiempos
            _graph.AddTimeEdge(0, 1, 5);
            _graph.AddTimeEdge(0, 2, 10);
            _graph.AddTimeEdge(1, 2, 2);
            _graph.AddTimeEdge(1, 3, 4);
            _graph.AddTimeEdge(1, 4, 6);
            _graph.AddTimeEdge(3, 2, 3);
            _graph.AddTimeEdge(3, 1, 2);
            _graph.AddTimeEdge(4, 3, 1);
        }

        /// <summary>
        /// Get the Shortest Path
        /// </summary>
        /// <param name="source">The starting point of the path</param>
        /// <param name="destinations">A list of destination nodes</param>
        /// <returns>Shortest path from the source to the destinations</returns>
        [HttpGet("shortest-path")]
        [ProducesResponseType(typeof(Dictionary<int, int>), StatusCodes.Status200OK)]  // Success response example
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // Error response
        public IActionResult GetShortestPath(int source, [FromQuery] List<int> destinations)
        {
            try
            {
                var result = _graph.FindMultipleDestinations(source, destinations);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the Shortest Time
        /// </summary>
        /// <param name="source">The starting point of the path</param>
        /// <param name="destinations">A list of destination nodes</param>
        /// <returns>Shortest delivery time to the destinations</returns>
        [HttpGet("shortest-time")]
        [ProducesResponseType(typeof(Dictionary<int, int>), StatusCodes.Status200OK)]  // Success response
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // Error response
        public IActionResult GetShortestTime(int source, [FromQuery] List<int> destinations)
        {
            try
            {
                // Llamar al método BellmanFordForTime para calcular los tiempos más cortos
                var (timeDist, hasNegativeCycle) = _graph.BellmanFordForTime(source, _graph.TimeEdges);

                // Verificar si hay ciclos negativos
                if (hasNegativeCycle)
                {
                    return BadRequest("El grafo contiene ciclos negativos en los tiempos de entrega.");
                }

                // Crear un diccionario con los destinos y los tiempos mínimos
                var result = destinations.ToDictionary(
                    destination => destination,
                    destination => timeDist[destination]
                );

                return Ok(result); // Devolver los tiempos mínimos hacia los destinos
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Manejar cualquier error
            }
        }
    }
}
