using System;
using System.Collections;
using System.Windows.Forms;

namespace ARKBreedingStats {
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn { set; get; }

        /// <summary>
        /// Specifies the last column to be sorted (is used for sorting when current compare is equal)
        /// </summary>
        public int LastSortColumn { set; get; }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order { set; get; }
        /// <summary>
        /// Gets or sets the order of the last column sorting (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder LastOrder { set; get; }

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            SortColumn = 0;
            LastSortColumn = 0;

            // Initialize the sort order to 'none'
            Order = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
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
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;
            double c1, c2;

            if (listviewX.SubItems.Count <= SortColumn) SortColumn = 0;

            // Compare the two items
            if ((listviewX.SubItems[SortColumn].Text + listviewY.SubItems[SortColumn].Text).Length == 0)
                compareResult = 0;
            else
            {
                compareResult = double.TryParse(listviewX.SubItems[SortColumn].Text, out c1) && 
                        double.TryParse(listviewY.SubItems[SortColumn].Text, out c2) ? 
                                Math.Sign(c1 - c2) : 
                                ObjectCompare.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);

                // if descending sort is selected, return negative result of compare operation
                if (Order == SortOrder.Descending)
                    compareResult = -compareResult;
            }

            // if comparing is 0 (items equal), use LastColumnToSort
            if (compareResult == 0)
            {
                if (listviewX.SubItems.Count <= LastSortColumn) LastSortColumn = 0;
                // Compare the two items
                // the first two columns are text, the others are int as string
                compareResult = double.TryParse(listviewX.SubItems[LastSortColumn].Text, out c1) && 
                        double.TryParse(listviewY.SubItems[LastSortColumn].Text, out c2) ? 
                                Math.Sign(c1 - c2) : 
                                ObjectCompare.Compare(listviewX.SubItems[LastSortColumn].Text, listviewY.SubItems[LastSortColumn].Text);
                // if descending sort is selected, return negative result of compare operation
                if (LastOrder == SortOrder.Descending)
                    compareResult = -compareResult;
            }

            return Order == SortOrder.Ascending || Order == SortOrder.Descending ? compareResult : 0;
            // Return '0' to indicate they are equal
        }

        public static void doSort(ListView lw, int column)
        {
            ListViewColumnSorter lwcs = (ListViewColumnSorter)lw.ListViewItemSorter;
            // Determine if clicked column is already the column that is being sorted.
            if (column == lwcs.SortColumn)
            {
                // Reverse the current sort direction for this column.
                lwcs.Order = lwcs.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to descending (except the name and owner column).
                lwcs.LastSortColumn = lwcs.SortColumn;
                lwcs.LastOrder = lwcs.Order;
                lwcs.SortColumn = column;
                lwcs.Order = column > 1 ? SortOrder.Descending : SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lw.Sort();
        }

    }
}