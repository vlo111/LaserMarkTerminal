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

namespace LaserMark
{
    public partial class UpdateEzdDataFromApi : DevExpress.XtraEditors.XtraUserControl
    {
        private Competitor _competitor;

        private LMForm _form;

        private PictureEdit _currentEzd;

        public UpdateEzdDataFromApi(LMForm form)
        {
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
    }
}