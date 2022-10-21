namespace School_Manager
{
    /// <summary>
    /// Side menu sub item that respresents the items of the <see cref="SideMenuItem"/>
    /// </summary>
    public class SideMenuSubItems : BasePropertyChanged
    {
        /// <summary>
        /// the main content (text) of the item
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// idicates if the item is currently selected
        /// </summary>
        public bool IsSelected { get; set; } = false;

        /// <summary>
        /// the page that this item represents
        /// </summary>
        public ApplicationPage Page { get; set; }
    }
}
