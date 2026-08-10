using System;
using Autodesk.Revit.UI;

namespace RevitAPIProject
{
    public class RevitExternalEventHandler : IExternalEventHandler
    {
        private Action<UIApplication>? _action;

        // Set the action to be executed inside Revit thread
        public void SetAction(Action<UIApplication> action)
        {
            _action = action;
        }

        public void Execute(UIApplication app)
        {
            try
            {
                _action?.Invoke(app);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }

        public string GetName() => "ModelNavigatorExternalEvent";
    }
}