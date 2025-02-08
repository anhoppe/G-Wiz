using Gwiz.Core.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Gwiz.Core.Serializer
{
    public class YamlSerializer : Contract.ISerializer
    {
        public Graph Deserialize(Stream stream)
        {
            Graph graph = new Graph();

            using (var reader = new StreamReader(stream))
            {
                string yaml = reader.ReadToEnd();

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(PascalCaseNamingConvention.Instance)
                    .Build();

                graph = deserializer.Deserialize<Graph>(yaml);
            }

            return graph;
        }
    }
}
