using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGraphic.Models
{
    public class Vertice
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vertice(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"X:{X}; Y:{Y}";
        }      


        public static void PontoMedio(double x1, double y1, double x2, double y2, Bitmap imagem)
        {
            int declive;
            double deltaX, deltaY, incE, incNE, d, x, y;
            deltaX = x2 - x1; 
            deltaY = y2 - y1;

            int w = imagem.Width;
            int h = imagem.Height;

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                if (x1 > x2)
                {
                    PontoMedio(x2, y2, x1, y1, imagem);
                    return;
                }

                declive = Math.Sign(deltaY);
                if (y1 > y2)
                {
                    deltaY = -deltaY;
                }


                incE = 2 * deltaY;
                incNE = 2 * (deltaY - deltaX);
                d = 2 * deltaY - deltaX;
                y = y1;
                for (x = x1; x <= x2; x++)
                {
                    if ((int)x > 0 && (int)x < w && (int)y > 0 && (int)y < h)
                    {
                        imagem.SetPixel((int)x, (int)y, Color.Black);
                    }
                    if (d <= 0)
                    {
                        d += incE;
                    }
                    else
                    {
                        d += incNE;
                        y += declive;
                    }
                }
            }
            else
            {
                if (y1 > y2)
                {
                    PontoMedio(x2, y2, x1, y1, imagem);
                    return;
                }

                declive = Math.Sign(deltaX);
                if (x1 > x2)
                {
                    deltaX = -deltaX;
                }


                incE = 2 * deltaX;
                incNE = 2 * (deltaX - deltaY);
                d = 2 * deltaX - deltaY;
                x = x1;
                for (y = y1; y <= y2; y++)
                {
                    if ((int)x > 0 && (int)x < w && (int)y > 0 && (int)y < h)
                    {
                        imagem.SetPixel((int)x, (int)y, Color.Black);
                    }
                    if (d <= 0)
                    {
                        d += incE;
                    }
                    else
                    {
                        d += incNE;
                        x += declive;
                    }
                }
            }
        }
    }


}
