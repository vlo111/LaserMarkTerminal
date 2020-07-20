using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Api;
using LaserMark.State;

namespace LaserMark
{
    public partial class LMForm : XtraForm
    {
        public List<Tuple<string, StringBuilder>> ezdObjects;

        public static List<Tuple<string, StringBuilder>> updatedEzdObjects;

        private enum UploadType
        {
            image = 1,
            Ezd
        }

        public LMForm()
        {
            InitializeComponent();
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
                // Get all events
                var base64HeaderValue = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes($@"{this.loginTextEdit.Text}:{this.passwordTextEdit.Text}"));

                const string Url = @"http://openeventor.ru/api/get_events";

                var task = await Queries.GetAllEventsAsync(Url, base64HeaderValue);

                var result = JsonConvert.DeserializeObject<Eventor>(task);

                CustomFlyoutDialog.ShowForm(this, null, new GetEvents(result));

                this.urlTextEdit.Text = $@"http://openeventor.ru/api/event/{CurrentApiData.Token}/get_event";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Нет доступ к апи, проверьте интернет соединения", "Error", MessageBoxButtons.OK);

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

        private void deleteBtn_Click(object sender, EventArgs e)
        {

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {

        }

        private void LMForm_Load(object sender, EventArgs e)
        {
            this.rightPanelControl.Width = ClientRectangle.Width / 100 * 25;
            var width_panel = this.headerLbl.Width;

            var onePictureWidth = width_panel / 10 - 10;

            // Connect sdk
            var err = JczLmc.Initialize(Application.StartupPath, true);

            // Init forground image parent
            this.foregroundCustomPictureEdit.Parent = this.backgroundCustomPictureEdit;

            CurrentUIData.RightLayoutControl = this.rightPanelControl;

            CurrentUIData.WindowSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

            CurrentUIData.RightPanelSize = new Size(this.rightPanelControl.Width, ClientRectangle.Height);
        }

        #endregion

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
                            var img = EzdDataControl.ReopositoryEzdFile.LoadImage(ofd.FileName,
                                this.foregroundCustomPictureEdit.Width,
                                this.foregroundCustomPictureEdit.Height);

                            this.foregroundCustomPictureEdit.Image = img;
                            this.foregroundCustomPictureEdit.Width = img.Width;
                            this.foregroundCustomPictureEdit.Height = img.Height;

                            CurrentEzd.EzdPictureEdit = this.foregroundCustomPictureEdit;

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

                        this.backgroundCustomPictureEdit.Image = img;
                        this.backgroundCustomPictureEdit.Width = img.Width;
                        this.backgroundCustomPictureEdit.Height = img.Height;

                        CurrentEzd.BgPictureEdit = this.backgroundCustomPictureEdit;

                        this.bgImageLbl.Text = Path.GetFileName(ofd.FileName);
                    }
                }
            }
        }
    }
}