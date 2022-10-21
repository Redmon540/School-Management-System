using System.Windows.Input;

namespace School_Manager
{
    public class SendMessageViewModel : BaseViewModel
    {
        #region Constructor

        public SendMessageViewModel()
        {
            //set the commands
            SendMessageCommand = new RelayCommand(SendMessage);
        }

        #endregion

        #region Properties

        public string Message { get; set; } = "Testing..........";

        public string Response { get; set; }

        public TextEntity IpAddress { get; set; } = new TextEntity { FeildName = "IP Address", Value = "http://192.168.1.100", IsEnabled = true };

        public TextEntity Port { get; set; } = new TextEntity { FeildName = "Port", Value = "8090", IsEnabled = true };

        public TextEntity UserName { get; set; } = new TextEntity { FeildName = "Username", Value = "abcd", IsEnabled = true };

        public TextEntity Password { get; set; } = new TextEntity { FeildName = "Password", Value = "1234", IsEnabled = true };

        public TextEntity Number { get; set; } = new TextEntity { FeildName = "Enter Number", ValidationType = ValidationType.PhoneNumber,Value = "+92331-2154171" };

        public TextEntity Mask { get; set; } = new TextEntity { FeildName = "Mask Name" , Value = "None" , IsEnabled = false};

        public string Count { get; set; }

        public string MessageCount { get; set; } = "1";

        #endregion

        #region Commands

        public ICommand SendMessageCommand { get; set; }

        #endregion

        #region Command Methods

        private void SendMessage()
        {
            if(!Number.IsValid || Message.IsNullOrEmpty())
            {
                DialogManager.ShowValidationMessage();
                return;
            }
                var phone = Number.Value.Replace("-","").Remove(0,3).Insert(0,"0");
            if (phone.Length == 11)
            {
                int messagecount = int.Parse(MessageCount);
                for (int i = 0; i < messagecount; i++)
                {
                    Response = SMS.SendSmsByMobile(phone, UserName.Value, Password.Value, 
                        IpAddress.Value, Port.Value, Message).ToLower().Contains("success")? 
                        "Message sent":"Message cannot be sent." ;
                }
            }
        }

        #endregion
    }
}
