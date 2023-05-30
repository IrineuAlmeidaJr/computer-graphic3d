using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ComputerGraphic.Models
{
    public class Iluminacao
    {
        public int luzX {  get; set; }
        public int luzY { get; set; }

        public Iluminacao()
        {
            luzX = luzY = 10;
        }

        public void DesenhaFonteLuz(Bitmap imagem, PictureBox pictureBox)
        {
            Graphics graphics = Graphics.FromImage(imagem);

            Brush brush = new SolidBrush(Color.FromKnownColor(KnownColor.Yellow));

            Pen pen = new Pen(brush, 15);

            // Desenhar retângulo
            graphics.DrawRectangle(pen, luzX, luzY, 10, 10);

            pictureBox.Image = imagem;
        }

        public void ApagaFonteLuz(Bitmap imagem, PictureBox pictureBox)
        {
            Graphics graphics = Graphics.FromImage(imagem);

            Brush brush = new SolidBrush(Color.FromKnownColor(KnownColor.Black));

            Pen pen = new Pen(brush, 15);

            // Desenhar retângulo
            graphics.DrawRectangle(pen, luzX, luzY, 10, 10);

            pictureBox.Image = imagem;
        }

    }
}
