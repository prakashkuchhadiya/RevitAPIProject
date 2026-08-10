using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAPIProject.Models
{
    public class CategoryBind : ObservableObject
    {
        private bool _isChecked;
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (SetProperty(ref _isChecked, value))
                {
                    // Automatically sync children whenever category selection changes
                    OnIsCheckedChanged(value);
                }
            }
        }

        public List<ElementTypeBind> ElementTypesBind { get; set; } = new();

        private void OnIsCheckedChanged(bool isChecked)
        {
            if (ElementTypesBind != null)
            {
                foreach (var elementType in ElementTypesBind)
                {
                    elementType.IsChecked = isChecked;
                }
            }
        }
    }
}