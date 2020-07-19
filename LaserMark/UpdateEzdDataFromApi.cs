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

namespace LaserMark
{
    public partial class UpdateEzdDataFromApi : DevExpress.XtraEditors.XtraUserControl
    {
        private string _token;

        private CustomPictureEdit _ezdPictureEdit;

        private Competitor _competitor;

        private CompetitorList _competitors;

        private LMForm _form;

        PanelControl _rightLayoutControl;


        public UpdateEzdDataFromApi(LMForm form, PanelControl layoutControl, string token, Control control, Size windowSize)
        {
            _token = token;
            _ezdPictureEdit = (CustomPictureEdit)control;
            _form = form;
            _rightLayoutControl = layoutControl;
            this.MaximumSize = windowSize;
            this.MinimumSize = windowSize;

            InitializeComponent();

            this.marginUpEmptySpace.MinSize = new Size(0, windowSize.Height / 6);

            this.flyoutPanel1.OwnerControl = layoutControl;

            this.flyoutPanel1.MaximumSize = windowSize;
            this.flyoutPanel1.MinimumSize = windowSize;

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
                  $@"http://openeventor.ru/api/event/{_token}/get_competitor/bib/{this.searchTextEdit.Text}");

                this._competitor = JsonConvert.DeserializeObject<Competitor>(task);

                this._ezdPictureEdit.Image = ReopositoryEzdFile.UpdateEzdApi(_competitor.CompetitorData, this._ezdPictureEdit.Width, this._ezdPictureEdit.Height);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Данные с этим номером не найдены", "Information", MessageBoxButtons.OK);
            }
        }

        private async void SearchSimpleButton_Click(object sender, EventArgs e)
        {
            await GetCompetitorList();

            CustomFlyoutDialog.ShowForm(_form, null, new SearchCompetitor(_competitors));
        }

        private async Task GetCompetitorList()
        {
            var task = await Queries.GetRequestAsync(
                               $@"http://openeventor.ru/api/event/{_token}/get_competitors");

            this._competitors = JsonConvert.DeserializeObject<CompetitorList>(task);
        }

        private void editEzdBtn_Click(object sender, EventArgs e)
        {
            var ezdObjects = EzdDataControl.ReopositoryEzdFile.GetEzdData();

            new UpdateEzdData(ezdObjects,
                _rightLayoutControl,
                _ezdPictureEdit,
                new Size(_rightLayoutControl.Width, this.ClientRectangle.Height));
        }
    }
}