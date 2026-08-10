using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Document = Autodesk.Revit.DB.Document;

namespace RevitAPIProject.Utiles
{
    public static class DocumentUtiles 
    {
        public static List<Element> GetElementsTypeof(this Document document, Type type)
        {
            FilteredElementCollector elements1 = new FilteredElementCollector(document);
            return elements1.OfClass(type).ToElements().ToList();
        }
    }
}
