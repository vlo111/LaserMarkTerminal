﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using EzdDataControl;
using LaserMark.State;

namespace LaserMark
{
    public partial class UpdateEzdData : XtraUserControl
    {
        List<TextEdit> textEdits = new List<TextEdit>();

        List<Tuple<string, StringBuilder>> _competitor;

        PictureEdit _currentEzd;

        public UpdateEzdData(List<Tuple<string, StringBuilder>> competitor)
        {
            _competitor = competitor;
            _currentEzd = (CustomPictureEdit)(CurrentEzd.EzdPictureEdit);

            this.MaximumSize = CurrentUIData.RightPanelSize;
            this.MinimumSize = CurrentUIData.RightPanelSize;

            InitializeComponent();

            try
            {

                this.flyoutPanel1.OwnerControl = CurrentUIData.RightLayoutControl;

                this.flyoutPanel1.MaximumSize = CurrentUIData.RightPanelSize;
                this.flyoutPanel1.MinimumSize = CurrentUIData.RightPanelSize;

                this.flyoutPanel1.ShowPopup();

                // Text
                var textEditSize = new Size();
                textEditSize.Height = 40 - 5;
                textEditSize.Width = this.Width - (int)(this.Width / 2.5) - 5;

                // Text`s layout
                var middlelayoutItemSize = new Size();
                middlelayoutItemSize.Height = 40;
                middlelayoutItemSize.Width = this.Width - (int)(this.Width / 2.5);

                // Button`s layout
                var layoutItemSize = new Size();
                layoutItemSize.Height = 40;
                layoutItemSize.Width = 42;

                for (int index = 60; index < (60 + competitor.Count); index++)
                {
                    #region Initial TextEdits

                    var textEdit = this.layoutControl2.Controls.OfType<TextEdit>()
                        .Where(c => c.TabIndex == index)
                        .Select(c => c)
                        .First();

                    textEdit.Text = competitor[index - 60].Item2.ToString();

                    textEdit.MinimumSize = textEditSize;
                    textEdit.MaximumSize = textEditSize;
                    textEdit.Size = textEditSize;

                    textEdit.Properties.NullText = competitor[index - 60].Item1;

                    textEdits.Add(textEdit);

                    #endregion

                    #region Initial TextEditLayout

                    var layoutItem = layoutControlGroup3.Items
                        .Where(p =>
                        {
                            if (p.Tag != null)
                            {
                                p.Tag = Convert.ToString(p.Tag);

                                string layout_index = index.ToString();

                                bool tag = (string)p.Tag == layout_index;

                                return tag;
                            }
                            return false;
                        })
                        .Select(c => c)
                        .First();

                    layoutItem.MaxSize = middlelayoutItemSize;
                    layoutItem.MinSize = middlelayoutItemSize;
                    layoutItem.Size = middlelayoutItemSize;

                    #endregion

                    #region Initial Button`s layouts

                    var pluslayout = layoutControlGroup3.Items
                        .Where(p =>
                        {
                            if (p.Tag != null)
                            {
                                p.Tag = Convert.ToString(p.Tag);

                                string layout_index = (index + 40).ToString();

                                bool tag = (string)p.Tag == layout_index;

                                return tag;
                            }
                            return false;
                        })
                        .Select(c => c)
                        .First();

                    var minuslayout = layoutControlGroup3.Items
                        .Where(p =>
                        {
                            if (p.Tag != null)
                            {
                                p.Tag = Convert.ToString(p.Tag);

                                string layout_index = (index + 50).ToString();

                                bool tag = (string)p.Tag == layout_index;

                                return tag;
                            }
                            return false;
                        })
                        .Select(c => c)
                        .First();

                    pluslayout.MaxSize = layoutItemSize;
                    pluslayout.MinSize = layoutItemSize;
                    pluslayout.Size = layoutItemSize;

                    minuslayout.MaxSize = layoutItemSize;
                    minuslayout.MinSize = layoutItemSize;
                    minuslayout.Size = layoutItemSize;

                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region Plus and Minus btn click event

        private void ChangeFont(ReopositoryEzdFile.ModeFontSize mode, int index)
        {
            var img = ReopositoryEzdFile.FontSize(_competitor[index].Item1,
                mode,
                _currentEzd.Image.Width,
                _currentEzd.Image.Height);

            img = PictureControl.Images.SetImageTransparent(img);

            _currentEzd.Image = img;
        }

        private void obj1PlusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Zoom, 0);
        }

        private void obj2PlusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Zoom, 1);
        }

        private void obj3PlusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Zoom, 2);
        }

        private void obj4PlusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Zoom, 3);
        }

        private void obj5PlusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Zoom, 4);
        }

        private void obj6PlusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Zoom, 5);
        }

        private void obj7PlusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Zoom, 6);
        }

        private void obj8PlusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Zoom, 7);
        }

        private void obj1MinusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Reduce, 0);
        }

        private void obj2MinusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Reduce, 1);
        }

        private void obj3MinusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Reduce, 2);
        }

        private void obj4MinusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Reduce, 3);
        }

        private void obj5MinusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Reduce, 4);
        }

        private void obj6MinusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Reduce, 5);
        }

        private void obj7MinusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Reduce, 6);
        }

        private void obj8MinusBtn_Click(object sender, EventArgs e)
        {
            ChangeFont(ReopositoryEzdFile.ModeFontSize.Reduce, 7);
        }

        #endregion

        private void HidePopupBtn_Click(object sender, EventArgs e)
        {
            this.flyoutPanel1.HidePopup();
        }

        private void obj1TextEdit_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var text = (TextEdit)sender;

                var comp = new Tuple<string, string>(text.Properties.NullText, text.Text);

                _currentEzd.Image = ReopositoryEzdFile.UpdateCustomEzd(comp, _currentEzd.Width, _currentEzd.Height);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Данные с этим номером не найдены", "Information", MessageBoxButtons.OK);
            }
        }
    }
}
