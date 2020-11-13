using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace ArcObjectX.DataManagement.Tool
{
    public class ArcObjectExportTool<T>
    {
        private Dictionary<string, string> _DicMapping = null;

        public ArcObjectExportTool<T> MappingField(Expression<Func<T , object>> expression , string _DesFieldName)
        {
            return this;
        }

        public void Insert()
        {
            throw new NotImplementedException();
        }
    }
}
