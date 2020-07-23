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
using System.Threading;

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

            try
            {
                if (testRedMarkBackgroundWorker.IsBusy)
                {
                    testRedMarkBackgroundWorker.CancelAsync();
                }

                if (testRedMarkContourBackgroundWorker.IsBusy)
                {
                    testRedMarkContourBackgroundWorker.CancelAsync();
                }

                if (btn.Name == "RUN")
                {
                    if (XtraMessageBox.Show("Вы действительно хотите гравировать?", "Сообщения", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (!runBackgroundWorker.IsBusy)
                        {
                            runBackgroundWorker.RunWorkerAsync();
                            btn.Name = "STOP";
                            btn.BackColor = Color.FromArgb(192, 0, 0);
                        }
                        else
                        {
                            XtraMessageBox.Show("Гравировка уже идет", "Information", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    ReopositoryEzdFile.StopMark();
                    btn.Name = "RUN";
                    btn.BackColor = Color.FromArgb(0, 192, 192);
                }

            }
            catch (Exception ex)
            {

                XtraMessageBox.Show(ex.Message, "Information", MessageBoxButtons.OK);
            }

        }

        private void RunBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ReopositoryEzdFile.Mark();
        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            var btn = (SimpleButton)sender;
            try
            {
                if (JczLmc.IsMarking())
                {
                    if (XtraMessageBox.Show("Вы действительно хотите простановить гравировку?", "Сообщения", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int nErr = JczLmc.StopMark();
                    }
                    else
                    {
                        return;
                    }
                }

                if (btn.Text == "TEST")
                {
                    if (btn.Tag.ToString() == "redMark")
                    {
                        if (!testRedMarkBackgroundWorker.IsBusy)
                        {
                            testRedMarkBackgroundWorker.RunWorkerAsync();
                        }

                        btn.Tag = "redMarkContour";
                    }
                    else if (btn.Tag.ToString() == "redMarkContour")
                    {
                        if (!testRedMarkContourBackgroundWorker.IsBusy)
                        {
                            testRedMarkContourBackgroundWorker.RunWorkerAsync();
                        }

                        btn.Tag = "redMark";
                    }

                    btn.BackColor = btn.BackColor = Color.FromArgb(192, 0, 0);
                    btn.Text = "STOP TEST";
                }
                else if (btn.Text == "STOP TEST")
                {
                    if (btn.Tag.ToString() == "redMark")
                    {
                        testRedMarkBackgroundWorker.CancelAsync();

                        btn.Tag = "redMarkContour";
                    }
                    else if (btn.Tag.ToString() == "redMarkContour")
                    {
                        testRedMarkContourBackgroundWorker.CancelAsync();

                        btn.Tag = "redMark";
                    }
                    btn.BackColor = Color.FromArgb(0, 192, 192);
                    btn.Text = "TEST";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Information", MessageBoxButtons.OK);
            }
        }

        private void TestRedMarkBackgroundWorkerr_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(400);
                ReopositoryEzdFile.RedMark();
            }
        }

        private void TestRedMarkContourBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(400);
                ReopositoryEzdFile.RedMarkContour();
            }
        }
    }
}