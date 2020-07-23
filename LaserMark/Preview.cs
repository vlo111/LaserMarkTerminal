﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Configuration;

namespace LaserMark
{
    public partial class Preview : DevExpress.XtraEditors.XtraForm
    {
        public Preview(Bitmap image)
        {
            InitializeComponent();
            this.pictureEdit1.Image = image;
        }
    }
}