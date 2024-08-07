using SoulsFormats;

namespace FSParam;
public partial class Param : SoulsFile<Param>
{
    public class Row
    {
        internal Param Parent;
        public int ID { get; set; }
        public string? Name { get; set; }
        internal uint DataIndex;

        public IEnumerable<Column> Cells => Parent.Cells;

        public IReadOnlyList<Cell> CellHandles
        {
            get
            {
                var cells = new List<Cell>(Cells.Count());
                foreach (var cell in Cells) { cells.Add(new Cell(this, cell)); }
                return cells;
            }
        }

        public PARAMDEF Def => Parent.AppliedParamdef;

        internal Row(int id, string? name, Param parent, uint dataIndex)
        {
            ID = id;
            Name = name;
            Parent = parent;
            DataIndex = dataIndex;
        }

        public Row(int id, string name, Param parent)
        {
            ID = id;
            Name = name;
            Parent = parent;
            DataIndex = parent._paramData.AddZeroedElement();
        }

        public Row(Row clone)
        {
            Parent = clone.Parent;
            ID = clone.ID;
            Name = clone.Name;
            DataIndex = Parent._paramData.AddZeroedElement();
            Parent._paramData.CopyData(DataIndex, clone.DataIndex);
        }

        public Row(Row clone, Param newParent)
        {
            Parent = newParent;
            ID = clone.ID;
            Name = clone.Name;
            DataIndex = Parent._paramData.AddZeroedElement();
            clone.Parent._paramData.CopyData(Parent._paramData, DataIndex, clone.DataIndex);
        }

        public bool DataEquals(Row? other)
        {
            if (other == null) return false;
            if (ID != other.ID) return false;

            return Parent._paramData.DataEquals(other.Parent._paramData, other.DataIndex, DataIndex);
        }

        ~Row() { Parent._paramData.RemoveAt(DataIndex); }

        /// <summary>
        /// Gets a cell handle from a name or throw an exception if the field name is not found
        /// </summary>
        /// <param name="field">The field to look for</param>
        /// <returns>A cell handle for the field</returns>
        /// <exception cref="ArgumentException">Throws if field name doesn't exist</exception>
        public Cell GetCellHandleOrThrow(string field)
        {
            var cell = Cells.FirstOrDefault(cell => cell.Def.InternalName == field);
            if (cell == null)
                throw new ArgumentException();
            return new Cell(this, cell);
        }

        public Cell? this[string field]
        {
            get
            {
                var cell = Cells.FirstOrDefault(cell => cell.Def.InternalName == field);
                return cell != null ? new Cell(this, cell) : null;
            }
        }
        public Cell this[Column field] => new Cell(this, field);
    }
}