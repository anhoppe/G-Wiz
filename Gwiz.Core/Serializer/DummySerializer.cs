using Gwiz.Core.Contract;
using System.IO;

namespace Gwiz.Core.Serializer
{
    public class DummySerializer : Contract.ISerializer
    {
        public IGraph Deserialize(Stream stream)
        {
            return new Graph();
        }
    }
}
