using Gwiz.Core.Contract;
using Gwiz.Core.Serializer;
using Windows.Storage;
using System.IO;

namespace Gwiz.App.GraphTest
{
    internal class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        public IGraph? Graph { get; private set; }

        internal async void LoadGraph(StorageFile file)
        {
            YamlSerializer serializer = new();
            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                Graph = serializer.Deserialize(fileStream);
            }

            RaisePropertyChanged(nameof(Graph));
        }
    }
}
