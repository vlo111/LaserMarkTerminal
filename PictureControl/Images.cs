using DevExpress.XtraEditors;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace PictureControl
{
    public class Images
    {
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        // The Scale. Reduce image size
        public static Image Scale(Image image, Size size)
        {
            int width = image.Width - (image.Width * size.Width / 100);
            int height = image.Height - (image.Height * size.Height / 100);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Image Zoom(Image image, Size size)
        {
            int width = image.Width + (image.Width * size.Width / 100);
            int height = image.Height + (image.Height * size.Height / 100);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;

            //    Bitmap OrgImage = new Bitmap(img);
            //    Bitmap newImage = new Bitmap(width, heigth);

            //    using (Graphics gr = Graphics.FromImage(newImage))
            //    {
            //        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //        gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //        gr.DrawImage(OrgImage, 0, 0, width, heigth);
            //    }

            //    return newImage;
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

        public static Image ByteArrayToImage( byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                return Bitmap.FromStream(ms);
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
