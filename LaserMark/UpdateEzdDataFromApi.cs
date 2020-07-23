using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using Api;
using EzdDataControl;
using LaserMark.State;
using System.IO;

namespace LaserMark
{
    public partial class UpdateEzdDataFromApi : DevExpress.XtraEditors.XtraUserControl
    {
        private Competitor _competitor;

        private LMForm _form;

        private PictureEdit _currentEzd;

        public UpdateEzdDataFromApi(LMForm form)
        {
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

            _form = form;
            _currentEzd = (PictureEdit)CurrentEzd.EzdPictureEdit;

            this.MaximumSize = CurrentUIData.RightPanelSize;
            this.MinimumSize = CurrentUIData.RightPanelSize;

            InitializeComponent();

            this.marginUpEmptySpace.MinSize = new Size(0, CurrentUIData.RightPanelSize.Height / 6);

            this.flyoutPanel1.OwnerControl = CurrentUIData.RightLayoutControl;

            this.flyoutPanel1.MaximumSize = CurrentUIData.RightPanelSize;
            this.flyoutPanel1.MinimumSize = CurrentUIData.RightPanelSize;

            this.flyoutPanel1.ShowPopup();
        }

        private void HidePopupBtn_Click(object sender, EventArgs e)
        {
            this.flyoutPanel1.HidePopup();
        }

        private void KeyBtns_Click(object sender, EventArgs e)
        {
            var btn = (DevExpress.XtraEditors.SimpleButton)sender;

            if (btn.Text == "C")
            {
                searchTextEdit.Text = searchTextEdit.Text.Remove(searchTextEdit.Text.Length - 1);
            }
            else
            {
                searchTextEdit.Text = searchTextEdit.Text + btn.Text;
            }
        }

        private async void OkSimpleButton_Click(object sender, EventArgs e)
        {
            try
            {
                var task = await Queries.GetRequestAsync(
                  $@"http://openeventor.ru/api/event/{CurrentApiData.Token}/get_competitor/bib/{this.searchTextEdit.Text}");

                this._competitor = JsonConvert.DeserializeObject<Competitor>(task);

                string filesPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName) + @"\files\";
                
                ReopositoryEzdFile.LoadImage($@"{filesPath}{this._currentEzd.Properties.NullText}");

                this._currentEzd.Image = ReopositoryEzdFile.UpdateEzdApi(_competitor.CompetitorData, this._currentEzd.Width, this._currentEzd.Height);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Данные с этим номером не найдены", "Information", MessageBoxButtons.OK);
            }
        }

        private void SearchSimpleButton_Click(object sender, EventArgs e)
        {
            CustomFlyoutDialog.ShowForm(_form, null, new SearchCompetitor(this.searchTextEdit));
        }

        private void editEzdBtn_Click(object sender, EventArgs e)
        {
            var ezdObjects = EzdDataControl.ReopositoryEzdFile.GetEzdData();

            new UpdateEzdData(ezdObjects);
        }

        private void RunBtn_Click(object sender, EventArgs e)
        {
            var btn = (SimpleButton)sender;

            if (btn.Name == "Гравировать")
            {
                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync();
                    btn.Name = "Стоп";
                    btn.BackColor = Color.FromArgb(192, 0, 0);
                }
                else
                {
                    XtraMessageBox.Show("后台线程工作中", "Information", MessageBoxButtons.OK);
                }
            }
            else
            {
                int nErr = JczLmc.StopMark();
                btn.Name = "Гравировать";
                btn.BackColor = Color.FromArgb(0, 192, 192);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int nErr = JczLmc.Mark(false);
        }
    }
}