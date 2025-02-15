using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using Microsoft.UI.Xaml.Controls;
using Prism.Commands;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using System;
using System.IO;
using System.IO.Pipes;

namespace Gwiz.App.GraphTest
{
    internal class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        public IList<Node> Nodes { get; private set; } = new List<Node>();

        internal async void LoadGraph(StorageFile file)
        {
            YamlSerializer serializer = new();
            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                var graph = serializer.Deserialize(fileStream);
                Nodes = graph.Nodes;
            }

            RaisePropertyChanged(nameof(Nodes));
        }
    }
}
