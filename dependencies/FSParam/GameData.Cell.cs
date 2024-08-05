using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FSParam.Param;

namespace FSParam;
    public partial class Param : SoulsFile<Param>
    {
        /// <summary>
        /// Minimal handle of a cell in a row that contains enough to mutate the value of the cell and created
        /// on demand
        /// </summary>
        public struct Cell
        {
            private Row _row;
            private Column _column;

            internal Cell(Row row, Column column)
            {
                _row = row;
                _column = column;
            }

            public object Value
            {
                get => _column.GetValue(_row);
                set => _column.SetValue(_row, value);
            }

            public void SetValue(object value)
            {
                _column.SetValue(_row, value);
            }
            public PARAMDEF.Field Def => _column.Def;
        }
    }
