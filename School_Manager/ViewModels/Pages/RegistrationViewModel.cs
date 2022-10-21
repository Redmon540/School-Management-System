using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace School_Manager
{
    public class RegistrationViewModel : BaseViewModel
    {
        #region Constructor

        public RegistrationViewModel()
        {
            //set commands
            TypeChangedCommand = new RelayCommand(TypeChanged);
            KeyChangedCommand = new RelayCommand(KeyChanged);
            RegisterCommand = new RelayCommand(Register);

            TypeChanged();
        }

        #endregion

        #region Properties

        public TextEntity RegistrationKey { get; set; } = new TextEntity { FeildName = "Enter Registration Key" ,  ValidationType = ValidationType.RegistrationKey, };

        public ListEntity Type { get; set; } = new ListEntity { FeildName = "Type", ValidationType = ValidationType.NotEmpty,
            Items = new List<string> { "Monthly", "Half Yearly", "Yearly", "Full Time" }, Value = "Monthly" };

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string RegistrationPeriod { get; set; }

        #endregion

        #region Commands

        public ICommand TypeChangedCommand { get; set; }

        public ICommand KeyChangedCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        #endregion

        #region Command Methods

        private void TypeChanged()
        {
            try
            {
                if (Type.Value == "Monthly")
                {
                    StartDate = DateTime.Now;
                    EndDate = DateTime.Now.AddMonths(1);
                    RegistrationPeriod = $"Product registration will be from {DateTime.Now.ToString("dd-MMMM-yyyy")} to {DateTime.Now.AddMonths(1).ToString("dd-MMMM-yyyy")}";
                }
                else if (Type.Value == "Half Yearly")
                {
                    StartDate = DateTime.Now;
                    EndDate = DateTime.Now.AddMonths(6);
                    RegistrationPeriod = $"Product registration will be from {DateTime.Now.ToString("dd-MMMM-yyyy")} to {DateTime.Now.AddMonths(6).ToString("dd-MMMM-yyyy")}";
                }
                else if (Type.Value == "Yearly")
                {
                    StartDate = DateTime.Now;
                    EndDate = DateTime.Now.AddMonths(12);
                    RegistrationPeriod = $"Product registration will be from {DateTime.Now.ToString("dd-MMMM-yyyy")} To {DateTime.Now.AddMonths(12).ToString("dd-MMMM-yyyy")}";
                }
                else if (Type.Value == "Full Time")
                {
                    RegistrationPeriod = $"Product will be registered Full Time";
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
            
        }

        private void KeyChanged()
        {
            ////to not add - when deleting charachters
            //if (KeyCharCount > RegistrationKey.Value.Length)
            //    return;

            //if (RegistrationKey.Value.Length == 5)
            //    RegistrationKey.Value += "-";
            //else if (RegistrationKey.Value.Length == 10)
            //    RegistrationKey.Value += "-";
            //else if (RegistrationKey.Value.Length == 15)
            //    RegistrationKey.Value += "-";
            //else if (RegistrationKey.Value.Length == 20)
            //    RegistrationKey.Value += "-";

            //KeyCharCount = RegistrationKey.Value.Length;
        }

        protected virtual void Register()
        {
            try
            {
                if (!RegistrationKey.IsValid || !Type.IsValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }
                RegistrationType type = RegistrationType.Monthly;
                if (Type.Value == "Half Yearly")
                    type = RegistrationType.HalfYearly;
                else if (Type.Value == "Yearly")
                    type = RegistrationType.Yearly;
                else if (Type.Value == "Full Time")
                    type = RegistrationType.FullTime;

                if (DataAccess.Register(RegistrationKey.Value, type, StartDate, EndDate))
                    DialogManager.ShowMessageDialog("Message", "Product has been registered successfully.",DialogTitleColor.Green);
                else
                    DialogManager.ShowMessageDialog("Message", "Product registration failed.",DialogTitleColor.Red);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
           
        }

        #endregion
    }
}
