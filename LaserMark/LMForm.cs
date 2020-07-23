using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Api;
using EzdDataL;
using LaserMark.State;
using LaserMark.DataAccess;
using System.Linq;
using PictureControl;
using System.Drawing.Imaging;
using Telerik.WinControls.UI;

namespace LaserMark
{
    public partial class LMForm : XtraForm
    {
        #region Fields

        Preview preview;

        RadWaitingBar waitingBar;

        private string filesPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName) + @"\files\";

        private string iconsPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName) + @"\icon\";

        public List<Tuple<string, StringBuilder>> ezdObjects;

        public static List<Tuple<string, StringBuilder>> updatedEzdObjects;

        private List<UserDataDto> userDataDtos;

        private int currentPEindex = 0;

        #endregion

        private enum UploadType
        {
            image = 1,
            Ezd
        }

        public LMForm()
        {
            InitializeComponent();

            Initial();
        }

        #region events method

        private void DialogUpdateEzd_Click(object sender, EventArgs e)
        {
            if (CurrentEzd.EzdPictureEdit != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(CurrentApiData.Token))
                    {
                        new UpdateEzdDataFromApi(this);
                    }
                    else
                    {
                        ezdObjects = EzdDataControl.ReopositoryEzdFile.GetEzdData();

                        updatedEzdObjects = new List<Tuple<string, StringBuilder>>();

                        new UpdateEzdData(ezdObjects);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                XtraMessageBox.Show("Пожалуйста, выберите ezd файл", "Error", MessageBoxButtons.OK);
            }
        }

        private async void LoginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                this.waitingBar.StartWaiting();

                // Get all events
                var base64HeaderValue = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes($@"{this.loginTextEdit.Text}:{this.passwordTextEdit.Text}"));

                const string Url = @"http://openeventor.ru/api/get_events";

                var task = await Queries.GetAllEventsAsync(Url, base64HeaderValue);

                var result = JsonConvert.DeserializeObject<Eventor>(task);

                this.waitingBar.StopWaiting();

                CustomFlyoutDialog.ShowForm(this, null, new GetEvents(result));

                this.urlTextEdit.Text = $@"http://openeventor.ru/api/event/{CurrentApiData.Token}/get_event";
            }
            catch (Exception ex)
            {
                this.waitingBar.StopWaiting();

                XtraMessageBox.Show("Проверте логин и пароль или нет доступ к апи, проверьте интернет соединения", "Error", MessageBoxButtons.OK);

                if (CurrentEzd.EzdPictureEdit != null)
                {
                    ezdObjects = EzdDataControl.ReopositoryEzdFile.GetEzdData();

                    updatedEzdObjects = new List<Tuple<string, StringBuilder>>();

                    new UpdateEzdData(ezdObjects);
                }
            }
        }

        private void UploadBGBtn_Click(object sender, EventArgs e)
        {
            this.Upload(UploadType.image);
        }

        private void UploadEzdBtn_Click(object sender, EventArgs e)
        {
            this.Upload(UploadType.Ezd);
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (CurrentEzd.BgPictureEdit != null || CurrentEzd.EzdPictureEdit != null)
            {
                if (XtraMessageBox.Show("Вы действительно хотите удалить?", "Сообщения", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (this.preview != null)
                    {
                        this.preview.Close();
                    }

                    if (currentPEindex < 10)
                    {
                        var selectedImage = this.layoutControl1.Controls.OfType<PictureEdit>()
                            .Where(c => c.TabIndex == currentPEindex + 40)
                            .Select(c => c)
                            .First();

                        if (selectedImage.Tag.ToString() == @"filled")
                        {
                            this.bgImageLbl.Text = " ";
                            this.ezdFileLbl.Text = " ";

                            selectedImage.Image = Image.FromFile($@"{iconsPath}plus.png");
                            selectedImage.Cursor = Cursors.Hand;
                            selectedImage.Properties.ReadOnly = false;
                            selectedImage.Tag = "next";

                            this.backgroundCustomPictureEdit.Image = null;
                            this.backgroundCustomPictureEdit.Properties.NullText = " ";

                            this.foregroundCustomPictureEdit.Image = null;
                            this.foregroundCustomPictureEdit.Properties.NullText = " ";

                            DeleteFileIfUpdated();

                            UserDataRepository.DeleteByTabIndex(currentPEindex);
                        }
                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Что удалить?", "Предупреждение", MessageBoxButtons.OK);
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.loginTextEdit.Text) && !string.IsNullOrEmpty(this.passwordTextEdit.Text)
                && !string.IsNullOrEmpty(this.urlTextEdit.Text))
            {
                if (CurrentEzd.BgPictureEdit != null && CurrentEzd.EzdPictureEdit != null)
                {
                    if (currentPEindex < 10)
                    {
                        // current saved
                        var selectedImage = this.layoutControl1.Controls.OfType<PictureEdit>()
                            .Where(c => c.TabIndex == currentPEindex + 40)
                            .Select(c => c)
                            .First();

                        selectedImage.Image = Images.PanelToImage(this.panelControl1);
                        selectedImage.Cursor = Cursors.Hand;
                        selectedImage.Properties.ReadOnly = false;
                        selectedImage.Tag = "filled";

                        if (currentPEindex < 9)
                        {
                            // next plus
                            var selectedNextImage = this.layoutControl1.Controls.OfType<PictureEdit>()
                                .Where(c => c.TabIndex == (currentPEindex + 1) + 40)
                                .Select(c => c)
                                .First();

                            if (selectedNextImage.Properties.ReadOnly)
                            {
                                selectedNextImage.Image = Image.FromFile($@"{iconsPath}plus.png");
                                selectedNextImage.Cursor = Cursors.Hand;
                                selectedNextImage.Properties.ReadOnly = false;
                                selectedNextImage.Tag = "next";
                            }
                        }

                        DeleteFileIfUpdated();

                        SaveImageDB();

                        if (this.preview != null)
                        {
                            this.preview.Close();
                        }

                        // Initial preview
                        this.foregroundCustomPictureEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                        this.preview = new Preview(PictureControl.Images.PanelToImage(this.panelControl1));
                        this.foregroundCustomPictureEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
                        this.preview.StartPosition = FormStartPosition.Manual;

                        this.preview.Show();
                    }
                }
                else
                {
                    XtraMessageBox.Show("Выберите обложку или ezd файл", "Error", MessageBoxButtons.OK);
                }
            }
            else
            {
                XtraMessageBox.Show("Пожалуйста войдите в систему", "Error", MessageBoxButtons.OK);
            }
        }

        private void LMForm_Load(object sender, EventArgs e)
        {
            this.rightPanelControl.Width = ClientRectangle.Width / 100 * 25;

            // Init forground image parent
            this.foregroundCustomPictureEdit.Parent = this.backgroundCustomPictureEdit;

            if (this.backgroundCustomPictureEdit.Image != null && this.foregroundCustomPictureEdit != null)
            {
                CurrentEzd.BgPictureEdit = this.backgroundCustomPictureEdit;

                CurrentEzd.EzdPictureEdit = this.foregroundCustomPictureEdit;

                // Initial preview
                this.preview = new Preview(PictureControl.Images.PanelToImage(this.panelControl1));

                this.preview.StartPosition = FormStartPosition.Manual;

                this.preview.Show();
            }

            CurrentUIData.RightLayoutControl = this.rightPanelControl;

            CurrentUIData.WindowSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

            CurrentUIData.RightPanelSize = new Size(this.rightPanelControl.Width, ClientRectangle.Height);

            try
            {
                EzdDataL.Load data = new EzdDataL.Load();

                if (!data.Go())
                {
                    if (new CustomMessage().ShowDialog() >= 0)
                    {
                        Application.Exit();
                    }

                    Application.Exit();
                }
            }
            catch (Exception)
            {
                Application.Exit();
            }
        }

        private void PictureEdit_Click(object sender, EventArgs e)
        {
            var picture = ((PictureEdit)sender);

            if (!picture.Properties.ReadOnly)
            {
                currentPEindex = picture.TabIndex - 40;

                var userDto = UserDataRepository.GetByTabIndex(currentPEindex);

                if (userDto.Token != null)
                {
                    UpdateImageFromDB(userDto);
                }
                else
                {
                    // delete
                }
            }
        }

        #endregion

        private void Initial()
        {
            // Init waiting bar
            waitingBar = new RadWaitingBar();
            waitingBar.AssociatedControl = this.layoutControl1;
            waitingBar.Size = new System.Drawing.Size(80, 80);
            waitingBar.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.LineRing;

            this.layoutControl1.Controls.Add(waitingBar);

            try
            {
                // Connect sdk
                var err = JczLmc.Initialize(Application.StartupPath, true);

                userDataDtos = UserDataRepository.GetAllUser();

                if (userDataDtos.Count > 0)
                {

                    for (int i = 0; i < userDataDtos.Count; i++)
                    {
                        var image = this.layoutControl1.Controls.OfType<PictureEdit>()
                            .Where(c => c.TabIndex == i + 40)
                            .Select(c => c)
                            .First();

                        image.Image = Image.FromFile($@"{filesPath}{userDataDtos[i].FullImage}");
                        image.Properties.ReadOnly = false;
                        image.Cursor = Cursors.Hand;
                        image.Tag = @"filled";
                        currentPEindex = i;
                        if (i + 1 == userDataDtos.Count)
                        {
                            // last plus
                            var lastImage = this.layoutControl1.Controls.OfType<PictureEdit>()
                                .Where(c => c.TabIndex == (i + 1) + 40)
                                .Select(c => c)
                                .First();

                            lastImage.Image = Image.FromFile($@"{iconsPath}plus.png");
                            lastImage.Cursor = Cursors.Hand;
                            lastImage.Properties.ReadOnly = false;
                            lastImage.Tag = @"next";
                        }
                    }

                    var currentData = userDataDtos.LastOrDefault();

                    UpdateImageFromDB(currentData);

                    this.loginTextEdit.Text = currentData.Login;

                    this.currentPEindex = (int)currentData.Sequence;

                    this.passwordTextEdit.Text = currentData.Password;

                    this.urlTextEdit.Text = currentData.Url;

                    CurrentApiData.Token = currentData.Token;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void UpdateImageFromDB(UserDataDto currentData)
        {
            var bg_image = Image.FromFile($@"{filesPath}{currentData.BgImage}");
            this.backgroundCustomPictureEdit.Image = bg_image;
            this.backgroundCustomPictureEdit.Width = bg_image.Width;
            this.backgroundCustomPictureEdit.Height = bg_image.Height;
            this.backgroundCustomPictureEdit.Properties.NullText = currentData.BgImage;
            this.backgroundCustomPictureEdit.Location = new Point((int)currentData.BgImagePosX, (int)currentData.BgImagePosY);

            CurrentEzd.BgPictureEdit = this.backgroundCustomPictureEdit;

            var fgImg = EzdDataControl.ReopositoryEzdFile.LoadImage($@"{filesPath}{currentData.EzdImage}",
                                this.foregroundCustomPictureEdit.Width,
                                this.foregroundCustomPictureEdit.Height);
            this.foregroundCustomPictureEdit.Image = fgImg;
            this.foregroundCustomPictureEdit.Properties.NullText = currentData.EzdImage;
            this.foregroundCustomPictureEdit.Width = fgImg.Width;
            this.foregroundCustomPictureEdit.Height = fgImg.Height;
            this.foregroundCustomPictureEdit.Location = new Point((int)currentData.EzdImagePosX, (int)currentData.EzdImagePosY);

            CurrentEzd.EzdPictureEdit = this.foregroundCustomPictureEdit;
        }

        private void Upload(UploadType type)
        {
            // take filter type
            var filter = type == UploadType.Ezd
                             ? @"EZD file (*.ezd) | *.ezd"
                             : @"Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            using (var ofd = new OpenFileDialog { Multiselect = false, ValidateNames = true, Filter = filter })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {

                    // take BG|FG image from fileName
                    if (type == UploadType.Ezd)
                    {
                        try
                        {
                            var ezdNewName = $@"{DateTime.Now.Ticks}{Path.GetFileName(ofd.FileName)}";

                            var ezdInCurentPath = Path.Combine(filesPath, ezdNewName);

                            File.Copy(ofd.FileName, ezdInCurentPath);

                            var img = EzdDataControl.ReopositoryEzdFile.LoadImage(ezdInCurentPath,
                                this.foregroundCustomPictureEdit.Width,
                                this.foregroundCustomPictureEdit.Height);

                            this.foregroundCustomPictureEdit.Properties.NullText = ezdNewName;
                            this.foregroundCustomPictureEdit.Image = img;
                            this.foregroundCustomPictureEdit.Width = img.Width;
                            this.foregroundCustomPictureEdit.Height = img.Height;

                            CurrentEzd.EzdPictureEdit = this.foregroundCustomPictureEdit;

                            CurrentEzd.OriginalEzdPictureEdit = this.foregroundCustomPictureEdit;

                            this.ezdFileLbl.Text = Path.GetFileName(ofd.FileName);
                        }
                        catch (Exception ex)
                        {
                            XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        var img = Image.FromFile(ofd.FileName);

                        var bgImgNewName = $@"{DateTime.Now.Ticks}{Path.GetFileName(ofd.FileName)}";

                        var bgInCurentPath = Path.Combine(filesPath, bgImgNewName);

                        File.Copy(ofd.FileName, bgInCurentPath);

                        this.backgroundCustomPictureEdit.Properties.NullText = bgImgNewName;
                        this.backgroundCustomPictureEdit.Image = img;
                        this.backgroundCustomPictureEdit.Width = img.Width;
                        this.backgroundCustomPictureEdit.Height = img.Height;

                        CurrentEzd.BgPictureEdit = this.backgroundCustomPictureEdit;

                        this.bgImageLbl.Text = Path.GetFileName(ofd.FileName);
                    }
                }
            }
        }

        private void DeleteFileIfUpdated()
        {
            if (UserDataRepository.CheckSquence(currentPEindex) > 0)
            {
                UserDataDto userData = UserDataRepository.GetByTabIndex(currentPEindex);

                if (userData.Token != null)
                {
                    if (File.Exists($@"{filesPath}{userData.BgImage}"))
                    {
                        if (userData.BgImage != this.backgroundCustomPictureEdit.Properties.NullText)
                        {
                            File.Delete($@"{filesPath}{userData.BgImage}");
                        }
                    }

                    if (File.Exists($@"{filesPath}{userData.EzdImage}"))
                    {
                        if (userData.EzdImage != this.foregroundCustomPictureEdit.Properties.NullText)
                        {
                            File.Delete($@"{filesPath}{userData.EzdImage}");
                        }
                    }

                    if (File.Exists($@"{filesPath}{userData.FullImage}"))
                    {
                        File.Delete($@"{filesPath}{userData.FullImage}");
                    }
                }
            }
        }

        private void SaveImageDB()
        {
            #region Save fullimage in file

            // save full(panel) file
            var fullPanel_imageName = $@"Full_{this.bgImageLbl.Text}{DateTime.Now.Ticks}.jpg";
            var full_image = Images.PanelToImage(this.panelControl1);

            full_image.Save($@"{filesPath}{fullPanel_imageName}", ImageFormat.Jpeg);

            #endregion

            // save db
            UserDataRepository.Insert(new UserDataDto
            {
                Token = CurrentApiData.Token,
                Sequence = currentPEindex,
                Login = this.loginTextEdit.Text,
                Password = this.passwordTextEdit.Text,
                Url = this.urlTextEdit.Text,
                BgImage = this.backgroundCustomPictureEdit.Properties.NullText,
                EzdImage = this.foregroundCustomPictureEdit.Properties.NullText,
                FullImage = fullPanel_imageName,
                BgImagePosX = this.backgroundCustomPictureEdit.Location.X,
                BgImagePosY = this.backgroundCustomPictureEdit.Location.Y,
                EzdImagePosX = this.foregroundCustomPictureEdit.Location.X,
                EzdImagePosY = this.foregroundCustomPictureEdit.Location.Y
            });
        }
    }
}