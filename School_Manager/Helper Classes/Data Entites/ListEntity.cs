using System.Collections.Generic;

namespace School_Manager
{
    public class ListEntity : TextEntity
    {
        /// <summary>
        /// Items of ComboBox
        /// </summary>
        public List<string> Items { get; set; }

        public bool IsEditable { get; set; }

        public string Text { get; set; }
    }
}
