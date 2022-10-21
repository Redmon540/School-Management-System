using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for SalarySheet.xaml
    /// </summary>
    public partial class SalarySheet : UserControl
    {
        public SalarySheet()
        {
            InitializeComponent();
        }

        private string teacherID;

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            #region Handle Special Character Column Names

            if(teacherID.IsNullOrEmpty())
            {
                teacherID = DataAccess.GetTeacherID();
            }

            var columnName = (string)e.Column.Header;

            //COPY PASTE CODE :)
            // We'll build a string with escaped characters.
            // The capacity is the length times 2 (for the carets),
            // plus 2 for the square brackets.
            // This is not optimized for multi-character glyphs, like emojis

            var bindingBuilder = new StringBuilder(columnName.Length * 2 + 2);

            bindingBuilder.Append('[');
            foreach (var c in columnName)
            {
                bindingBuilder.Append('^');
                bindingBuilder.Append(c);
            }
            bindingBuilder.Append(']');

            e.Column = new DataGridTextColumn
            {
                Binding = new Binding(bindingBuilder.ToString()),
                Header = e.Column.Header,
                IsReadOnly = e.Column.Header.ToString() == teacherID ? true:false
            };

            #endregion
        }
    }
}
