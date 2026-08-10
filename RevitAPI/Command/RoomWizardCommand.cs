using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using RevitAPIProject.Models;
using RevitAPIProject.ViewModels;
using RevitAPIProject.Views;
using RApplication = Autodesk.Revit.ApplicationServices;

namespace RevitAPIProject.Command
{
    [Transaction(TransactionMode.Manual)]
    public class RoomWizardCommand : IExternalCommand
    {
        private static RoomWizardView? _viewInstance;
        public static UIApplication UIApplication { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (_viewInstance != null && _viewInstance.IsLoaded)
            {
                _viewInstance.Focus();
                return Result.Succeeded;
            }

            UIApplication uIApplication = commandData.Application;
            UIDocument uIDocument = uIApplication.ActiveUIDocument;
            Document document = uIDocument.Document;

            var handler = new RevitExternalEventHandler();
            var externalEvent = ExternalEvent.Create(handler);

            // FIX: Target OST_Rooms specifically and use .OfType<Room>() safely
            var rooms = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .OfType<Room>()
                .Where(r => r.Area > 0) // Prevents unplaced/unbounded rooms
                .ToList();

            RoomWizardViewModel roomWizardViewModel = new RoomWizardViewModel(externalEvent, handler);

            foreach (var room in rooms)
            {
                RoomBiding roomBiding = new RoomBiding
                {
                    Name = room.Name,
                    Number = room.Number,
                    Area = Math.Round(room.Area, 2),
                    Volume = Math.Round(room.Volume, 2)
                };
                roomWizardViewModel.RoomBindings.Add(roomBiding);
            }

            _viewInstance = new RoomWizardView()
            {
                DataContext = roomWizardViewModel
            };

            _viewInstance.Closed += (sender, e) => { _viewInstance = null; };

            _viewInstance.Show();

            return Result.Succeeded;
        }
    }
}