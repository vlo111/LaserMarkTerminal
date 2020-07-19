using Api;
using LaserMark;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PictureControl;

namespace EzdDataControl
{
    public class ReopositoryEzdFile
    {
        public enum ModeFontSize
        {
            reduce,
            zoom
        }
        public static List<Tuple<string, StringBuilder>> GetEzdData()
        {
            var count = JczLmc.GetEntityCount();

            var ezdObjects = new List<Tuple<string, StringBuilder>>();

            var names = new List<string>();

            for (int i = 0; i < count; i++)
            {
                names.Add(JczLmc.GetEntityNameByIndex(i));
            }

            names = names.Where(p => { return !string.IsNullOrEmpty(p); }).Distinct().ToList();

            foreach (var name in names)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var str = new StringBuilder();

                    JczLmc.GetTextByName(name, str);

                    ezdObjects.Add(new Tuple<string, StringBuilder>(name, str));
                }
            }

            return ezdObjects;
        }

        public static Image UpdateCustomEzd(Tuple<string, string> ezdObj, int width, int height)
        {
            JczLmc.ChangeTextByName(ezdObj.Item1, ezdObj.Item2);

            var img = JczLmc.GetCurPreviewImage(width, height);

            img = PictureControl.Images.SetImageTransparent(img);

            return img;
        }

        public static Image UpdateEzdApi(CompetitorData competitor, int width, int height)
        {
            if (!string.IsNullOrEmpty(competitor.FirstName))
            {
                JczLmc.ChangeTextByName(JczLmc.GetEntityNameByIndex(0), competitor.FirstName);
            }

            if (!string.IsNullOrEmpty(competitor.LastName))
            {
                JczLmc.ChangeTextByName(JczLmc.GetEntityNameByIndex(1), competitor.LastName);
            }

            if (!string.IsNullOrEmpty(competitor.TimeOfDistance))
            {
                JczLmc.ChangeTextByName(JczLmc.GetEntityNameByIndex(2), competitor.TimeOfDistance);
            }

            if (!string.IsNullOrEmpty(competitor.Distance))
            {
                JczLmc.ChangeTextByName(JczLmc.GetEntityNameByIndex(3), competitor.Distance);
            }

            var img = JczLmc.GetCurPreviewImage(width, height);

            img = Images.SetImageTransparent(img);

            return img;
        }

        public static Image LoadImage(string fileName, int width, int height)
        {
            // load ezd
            JczLmc.LoadEzdFile(fileName);

            // get image from sdk
            var img = JczLmc.GetCurPreviewImage(width, height);

            img = PictureControl.Images.SetImageTransparent(img);

            return img;
        }

        public static Image FontSize(string entName, ModeFontSize mode, int width, int heght)
        {
            StringBuilder font = new StringBuilder();
            double height_ezd = 0;
            double width_ezd = 0;
            double angle = 0;
            double space = 0;
            double line_space = 0;
            bool bBold = false;
            int nTextAlign = 0;
            bool bItalic = false;

            int nTextSpaceMode = 0;
            double dTextSpace = 0;
            double dNullCharWidthRatio = 0;

            JczLmc.GetTextEntParam4(entName,
                font,
                ref nTextSpaceMode,
                ref dTextSpace,
                ref height_ezd,
                ref width_ezd,
                ref angle,
                ref space,
                ref line_space,
                ref dNullCharWidthRatio,
                ref nTextAlign,
                ref bBold,
                ref bItalic);
            if (mode == ModeFontSize.reduce)
            {
                height_ezd = height_ezd - 0.125d;
                width_ezd = width_ezd - 0.0125d;
            }
            else
            {
                height_ezd = height_ezd + 0.125d;
                width_ezd = width_ezd + 0.0125d;
            }


            JczLmc.SetTextEntParam4(entName,
                font.ToString(),
                nTextSpaceMode,
                dTextSpace,
                height_ezd,
                width_ezd,
                angle,
                space,
                line_space,
                dNullCharWidthRatio,
                nTextAlign,
                bBold,
                bItalic);

            return JczLmc.GetCurPreviewImage(width, heght);
        }
    }
}