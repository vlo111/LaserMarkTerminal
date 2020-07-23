using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using EzdDataControl;

namespace LaserMark
{
    public partial class Preview : DevExpress.XtraEditors.XtraForm
    {
        public Preview(Bitmap image)
        {
            InitializeComponent();
            this.pictureEdit1.Image = image;
        }

        private void RunBtn_Click(object sender, EventArgs e)
        {
            var btn = (SimpleButton)sender;

            if (btn.Name == "Гравировать")
            {
                if (XtraMessageBox.Show("Вы действительно хотите гравировать?", "Сообщения", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (!runBackgroundWorker.IsBusy)
                    {
                        runBackgroundWorker.RunWorkerAsync();
                        btn.Name = "Стоп";
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
                btn.Name = "Гравировать";
                btn.BackColor = Color.FromArgb(0, 192, 192);
            }
        }

        private void RunBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ReopositoryEzdFile.Mark();
        }
    }
}