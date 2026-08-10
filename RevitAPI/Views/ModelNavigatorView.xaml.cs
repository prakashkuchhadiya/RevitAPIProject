using System.Windows;

namespace RevitAPIProject.Views
{
    /// <summary>
    /// Interaction logic for ModelNavigatorView.xaml
    /// </summary>
    public partial class ModelNavigatorView : Window
    {

        public static ModelNavigatorView Instance { get; set; }

        public ModelNavigatorView()
        {
            InitializeComponent();
            Instance = this; 
        }
    }
}
