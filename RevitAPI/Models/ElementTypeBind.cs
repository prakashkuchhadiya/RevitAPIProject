using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAPIProject.Models
{
    public class ElementTypeBind : ObservableObject
    {
        private bool _isChecked;
        public string Name { get; set; }
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
        public List<Element> Elements { get; set; } = new List<Element>();
    }
}
