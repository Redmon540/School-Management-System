using System;
using System.Windows.Input;

namespace School_Manager
{
    /// <summary>
    /// A basic command that runs an action
    /// </summary>
    class RelayParameterizedCommand : ICommand
    {
        #region Private Members

        private Action<object> mAction;
        
        #endregion

        #region Public Events
        /// <summary>
        /// The event that fires when the <see cref="CanExecute(object)"/>value has changed 
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor
        public RelayParameterizedCommand(Action<object> action)
        {
            mAction = action;
        }
        #endregion

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mAction(parameter);
        }
    }
}
