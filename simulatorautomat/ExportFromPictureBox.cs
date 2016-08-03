using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SimulatorAutomat
{
    class ExportFromPictureBox
    {
        private static Bitmap bmpCrop; // pastreaza regiunea de automat

        // calculeaza coordonatele: cea mai din stanga, cea mai din dreapta,
        // cea mai de sus si cea mai de jos ale dreptunghiului ce incadreaza
        // automatul si returneaza acest dreptunghi
        public static Rectangle GasesteGranita(PictureBox box)
        {
            int maxJos, minSus, minStanga, maxDreapta;
            Rectangle r = Rectangle.Empty;
            // daca avem vreun ontrol pe PictureBox
            if (box.Controls.Count > 0)
            {
                // luam coordonatele primului control adaugat
                maxJos = box.Controls[0].Bottom;
                minSus = box.Controls[0].Top;
                minStanga = box.Controls[0].Left;
                maxDreapta = box.Controls[0].Right;

                // si cautam minimul de sus si stanga
                // si maximul de jos si dreapta
                foreach (Control c in box.Controls)
                {
                    if (c.Bottom >= maxJos) maxJos = c.Bottom;
                    if (c.Left <= minStanga) minStanga = c.Left;
                    if (c.Right >= maxDreapta) maxDreapta = c.Right;
                    if (c.Top <= minSus) minSus = c.Top;
                }
                // cream dreptunghiul ce incepe cel mai de sus stanga
                // ai are lungimea maxDreapta - minStanga
                // si latimea maxJos - minSus
                r = new Rectangle(minStanga, minSus, Math.Abs(maxDreapta - minStanga + 10), Math.Abs(maxJos - minSus + 10));
            }
            return r;
        }

        // face efectiv exportul ca poza
        public static void ExportAsPicture(string fileName, PictureBox box)
        {
            Rectangle r = GasesteGranita(box);

            // cream un Bitmap si scriem in el ceea ce este desenat pe PictureBox
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(box.Width, box.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                box.DrawToBitmap(bmp, box.ClientRectangle);

                // cream un nou bitmap ce reprezinta dreptunghiul care 
                // incadreaza automatul
                bmpCrop = new Bitmap(r.Width, r.Height);
                Graphics g = Graphics.FromImage(bmpCrop);
                // fixam calitatea si continutul ca fiind automatul nostru
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.DrawImage(bmp, 0, 0, r, GraphicsUnit.Pixel);

                ImageFormat imgFormat = null; // alegem formatul imginii in functie de extensie
                string extension = Path.GetExtension(fileName); // asa preluam extensia
                switch (extension)
                {
                    case ".bmp":
                    case ".BMP":
                        imgFormat = ImageFormat.Bmp;
                        break;
                    case ".png":
                    case ".PNG":
                        imgFormat = ImageFormat.Png;
                        break;
                    case ".jpg":
                    case ".JPG":
                        imgFormat = ImageFormat.Jpeg;
                        break;
                }
                // salvam automatul, fara zone libere, fara nicio componenta
                bmpCrop.Save(fileName, imgFormat); // in formatul corespunzator
                bmpCrop.Dispose(); // eliberam bmpCrop din memorie
            }
        }
    }
}
