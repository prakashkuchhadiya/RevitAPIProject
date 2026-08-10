using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIProject.Utiles
{
    public class SelectionFilterUtiles : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Floor)
                return false;
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
