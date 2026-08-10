using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using RevitAPIProject.Models;
using RevitAPIProject.Views;
using System.Windows.Media;

namespace RevitAPIProject.ViewModels
{
    public class ModelNavigatorViewModel : ObservableObject
    {
        private readonly Document _document;
        private readonly UIDocument _uiDoc;

        private readonly ExternalEvent _externalEvent;
        private readonly RevitExternalEventHandler _eventHandler;

        private System.Windows.Media.Color _selectedColor = System.Windows.Media.Colors.Red; // Default color

        public System.Windows.Media.Color SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        public IRelayCommand OverrideColorInRevitCommand { get; }

        private ObservableCollection<CategoryBind> _categoryBinds = new();
        private ObservableCollection<ElementTypeBind> _elementTypeBinds = new();
        private CategoryBind? _selectedCategoryBind;
        private bool? _allCategoriesChecked = false;

        // Command for the Select button
        public IRelayCommand SelectInRevitCommand { get; }
        public IRelayCommand IsolateInRevitCommand { get; }
        public IRelayCommand HideInRevitCommand { get; }
        public IRelayCommand DeleteInRevitCommand { get; }

        public ModelNavigatorViewModel(ExternalEvent externalEvent, RevitExternalEventHandler eventHandler)
        {
            _externalEvent = externalEvent;
            _eventHandler = eventHandler;

            SelectInRevitCommand = new RelayCommand(() => QueueRevitAction(SelectElementsInRevit));
            IsolateInRevitCommand = new RelayCommand(() => QueueRevitAction(IsolateElementsInRevit));
            HideInRevitCommand = new RelayCommand(() => QueueRevitAction(HideElementsInRevit));
            DeleteInRevitCommand = new RelayCommand(() => QueueRevitAction(DeleteElementsInRevit));
            OverrideColorInRevitCommand = new RelayCommand(() => QueueRevitAction(OverrideColorInRevit));
        }        

        //// Inject the active Revit UIDocument into the ViewModel constructor
        //public ModelNavigatorViewModel(Document doc, UIDocument uiDoc)
        //{
        //    _uiDoc = uiDoc;
        //    _document = doc ?? uiDoc?.Document!; // Safe fallback assign

        //    // Initialize Commands
        //    SelectInRevitCommand = new RelayCommand(SelectElementsInRevit);
        //    IsolateInRevitCommand = new RelayCommand(IsolateElementsInRevit);
        //    HideInRevitCommand = new RelayCommand(HideElementsInRevit);
        //    DeleteInRevitCommand = new RelayCommand(DeleteElementsInRevit);
        //    OverrideColorInRevitCommand = new RelayCommand(OverrideColorInRevit);
        //}

        public ObservableCollection<CategoryBind> CategoryBinds
        {
            get => _categoryBinds;
            set => SetProperty(ref _categoryBinds, value);
        }

        public ObservableCollection<ElementTypeBind> ElementTypeBinds
        {
            get => _elementTypeBinds;
            set => SetProperty(ref _elementTypeBinds, value);
        }

        public CategoryBind? SelectedCategoryBind
        {
            get => _selectedCategoryBind;
            set
            {
                if (SetProperty(ref _selectedCategoryBind, value))
                {
                    ElementTypeBinds = value?.ElementTypesBind != null
                        ? new ObservableCollection<ElementTypeBind>(value.ElementTypesBind)
                        : new ObservableCollection<ElementTypeBind>();
                }
            }
        }

        public bool? AllCategoriesChecked
        {
            get => _allCategoriesChecked;
            set
            {
                if (SetProperty(ref _allCategoriesChecked, value) && value.HasValue)
                {
                    ToggleAllCategories(value.Value);
                }
            }
        }

        private void ToggleAllCategories(bool checkAll)
        {
            foreach (var category in CategoryBinds)
            {
                category.IsChecked = checkAll;
            }
        }

        // Helper to queue actions safely for modeless execution
        private void QueueRevitAction(Action<UIApplication> action)
        {
            _eventHandler.SetAction(action);
            _externalEvent.Raise();
        }

        private List<ElementId> GetCheckedElementIds()
        {
            return CategoryBinds
                .SelectMany(cat => cat.ElementTypesBind)
                .Where(eType => eType.IsChecked)
                .SelectMany(eType => eType.Elements)
                .Select(e => e.Id)
                .Distinct()
                .ToList();
        }

        // Logic to select elements in Revit
        private void SelectElementsInRevit(UIApplication app)
        {
            UIDocument uiDoc = app.ActiveUIDocument;
            var ids = GetCheckedElementIds();
            if (ids.Any() && uiDoc != null)
            {
                uiDoc.Selection.SetElementIds(ids);
            }
        }

        private void IsolateElementsInRevit(UIApplication app)
        {
            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = uiDoc.Document;
            var ids = GetCheckedElementIds();
            if (!ids.Any()) return;

            using (Transaction trans = new Transaction(doc, "Isolate Elements"))
            {
                trans.Start();
                uiDoc.ActiveView.IsolateElementsTemporary(ids);
                trans.Commit();
            }
        }

        private void HideElementsInRevit(UIApplication app)
        {
            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = uiDoc.Document;
            var ids = GetCheckedElementIds();
            if (!ids.Any()) return;

            using (Transaction trans = new Transaction(doc, "Hide Elements"))
            {
                trans.Start();
                uiDoc.ActiveView.HideElements(ids);
                trans.Commit();
            }
        }

        private void DeleteElementsInRevit(UIApplication app)
        {
            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = uiDoc.Document;
            var ids = GetCheckedElementIds();
            if (!ids.Any()) return;

            using (Transaction trans = new Transaction(doc, "Delete Elements"))
            {
                trans.Start();
                doc.Delete(ids);
                trans.Commit();
            }
        }

        private void OverrideColorInRevit(UIApplication app)
        {
            UIDocument uiDoc = app.ActiveUIDocument;
            Document doc = uiDoc.Document;
            var ids = GetCheckedElementIds();
            if (!ids.Any()) return;

            Autodesk.Revit.DB.Color revitColor = new Autodesk.Revit.DB.Color(SelectedColor.R, SelectedColor.G, SelectedColor.B);

            FillPatternElement solidFillPattern = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement))
                .Cast<FillPatternElement>()
                .FirstOrDefault(fp => fp.GetFillPattern().IsSolidFill);

            if (solidFillPattern == null) return;

            OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();
            overrideSettings.SetSurfaceForegroundPatternId(solidFillPattern.Id);
            overrideSettings.SetSurfaceForegroundPatternColor(revitColor);
            overrideSettings.SetCutForegroundPatternId(solidFillPattern.Id);
            overrideSettings.SetCutForegroundPatternColor(revitColor);

            using (Transaction trans = new Transaction(doc, "Override Color"))
            {
                trans.Start();
                foreach (ElementId id in ids)
                {
                    uiDoc.ActiveView.SetElementOverrides(id, overrideSettings);
                }
                trans.Commit();
            }
        }
    }
}