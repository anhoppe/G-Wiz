using awiz.Graph.Core;
using System.Collections.Generic;

namespace awiz.App.GraphTest
{
    internal class MainWindowViewModel : Prism.Mvvm.BindableBase
    {
        public MainWindowViewModel() 
        {
            NodeTemplate nt = new NodeTemplate();
            nt.Grid.AddRow("*");
            nt.Grid.AddRow("*");
            
            Nodes = new List<Node>()
            {
                new Node()
                {
                    X = 10,
                    Y = 10,
                },
                new Node()
                {
                    X = 100,
                    Y = 30,
                },
                new Node()
                {
                    X = 50,
                    Y = 80,
                },

            };
        }

        public IList<Node> Nodes { get; }
    }
}
