using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwiz.Core.Contract
{
    /// <summary>
    /// Serializer interface for deserializing a stream into a graph.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Deserialize a stream into a graph.
        /// </summary>
        /// <param name="stream">Stream to be deserialized</param>
        /// <returns>Deserialized graph</returns>
        IGraph Deserialize(Stream stream);

        /// <summary>
        /// Serialize a graph into a stream.
        /// </summary>
        /// <param name="stream">The stream the graph is serialized to</param>
        /// <param name="graph">The serialized graph</param>
        void Serialize(Stream stream, IGraph graph);
    }
}
