namespace LaserMark
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    class CustomPictureEdit : DevExpress.XtraEditors.PictureEdit
    {
        public bool controlMoving = true;

        public CustomPictureEdit(IContainer container)
        {
            container.Add(this);
        }

        Point point;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (controlMoving)
            {
                this.point = e.Location;
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (controlMoving)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Left += e.X - this.point.X;
                    this.Top += e.Y - this.point.Y;
                }

                base.OnMouseMove(e);
            }
        }

    }
}
