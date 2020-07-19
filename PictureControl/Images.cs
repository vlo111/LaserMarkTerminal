using DevExpress.XtraEditors;
using System.Drawing;
using System.IO;

namespace PictureControl
{
    public class Images
    {
        // The Scale. Reduce image size
        public static Image Scale(Image img, Size size)
        {
            int width = img.Width - (img.Width * size.Width / 100);
            int heigth = img.Height - (img.Height * size.Height / 100);

            Bitmap bmp = new Bitmap(img, width, heigth);

            Graphics graphics = Graphics.FromImage(bmp);

            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            return bmp;
        }

        public static Image Zoom(Image img, Size size)
        {
            int width = img.Width + (img.Width * size.Width / 100);
            int heigth = img.Height + (img.Height * size.Height / 100);

            Bitmap bmp = new Bitmap(img, width, heigth);

            Graphics graphics = Graphics.FromImage(bmp);

            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            return bmp;
        }

        public static Bitmap SetImageTransparent(Image image)
        {
            var bitmap = (Bitmap)image;

            bitmap.MakeTransparent();

            return bitmap;
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        public static Bitmap PanelToImage(PanelControl control)
        {
            int width = control.Size.Width;
            int height = control.Size.Height;

            var bmp = new Bitmap(width, height);
            control.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));

            return bmp;
        }
    }
}
