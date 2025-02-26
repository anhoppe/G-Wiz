using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using Microsoft.UI.Xaml.Controls;
using Prism.Commands;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.IO;

namespace Gwiz.App.GraphTest
{
    internal class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        public IList<IEdge> Edges { get; private set; } = new List<IEdge>();

        public IList<INode> Nodes { get; private set; } = new List<INode>();

        internal async void LoadGraph(StorageFile file)
        {
            YamlSerializer serializer = new();
            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                var graph = serializer.Deserialize(fileStream);
                Edges = graph.Edges;
                Nodes = graph.Nodes;
            }

            RaisePropertyChanged(nameof(Nodes));
            RaisePropertyChanged(nameof(Edges));
        }
    }
}
