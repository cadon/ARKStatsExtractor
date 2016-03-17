using System.Collections;
using System.Windows.Forms;

/// <summary>
/// This class is an implementation of the 'IComparer' interface.
/// </summary>
public class ListViewColumnSorter : IComparer
{
    /// <summary>
    /// Specifies the column to be sorted
    /// </summary>
    private int ColumnToSort;
    /// <summary>
    /// Specifies the last column to be sorted (is used for sorting when current compare is equal)
    /// </summary>
    private int LastColumnToSort;
    /// <summary>
    /// Specifies the order in which to sort (i.e. 'Ascending').
    /// </summary>
    private SortOrder OrderOfSort;
    /// <summary>
    /// Specifies the order in which the LastColumnToSort is sorted (i.e. 'Ascending').
    /// </summary>
    private SortOrder LastOrderOfSort;
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
        ColumnToSort = 0;
        LastColumnToSort = 0;

        // Initialize the sort order to 'none'
        OrderOfSort = SortOrder.None;

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
        ListViewItem listviewX, listviewY;

        // Cast the objects to be compared to ListViewItem objects
        listviewX = (ListViewItem)x;
        listviewY = (ListViewItem)y;

        // Compare the two items
        // the first two columns are text, the others are int as string
        if (ColumnToSort > 2)
        {
            compareResult = int.Parse(listviewX.SubItems[ColumnToSort].Text) - int.Parse(listviewY.SubItems[ColumnToSort].Text);
        }
        else
        {
            compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
        }

        // if descending sort is selected, return negative result of compare operation
        if (OrderOfSort == SortOrder.Descending)
            compareResult = -compareResult;

        // if comparing is 0 (items equal), use LastColumnToSort
        if (compareResult == 0)
        {
            // Compare the two items
            // the first two columns are text, the others are int as string
            if (LastColumnToSort > 2)
            {
                compareResult = int.Parse(listviewX.SubItems[LastColumnToSort].Text) - int.Parse(listviewY.SubItems[LastColumnToSort].Text);
            }
            else
            {
                compareResult = ObjectCompare.Compare(listviewX.SubItems[LastColumnToSort].Text, listviewY.SubItems[LastColumnToSort].Text);
            }
            // if descending sort is selected, return negative result of compare operation
            if (LastOrderOfSort == SortOrder.Descending)
                compareResult = -compareResult;
        }


        if (OrderOfSort == SortOrder.Ascending || OrderOfSort == SortOrder.Descending)
        {
            return compareResult;
        }
        else
        {
            // Return '0' to indicate they are equal
            return 0;
        }
    }

    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int SortColumn
    {
        set
        {
            ColumnToSort = value;
        }
        get
        {
            return ColumnToSort;
        }
    }

    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int LastSortColumn
    {
        set
        {
            LastColumnToSort = value;
        }
        get
        {
            return LastColumnToSort;
        }
    }

    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order
    {
        set
        {
            OrderOfSort = value;
        }
        get
        {
            return OrderOfSort;
        }
    }
    /// <summary>
    /// Gets or sets the order of the last column sorting (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder LastOrder
    {
        set
        {
            LastOrderOfSort = value;
        }
        get
        {
            return LastOrderOfSort;
        }
    }

}