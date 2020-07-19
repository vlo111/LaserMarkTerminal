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
using Telerik.WinControls.UI;
using Api;

namespace LaserMark
{
    public partial class SearchCompetitor : DevExpress.XtraEditors.XtraUserControl
    {
        private CompetitorList _competitors;
        public SearchCompetitor(CompetitorList competitorList)
        {
            _competitors = competitorList;

            InitializeComponent();
        }

        private void SearchCompetitor_Load(object sender, EventArgs e)
        {
            this.radListView1.Columns.AddRange(
                new Telerik.WinControls.UI.ListViewDetailColumn[]
                    {
                        new Telerik.WinControls.UI.ListViewDetailColumn(@"Bib"),
                        new Telerik.WinControls.UI.ListViewDetailColumn(@"First Name"),
                        new Telerik.WinControls.UI.ListViewDetailColumn( @"Last Name"),
                        new Telerik.WinControls.UI.ListViewDetailColumn(@"Birth year")
                    });
        }

        private void searchControl1_TextChanged(object sender, EventArgs e)
        {
            var search = this.searchControl.Text;

            if (string.IsNullOrEmpty(search))
            {
                return;
            }

            this.radListView1.Items.Clear();

            var listItem = new List<ListViewDataItem>();

            var competitors = this._competitors.CompetitorDatas.Where(
                p => p.LastName != null && p.LastName.ToLower().Contains(search.Trim().ToLower()));

            var searchedCompetitorList = competitors.ToList();

            searchedCompetitorList.ForEach(
                   p =>
                   {
                       listItem.Add(new ListViewDataItem(p.LastName, new[] { p.Bib, p.FirstName, p.LastName, p.BirthYear }));
                   });

            this.radListView1.Items.AddRange(listItem.ToArray());

            if (this.radListView1.Items.Count <= 0)
            {
                this.errorLabel.Text = $@"Info: Поиск не дал результатов";
            }
            else
            {
                this.errorLabel.Text = " ";
            }
        }

        private void KeyBtns_Click(object sender, EventArgs e)
        {
            var btn = (DevExpress.XtraEditors.SimpleButton)sender;

            if (btn.Text == "<")
            {
                searchControl.Text = searchControl.Text.Remove(searchControl.Text.Length - 1);
            }
            else
            {
                searchControl.Text = searchControl.Text + btn.Text;
            }
        }

        private void enterBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
