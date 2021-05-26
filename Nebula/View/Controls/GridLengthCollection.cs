using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Nebula.View.Converters;

namespace Nebula.View.Controls
{
    [TypeConverter(typeof(GridLengthCollectionConverter))]
    public class GridLengthCollection : ReadOnlyCollection<GridLength>
    {
        public GridLengthCollection(IList<GridLength> list) : base(list)
        {
            
        }
    }
}