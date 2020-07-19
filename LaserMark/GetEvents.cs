using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using DevExpress.XtraEditors;
using Api;

namespace LaserMark
{
    public partial class GetEvents : DevExpress.XtraEditors.XtraUserControl
    {
        private readonly IList<Event> _data;

        public GetEvents(Eventor eventor)
        {
            InitializeComponent();

            this._data = eventor.Events.Select(p => new Event { Token = p.Token, Id = p.Id, Name = p.Name }).ToList();

            if (eventor.Events != null)
            {
                this.listBoxControl1.DataSource = this._data;
                this.listBoxControl1.DisplayMember = "Name";
            }
        }

        private void KeyBtns_Click(object sender, EventArgs e)
        {
            var btn = (DevExpress.XtraEditors.SimpleButton)sender;
            if (btn.Text == "<")
            {
                if (string.IsNullOrEmpty(this.searchEventControl.Text))
                {
                    return;
                }

                this.searchEventControl.Text = this.searchEventControl.Text.Remove(this.searchEventControl.Text.Length - 1);
            }
            else
            {
                this.searchEventControl.Text = this.searchEventControl.Text + btn.Text;
            }

            this.listBoxControl1.Items.Clear();

            Event[] search = this._data.Where(p => p.Name.ToLower().Contains(this.searchEventControl.Text.Trim().ToLower()))
                .ToArray();

            this.listBoxControl1.DataSource = search;
        }

        private void enterBtn_Click(object sender, EventArgs e)
        {
            var data = (Event)this.listBoxControl1.SelectedItem;

            if (data != null)
            {
                LMForm.current_token = data.Token;

                ((SimpleButton)sender).DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
    }
}