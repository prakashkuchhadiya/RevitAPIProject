using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace RevitAPIProject.Application
{
    public class App : IExternalApplication
    {
        public string RibbonTabName { get => "RevitAPI"; }
        public string RibbonPanelName { get => "RevitAPIPanel"; }

        public Result OnStartup(UIControlledApplication application)
        {
            // 1. Create Custom Ribbon Tab
            application.CreateRibbonTab(RibbonTabName);
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(RibbonTabName, RibbonPanelName);

            PushButtonData pushButtonData1 = new PushButtonData("ModelNavigatorCommand", "Model\nNavigator", 
                                        Assembly.GetExecutingAssembly().Location, "RevitAPIProject.Command.ModelNavigatorCommand");
            BitmapImage bitmapImage1 = GetEmbeddedImage("RevitAPIProject.Resources.icon32.png");
            pushButtonData1.LargeImage = bitmapImage1;
            ribbonPanel.AddItem(pushButtonData1);   

            ribbonPanel.AddSeparator();

            PushButtonData pushButtonData2 = new PushButtonData("RoomWizard", "Room\nWizard",
                                        Assembly.GetExecutingAssembly().Location, "RevitAPIProject.Command.RoomWizardCommand");
            BitmapImage bitmapImage2 = GetEmbeddedImage("RevitAPIProject.Resources.icon32.png");
            pushButtonData2.LargeImage = bitmapImage2;
            ribbonPanel.AddItem(pushButtonData2);

            //Assembly.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\MaterialDesignThemes.Wpf.dll");
            Assembly.Load("MaterialDesignThemes.Wpf");

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        // Helper Method to Load Embedded Resource Images for Icons
        private BitmapImage GetEmbeddedImage(string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return null;

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    return image;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}