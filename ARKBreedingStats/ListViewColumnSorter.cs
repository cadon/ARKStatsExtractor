using System;
using System.Collections;
using System.Windows.Forms;

namespace ARKBreedingStats
{
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn { set; get; }

        /// <summary>
        /// Specifies the last column to be sorted (is used for sorting when current compare is equal)
        /// </summary>
        private int _lastSortColumn;

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order { set; get; }

        /// <summary>
        /// Gets or sets the order of the last column sorting (for example, 'Ascending' or 'Descending').
        /// </summary>
        private SortOrder _lastOrder;

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private readonly CaseInsensitiveComparer _objectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            SortColumn = 0;
            _lastSortColumn = 0;

            // Initialize the sort order to 'none'
            Order = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            _objectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;

            // Cast the objects to be compared to ListViewItem objects
            if (!(x is ListViewItem listViewX
                  && y is ListViewItem listViewY))
            {
                return 0;
            }

            double c1, c2;

            if (listViewX.SubItems.Count <= SortColumn) SortColumn = 0;

            // Compare the two items
            if ((listViewX.SubItems[SortColumn].Text + listViewY.SubItems[SortColumn].Text).Length == 0)
                compareResult = 0;
            else
            {
                compareResult = double.TryParse(listViewX.SubItems[SortColumn].Text, out c1) &&
                        double.TryParse(listViewY.SubItems[SortColumn].Text, out c2) ?
                                Math.Sign(c1 - c2) :
                                _objectCompare.Compare(listViewX.SubItems[SortColumn].Text, listViewY.SubItems[SortColumn].Text);

                // if descending sort is selected, return negative result of compare operation
                if (Order == SortOrder.Descending)
                    compareResult = -compareResult;
            }

            // if comparing is 0 (items equal), use LastColumnToSort
            if (compareResult == 0)
            {
                if (listViewX.SubItems.Count <= _lastSortColumn) _lastSortColumn = 0;
                // Compare the two items
                // the first two columns are text, the others are int as string
                compareResult = double.TryParse(listViewX.SubItems[_lastSortColumn].Text, out c1) &&
                        double.TryParse(listViewY.SubItems[_lastSortColumn].Text, out c2) ?
                                Math.Sign(c1 - c2) :
                                _objectCompare.Compare(listViewX.SubItems[_lastSortColumn].Text, listViewY.SubItems[_lastSortColumn].Text);
                // if descending sort is selected, return negative result of compare operation
                if (_lastOrder == SortOrder.Descending)
                    compareResult = -compareResult;
            }

            return Order == SortOrder.Ascending || Order == SortOrder.Descending ? compareResult : 0;
            // Return '0' to indicate they are equal
        }

        public static void DoSort(ListView lw, int column)
        {
            if (!(lw.ListViewItemSorter is ListViewColumnSorter lvcs)) return;
            // Determine if clicked column is already the column that is being sorted.
            if (column == lvcs.SortColumn)
            {
                // Reverse the current sort direction for this column.
                lvcs.Order = lvcs.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to descending (except the name and owner column).
                lvcs._lastSortColumn = lvcs.SortColumn;
                lvcs._lastOrder = lvcs.Order;
                lvcs.SortColumn = column;
                lvcs.Order = column > 1 ? SortOrder.Descending : SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lw.Sort();
        }

    }
}