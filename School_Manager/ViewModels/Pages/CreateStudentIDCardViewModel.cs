using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class CreateStudentIDCardViewModel : BaseViewModel
    {

        #region Constructor

        public CreateStudentIDCardViewModel()
        {
            //set the commands
            CreateCardCommand = new RelayCommand(CreateCards);

            //set the commands
            SelectLogoCommand = new RelayCommand(SelectLogo);

            //set the properties
            IDCards = new List<UserControl>()
            {
                new IDCardThumbnail(){ DataContext = new IDCardThumbnailViewModel()
                {
                    CardFront = new Student_Card_1_Front(){DataContext = new IDCardDummyViewModel()},
                    CardBack = new Student_Card_1_Back(){DataContext = new IDCardDummyViewModel()}
                } },
                new IDCardThumbnail(){ DataContext = new IDCardThumbnailViewModel()
                {
                    CardFront = new Student_Card_1_Front(){DataContext = new IDCardDummyViewModel()},
                    CardBack = new Student_Card_1_Back(){DataContext = new IDCardDummyViewModel()}
                } },
                new IDCardThumbnail(){ DataContext = new IDCardThumbnailViewModel()
                {
                    CardFront = new Student_Card_1_Front(){DataContext = new IDCardDummyViewModel()},
                    CardBack = new Student_Card_1_Back(){DataContext = new IDCardDummyViewModel()}
                } },
                new IDCardThumbnail(){ DataContext = new IDCardThumbnailViewModel()
                {
                    CardFront = new Student_Card_1_Front(){DataContext = new IDCardDummyViewModel()},
                    CardBack = new Student_Card_1_Back(){DataContext = new IDCardDummyViewModel()}
                } },
                new IDCardThumbnail(){ DataContext = new IDCardThumbnailViewModel()
                {
                    CardFront = new Student_Card_1_Front(){DataContext = new IDCardDummyViewModel()},
                    CardBack = new Student_Card_1_Back(){DataContext = new IDCardDummyViewModel()}
                } },
                new IDCardThumbnail(){ DataContext = new IDCardThumbnailViewModel()
                {
                    CardFront = new Student_Card_1_Front(){DataContext = new IDCardDummyViewModel()},
                    CardBack = new Student_Card_1_Back(){DataContext = new IDCardDummyViewModel()}
                } },
                new IDCardThumbnail(){ DataContext = new IDCardThumbnailViewModel()
                {
                    CardFront = new Student_Card_1_Front(){DataContext = new IDCardDummyViewModel()},
                    CardBack = new Student_Card_1_Back(){DataContext = new IDCardDummyViewModel()}
                } },
                new IDCardThumbnail(){ DataContext = new IDCardThumbnailViewModel()
                {
                    CardFront = new Student_Card_1_Front(){DataContext = new IDCardDummyViewModel()},
                    CardBack = new Student_Card_1_Back(){DataContext = new IDCardDummyViewModel()}
                } },
            };
            IDCardViewModel = new IDCardViewModel();
            StudentIDName = DataAccess.GetStudentID();
            IDCardViewModel.IDName = StudentIDName;
            CardCreateOptions = new List<string>() { "All Students" , "Single Student" };
            CardCreateOptions.AddRange(DataAccess.GetClassNames());

            CardFront = new UserControl();
            CardBack = new UserControl();
            
        }

        #endregion

        #region Private Members

        private UserControl _SelectedIDCard { get; set; }

        private string _StudentID { get; set; }

        private string _SelectedCardCreateOption { get; set; }

        #endregion

        #region Public Properties

        public List<UserControl> IDCards { get; set; }

        public IDCardViewModel IDCardViewModel { get; set; }

        public UserControl SelectedCard
        {
            get => _SelectedIDCard;
            set
            {
                if (_SelectedIDCard != value)
                {
                    _SelectedIDCard = value;

                    CardFront = Activator.CreateInstance(Type.GetType(((_SelectedIDCard as IDCardThumbnail).DataContext as IDCardThumbnailViewModel).CardFront.ToString())) as UserControl;
                    CardFront.DataContext = IDCardViewModel;

                    CardBack = Activator.CreateInstance(Type.GetType(((_SelectedIDCard as IDCardThumbnail).DataContext as IDCardThumbnailViewModel).CardBack.ToString())) as UserControl;
                    CardBack.DataContext = IDCardViewModel;
                }
            }
        }

        public UserControl CardFront { get; set; }

        public UserControl CardBack { get; set; }

        public string StudentIDName { get; set; }

        public string StudentID
        {
            get => _StudentID;
            set
            {
                if(_StudentID != value)
                {
                    _StudentID = value;
                    if (_StudentID.IsNotNullOrEmpty())
                    {
                        DataTable table = DataAccess.GetDataTable($"SELECT [Photo] , [Name] , [Father Name] , [Class] , [Section] FROM Students " +
                                                       $"LEFT JOIN Parents ON Parents.Parent_ID = Students.Parent_ID " +
                                                       $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                                                       $"LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID " +
                                                       $"WHERE Student_ID = {_StudentID}");
                        if (table.Rows.Count != 0)
                        {

                            IDCardViewModel.IDName = StudentIDName;
                            IDCardViewModel.ID = _StudentID;
                            IDCardViewModel.Name = table.Rows[0]["Name"] as string;
                            IDCardViewModel.FatherName = table.Rows[0]["Father Name"] as string;
                            IDCardViewModel.Class = $"{table.Rows[0]["Class"]} - {table.Rows[0]["Section"]}";
                            IDCardViewModel.Photo = Helper.ByteArrayToImage(table.Rows[0]["Photo"] as byte[]);
                            IDCardViewModel.QRCode = Helper.GetQRCode($"STD-{_StudentID}");

                            CardFront.DataContext = IDCardViewModel;
                            CardBack.DataContext = IDCardViewModel;
                        }
                    }
                    else
                    {
                        IDCardViewModel = new IDCardViewModel();
                        IDCardViewModel.QRCode = Helper.GetQRCode($"STD-{_StudentID}");
                        IDCardViewModel.IDName = StudentIDName;
                        CardFront.DataContext = IDCardViewModel;
                        CardBack.DataContext = IDCardViewModel;
                    }
                }
            }
        }

        public List<string> CardCreateOptions { get; set; }

        public string SelectedCardCreateOption
        {
            get => _SelectedCardCreateOption;
            set
            {
                if(_SelectedCardCreateOption != value)
                {
                    _SelectedCardCreateOption = value;
                    if (_SelectedCardCreateOption == "Single Student")
                        IsVisible = true;
                    else
                        IsVisible = false;
                }
            }
        }

        public bool IsVisible { get; set; }

        #endregion

        #region Commands

        public ICommand CreateCardCommand { get; set; }

        public ICommand SelectLogoCommand { get; set; }

        #endregion

        #region Command Methods

        private void CreateCards()
        {
            var saveFileDialog = new SaveFileDialog();
            if(saveFileDialog.ShowDialog() == true)
            {
                if(SelectedCardCreateOption == "Single Student")
                    DataAccess.CreateIDCards(((_SelectedIDCard as IDCardThumbnail).DataContext as IDCardThumbnailViewModel).CardFront.ToString(),
                        ((_SelectedIDCard as IDCardThumbnail).DataContext as IDCardThumbnailViewModel).CardBack.ToString(),
                        IDCardViewModel,saveFileDialog.FileName );
                else if(SelectedCardCreateOption == "All Students")
                {
                    DataAccess.CreateIDCards(((_SelectedIDCard as IDCardThumbnail).DataContext as IDCardThumbnailViewModel).CardFront.ToString(),
                        ((_SelectedIDCard as IDCardThumbnail).DataContext as IDCardThumbnailViewModel).CardBack.ToString(),
                        IDCardViewModel, string.Empty , saveFileDialog.FileName);
                }
                else
                {
                    DataAccess.CreateIDCards(((_SelectedIDCard as IDCardThumbnail).DataContext as IDCardThumbnailViewModel).CardFront.ToString(),
                        ((_SelectedIDCard as IDCardThumbnail).DataContext as IDCardThumbnailViewModel).CardBack.ToString(),
                        IDCardViewModel, DataAccess.GetClassID(SelectedCardCreateOption), saveFileDialog.FileName);
                }
            }
        }

        private void SelectLogo()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                IDCardViewModel.SchoolLogo = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        #endregion
    }
}
