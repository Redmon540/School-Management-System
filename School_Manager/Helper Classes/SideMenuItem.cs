using System.Collections.ObjectModel;
using System.Linq;

namespace School_Manager
{
    /// <summary>
    /// Main item for the side menu
    /// </summary>
    public class SideMenuItem : BasePropertyChanged
    {
       
        /// <summary>
        /// font icon for the main item
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Main Content (text) of the item
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// represents the list of side menu sub items
        /// </summary>
        public ObservableCollection<SideMenuSubItems> Items { get; set; }

        /// <summary>
        /// indicates if the item is currently selected
        /// </summary>
        public bool IsSelected { get { return Items.Select(s => s).Where(x => x.IsSelected == true).ToList().Count > 0 ? true : false; } set { IsSelected = value; } }

        /// <summary>
        /// true if the sub items panel is open
        /// </summary>
        public bool IsOpen { get; set; } = false;

        /// <summary>
        /// indicates if the item can expand
        /// </summary>
        public bool CanOpen { get; set; } = true;
    }
}
