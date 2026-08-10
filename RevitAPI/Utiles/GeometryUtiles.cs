using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Autodesk.Revit.DB.Point;

namespace RevitAPIProject.Utiles
{
    public static class GeometryUtiles
    {
        public static Point ToPoint(this XYZ xYZ)
        {
            return Point.Create(xYZ);
        }

        public static void ShowGeometry(this List<GeometryObject> geometryObjects, Document document, bool withTransaction)
        {
            if (withTransaction)
            {
                using (Transaction transaction = new Transaction(document, "Testing"))
                {
                    transaction.Start();

                    DirectShape directShape = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_GenericModel));
                    directShape.SetShape(geometryObjects);

                    transaction.Commit();
                }
            }
            else
            {
                DirectShape directShape = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_GenericModel));
                directShape.SetShape(geometryObjects);
            }
        }

        public static Solid GetGeometry(this Element element)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            options.View = element.Document.ActiveView;
            GeometryElement geometryElement = element.get_Geometry(options);
            List<Solid> solids1 = geometryElement.OfType<Solid>().ToList();
            List<Solid> solids2 = geometryElement.OfType<GeometryInstance>().Select(g => g.GetInstanceGeometry()).SelectMany(g => g.OfType<Solid>()).ToList();
            List<Solid> solids = solids1.Concat(solids2).Where(s => s.Volume != 0).ToList();

            Solid combinedSolid = null;
            foreach(Solid solid in solids)
            {
                if (combinedSolid == null)
                {
                    combinedSolid = solid;
                }
                else
                {
                    combinedSolid = BooleanOperationsUtils.ExecuteBooleanOperation(combinedSolid, solid, BooleanOperationsType.Union);
                }
            }

            return combinedSolid;
        }
    }
}
