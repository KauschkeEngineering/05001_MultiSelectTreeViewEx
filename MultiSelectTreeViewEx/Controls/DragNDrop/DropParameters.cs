
using System.Windows;
namespace System.Windows.Controls
{
    public class DropParameters
    {
        public readonly MultiSelectTreeViewExItem DropToItem;
        public readonly IDataObject Data;
        public readonly int Index;

        public DropParameters(MultiSelectTreeViewExItem dropToItem, IDataObject data): this(dropToItem, data, -1)
        {
        }

        public DropParameters(MultiSelectTreeViewExItem dropToItem, IDataObject data, int index)
        {
            this.DropToItem = dropToItem;
            this.Data = data;
            this.Index = index;
        }
                
    }
}
