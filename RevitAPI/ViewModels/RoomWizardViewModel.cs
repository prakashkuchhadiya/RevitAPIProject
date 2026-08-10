using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RevitAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIProject.ViewModels
{
    public class RoomWizardViewModel : ObservableObject
    {
        private readonly Document _document;
        private readonly UIDocument _uiDoc;

        private readonly ExternalEvent _externalEvent;
        private readonly RevitExternalEventHandler _eventHandler;

        private ObservableCollection<RoomBiding> _roomBindings = new();

        public RoomWizardViewModel(ExternalEvent externalEvent, RevitExternalEventHandler eventHandler)
        {
            _externalEvent = externalEvent;
            _eventHandler = eventHandler;
        }

        public ObservableCollection<RoomBiding> RoomBindings
        {
            get => _roomBindings;
            set => SetProperty(ref _roomBindings, value);
        }


        // Helper to queue actions safely for modeless execution
        private void QueueRevitAction(Action<UIApplication> action)
        {
            _eventHandler.SetAction(action);
            _externalEvent.Raise();
        }

    }
}
