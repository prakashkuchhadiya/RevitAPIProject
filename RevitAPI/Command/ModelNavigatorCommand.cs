using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAPIProject.Models;
using RevitAPIProject.ViewModels;
using RevitAPIProject.Views;

namespace RevitAPIProject.Command
{
    [Transaction(TransactionMode.Manual)]
    public class ModelNavigatorCommand : IExternalCommand
    {
        // Static reference so only one instance of the window opens at a time
        private static ModelNavigatorView? _viewInstance;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // If window is already open, bring it to focus instead of creating a duplicate
            if (_viewInstance != null && _viewInstance.IsLoaded)
            {
                _viewInstance.Focus();
                return Result.Succeeded;
            }

            UIApplication uIApplication = commandData.Application;
            UIDocument uIDocument = uIApplication.ActiveUIDocument;
            Document document = uIDocument.Document;

            // 1. Initialize External Event for Modeless Window
            var handler = new RevitExternalEventHandler();
            var externalEvent = ExternalEvent.Create(handler);

            var modelNavigatorViewModel = new ModelNavigatorViewModel(externalEvent, handler);

            // 2. Load Elements in Active View
            var docElements = new FilteredElementCollector(document, uIDocument.ActiveView.Id)
                .WhereElementIsNotElementType()
                .WherePasses(new ElementIsElementTypeFilter(true))
                .Where(e => e.Category != null
                         && e.Category.CategoryType == CategoryType.Model
                         && document.GetElement(e.GetTypeId()) != null)
                .ToList();

            var categoryGroups = docElements.GroupBy(e => e.Category.Id).ToList();

            foreach (var catGroup in categoryGroups)
            {
                var categoryElements = catGroup.ToList();
                var firstCategory = categoryElements.First().Category;

                CategoryBind categoryBind = new()
                {
                    Name = firstCategory.Name,
                    IsChecked = false,
                };

                var typeGroups = categoryElements.GroupBy(e => e.GetTypeId()).ToList();

                foreach (var typeGroup in typeGroups)
                {
                    ElementType? elementType = document.GetElement(typeGroup.Key) as ElementType;
                    if (elementType == null) continue;

                    ElementTypeBind elementTypeBind = new()
                    {
                        Name = elementType.Name,
                        IsChecked = false,
                        Elements = typeGroup.ToList()
                    };

                    categoryBind.ElementTypesBind.Add(elementTypeBind);
                }

                if (categoryBind.ElementTypesBind.Count > 0)
                {
                    modelNavigatorViewModel.CategoryBinds.Add(categoryBind);
                }
            }

            // 3. Open as Modeless Window (Show instead of ShowDialog)
            _viewInstance = new ModelNavigatorView()
            {
                DataContext = modelNavigatorViewModel
            };

            // Reset static instance when closed
            _viewInstance.Closed += (sender, e) => { _viewInstance = null; };

            _viewInstance.Show();

            return Result.Succeeded;
        }
    }
}