#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#endregion

namespace Yea.DataTypes
{
    /// <summary>
    ///     Holds tabular information
    /// </summary>
    public class Table
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ColumnNames">Column names</param>
        public Table(params string[] ColumnNames)
        {
            this.ColumnNames = (string[]) ColumnNames.Clone();
            Rows = new List<Row>();
            ColumnNameHash = new Hashtable();
            int x = 0;
            foreach (var ColumnName in ColumnNames)
            {
                if (!ColumnNameHash.ContainsKey(ColumnName))
                    ColumnNameHash.Add(ColumnName, x++);
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="Reader">Data reader to get the data from</param>
        public Table(IDataReader Reader)
        {
            ColumnNames = new string[Reader.FieldCount];
            for (int x = 0; x < Reader.FieldCount; ++x)
            {
                ColumnNames[x] = Reader.GetName(x);
            }
            ColumnNameHash = new Hashtable();
            int y = 0;
            foreach (var ColumnName in ColumnNames)
            {
                if (!ColumnNameHash.ContainsKey(ColumnName))
                    ColumnNameHash.Add(ColumnName, y++);
            }
            Rows = new List<Row>();
            while (Reader.Read())
            {
                var Values = new object[ColumnNames.Length];
                for (int x = 0; x < Reader.FieldCount; ++x)
                {
                    Values[x] = Reader[x];
                }
                Rows.Add(new Row(ColumnNameHash, ColumnNames, Values));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Column names for the table
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ColumnNames { get; protected set; }

        /// <summary>
        ///     Column Name hash table
        /// </summary>
        public Hashtable ColumnNameHash { get; private set; }

        /// <summary>
        ///     Rows within the table
        /// </summary>
        public ICollection<Row> Rows { get; private set; }

        /// <summary>
        ///     Gets a specific row
        /// </summary>
        /// <param name="RowNumber">Row number</param>
        /// <returns>The row specified</returns>
        public Row this[int RowNumber]
        {
            get { return Rows.Count > RowNumber ? Rows.ElementAt(RowNumber) : null; }
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Adds a row using the objects passed in
        /// </summary>
        /// <param name="Objects">Objects to create the row from</param>
        /// <returns>This</returns>
        public virtual Table AddRow(params object[] Objects)
        {
            Rows.Add(new Row(ColumnNameHash, ColumnNames, Objects));
            return this;
        }

        #endregion
    }

    /// <summary>
    ///     Holds an individual row
    /// </summary>
    public class Row
    {
        #region Constructor

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ColumnNames">Column names</param>
        /// <param name="ColumnValues">Column values</param>
        /// <param name="ColumnNameHash">Column name hash</param>
        public Row(Hashtable ColumnNameHash, string[] ColumnNames, params object[] ColumnValues)
        {
            this.ColumnNameHash = ColumnNameHash;
            this.ColumnNames = ColumnNames;
            this.ColumnValues = (object[]) ColumnValues.Clone();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Column names
        /// </summary>
        public Hashtable ColumnNameHash { get; private set; }

        /// <summary>
        ///     Column names
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] ColumnNames { get; protected set; }

        /// <summary>
        ///     Column values
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public object[] ColumnValues { get; protected set; }

        /// <summary>
        ///     Returns a column based on the column name specified
        /// </summary>
        /// <param name="ColumnName">Column name to search for</param>
        /// <returns>The value specified</returns>
        public object this[string ColumnName]
        {
            get
            {
                var Column = (int) ColumnNameHash[ColumnName]; //.PositionOf(ColumnName);
                if (Column == -1)
                    throw new ArgumentOutOfRangeException(ColumnName + " is not present in the row");
                return this[Column];
            }
        }

        /// <summary>
        ///     Returns a column based on the value specified
        /// </summary>
        /// <param name="Column">Column number</param>
        /// <returns>The value specified</returns>
        public object this[int Column]
        {
            get
            {
                if (Column < 0)
                    throw new ArgumentOutOfRangeException("Column");
                if (ColumnValues.Length <= Column)
                    return null;
                return ColumnValues[Column];
            }
        }

        #endregion
    }
}