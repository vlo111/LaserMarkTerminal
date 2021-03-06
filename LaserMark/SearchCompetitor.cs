﻿using System;
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
using Newtonsoft.Json;
using EzdDataControl;
using System.Threading;
using LaserMark.State;

namespace LaserMark
{
    public partial class SearchCompetitor : DevExpress.XtraEditors.XtraUserControl
    {
        private CompetitorList _competitors;

        private PictureEdit _currentEzd;

        private TextEdit _bib_text;

        RadWaitingBar waitingBar;

        CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public SearchCompetitor(TextEdit bib)
        {
            _currentEzd = (PictureEdit)CurrentEzd.EzdPictureEdit;

            _bib_text = bib;

            InitializeComponent();

            waitingBar = new RadWaitingBar();
            waitingBar.AssociatedControl = this.layoutControl2;
            waitingBar.Size = new System.Drawing.Size(80, 80);
            waitingBar.WaitingStyle = Telerik.WinControls.Enumerations.WaitingBarStyles.LineRing;

            this.layoutControl2.Controls.Add(waitingBar);
        }

        private void SearchCompetitor_Load(object sender, EventArgs e)
        {
            this.listView1.Columns.AddRange(
                new ColumnHeader[]
                    {
                        new ColumnHeader() { Text = @"Bib", Width = 50 },
                        new ColumnHeader() { Text = @"First Name", Width = 150 },
                        new ColumnHeader() { Text = @"Last Name", Width = 150 },
                        new ColumnHeader() { Text = @"Birth year", Width = 100 }
                    });

            this.MaximumSize = new Size(CurrentUIData.WindowSize.Width, CurrentUIData.WindowSize.Height - (CurrentUIData.WindowSize.Height / 3));

            this.Height = CurrentUIData.WindowSize.Height - (CurrentUIData.WindowSize.Height / 3);
            this.Width = CurrentUIData.WindowSize.Width;

            this.Left = 0;
            this.Top = (CurrentUIData.WindowSize.Height - this.Height) / 2;
        }

        private async void searchControl1_TextChanged(object sender, EventArgs e)
        {

            var search = this.searchControl.Text;

            if (string.IsNullOrEmpty(search))
            {
                return;
            }

            

            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }

            _tokenSource = new CancellationTokenSource();

            try
            {
                
                this.waitingBar.StartWaiting();

                await loadPrestatieGetCompetitorAsync(this.listView1, _tokenSource.Token, search);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task loadPrestatieGetCompetitorAsync(ListView list, CancellationToken token, string search)
        {
            await Task.Delay(500, token).ConfigureAwait(true);
            try
            {
                var task = await Queries.GetRequestAsync(
                  $@"http://openeventor.ru/event/{CurrentApiData.Token}/plugins/engraver/get?search={this.searchControl.Text}");

                this._competitors = JsonConvert.DeserializeObject<CompetitorList>(task);

                UpdateListView(search);

                this.waitingBar.StopWaiting();

                token.ThrowIfCancellationRequested();

            }
            catch (OperationCanceledException ex)
            {
                throw;
            }
            catch (Exception Ex)
            {
                throw;
            }
        }

        private void UpdateListView(string search)
        {
            this.listView1.Items.Clear();

            var listItem = new List<ListViewItem>();

            if (this._competitors.CompetitorDatas == null || this._competitors.CompetitorDatas.Count <= 0)
            {
                return;
            }

            var competitors = this._competitors.CompetitorDatas.Where(
                p => p.LastName != null && p.LastName.ToLower().Contains(search.Trim().ToLower()));

            var searchedCompetitorList = competitors.ToList();

            searchedCompetitorList.ForEach(
                   p =>
                   {
                       listItem.Add(new ListViewItem(new[] { p.Bib, p.FirstName, p.LastName, p.BirthYear }));
                   });

            this.listView1.Items.AddRange(listItem.ToArray());

            //if (this.listView1.Items.Count <= 0)
            //{
            //    this.errorLabel.Text = $@"Info: Поиск не дал результатов";
            //}
            //else
            //{
            //    this.errorLabel.Text = " ";
            //}
        }

        private void KeyBtns_Click(object sender, EventArgs e)
        {
            var btn = (DevExpress.XtraEditors.SimpleButton)sender;

            if (btn.Text == "<")
            {
                if (searchControl.Text.Length <= 0)
                {
                    return;
                }

                searchControl.Text = searchControl.Text.Remove(searchControl.Text.Length - 1);
            }
            else
            {
                searchControl.Text = searchControl.Text + btn.Text;
            }
        }

        private void enterBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.searchControl.Text) 
                || this._competitors == null 
                || this._competitors.CompetitorDatas == null 
                || this._competitors.CompetitorDatas.Count <= 0)
            {
                return;
            }

            var selectedCompotitor = this._competitors.CompetitorDatas.FirstOrDefault(p => p.Bib == this.listView1.SelectedItems[0].Text);

            this._bib_text.Text = selectedCompotitor.Bib;

            this._currentEzd.Image = ReopositoryEzdFile.UpdateEzdApi(selectedCompotitor, this._currentEzd.Width, this._currentEzd.Height);
        }
    }
}