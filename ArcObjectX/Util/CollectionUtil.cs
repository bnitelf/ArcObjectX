using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcObjectX.Util
{
    public class CollectionUtil
    {
        public static List<List<T>> SplitToParts<T>(List<T> list, int numPerPart)
        {
            List<List<T>> listPart = new List<List<T>>();
            List<T> part;
            int totalPart = (int)Math.Ceiling(((float)list.Count / numPerPart));
            int startIndex = 0;

            for (int i = 1; i <= totalPart; i++)
            {
                if (i < totalPart)
                    part = list.GetRange(startIndex, numPerPart);
                else
                    part = list.GetRange(startIndex, list.Count - startIndex);

                startIndex += numPerPart;

                listPart.Add(part);
            }

            return listPart;
        }

        public static List<IEnumerable<T>> SplitToParts<T>(IEnumerable<T> list, int numPerPart)
        {
            List<IEnumerable<T>> listPart = new List<IEnumerable<T>>();
            IEnumerable<T> part;
            int totalPart = (int)Math.Ceiling(((float)list.Count() / numPerPart));
            int startIndex = 0;

            for (int i = 1; i <= totalPart; i++)
            {
                if (i < totalPart)
                    part = list.ToList().GetRange(startIndex, numPerPart);
                else
                    part = list.ToList().GetRange(startIndex, list.Count() - startIndex);

                startIndex += numPerPart;

                listPart.Add(part);
            }

            return listPart;
        }
    }
}
