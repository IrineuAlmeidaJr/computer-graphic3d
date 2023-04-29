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

        private unsafe static void PosicionaXY(byte* ptr, int width, int height, int padding, int x, int y, int cor)
        {
            if (x > 0 && x < width && y > 0 && y < height)
            { 
                int deslocamento = (y * (width * 3 + padding)) + (x * 3);
                ptr += deslocamento;

                *(ptr++) = (byte)cor;
                *(ptr++) = (byte)0;
                *(ptr) = (byte)0;
            }

        }

        public static unsafe void DesenhaPontoMedio(double x1, double y1, double x2, double y2, byte* ptrIni, Bitmap imagem, int padding, int cor)
        {   
            int declive;
            double deltaX, deltaY, incE, incNE, d, x, y;
            deltaX = x2 - x1;
            deltaY = y2 - y1;


            if (Math.Abs(deltaX) > Math.Abs(deltaY)) // 10 -- 5
            {
                if (x1 > x2)
                {
                    DesenhaPontoMedio(x2, y2, x1, y1, ptrIni, imagem, padding, cor);
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
                    PosicionaXY(ptrIni, imagem.Width, imagem.Height, padding, (int)x, (int)y, cor);
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
                    DesenhaPontoMedio(x2, y2, x1, y1, ptrIni, imagem, padding, cor);
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
                    PosicionaXY(ptrIni, imagem.Width, imagem.Height, padding, (int)x, (int)y, cor);
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


        public static unsafe void PontoMedio(double x1, double y1, double x2, double y2, Bitmap imagem, int cor)
        {

            // --- Inicio DMA

            //lock dados bitmap origem
            BitmapData bitmapDataSrc = imagem.LockBits(new Rectangle(0, 0, imagem.Width, imagem.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // No bitmap ao abri no final sobra uma faixa de padding, então tem que tirar, pois, irá dar erro
            int padding = bitmapDataSrc.Stride - (imagem.Width * 3);

            byte* ptrIni = (byte*)bitmapDataSrc.Scan0;

            // ---------------

            DesenhaPontoMedio(x1, y1, x2, y2, ptrIni, imagem, padding, cor);

            // Desbloqueia a área de memória do bitmap
            imagem.UnlockBits(bitmapDataSrc);
        }


        //public static void PontoMedio(double x1, double y1, double x2, double y2, Bitmap imagem, int cor)
        //{
        //    int declive;
        //    double deltaX, deltaY, incE, incNE, d, x, y;
        //    deltaX = x2 - x1; 
        //    deltaY = y2 - y1;

        //    int w = imagem.Width;
        //    int h = imagem.Height;

        //    if (Math.Abs(deltaX) > Math.Abs(deltaY))
        //    {
        //        if (x1 > x2)
        //        {
        //            PontoMedio(x2, y2, x1, y1, imagem, cor);
        //            return;
        //        }

        //        declive = Math.Sign(deltaY);
        //        if (y1 > y2)
        //        {
        //            deltaY = -deltaY;
        //        }


        //        incE = 2 * deltaY;
        //        incNE = 2 * (deltaY - deltaX);
        //        d = 2 * deltaY - deltaX;
        //        y = y1;
        //        for (x = x1; x <= x2; x++)
        //        {
        //            if ((int)x > 0 && (int)x < w && (int)y > 0 && (int)y < h)
        //            {
        //                if (cor == 0)
        //                    imagem.SetPixel((int)x, (int)y, Color.Black);
        //                else
        //                    imagem.SetPixel((int)x, (int)y, Color.Blue);
        //            }
        //            if (d <= 0)
        //            {
        //                d += incE;
        //            }
        //            else
        //            {
        //                d += incNE;
        //                y += declive;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (y1 > y2)
        //        {
        //            PontoMedio(x2, y2, x1, y1, imagem, cor);
        //            return;
        //        }

        //        declive = Math.Sign(deltaX);
        //        if (x1 > x2)
        //        {
        //            deltaX = -deltaX;
        //        }


        //        incE = 2 * deltaX;
        //        incNE = 2 * (deltaX - deltaY);
        //        d = 2 * deltaX - deltaY;
        //        x = x1;
        //        for (y = y1; y <= y2; y++)
        //        {
        //            if ((int)x > 0 && (int)x < w && (int)y > 0 && (int)y < h)
        //            {
        //                if (cor == 0)
        //                    imagem.SetPixel((int)x, (int)y, Color.Black);
        //                else
        //                    imagem.SetPixel((int)x, (int)y, Color.Blue);
        //            }
        //            if (d <= 0)
        //            {
        //                d += incE;
        //            }
        //            else
        //            {
        //                d += incNE;
        //                x += declive;
        //            }
        //        }
        //    }
        //}
    }


}
