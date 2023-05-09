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

        public int NumFaceCompartilhadas { get; set; }

        public Vertice(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;

            NumFaceCompartilhadas = 0;
        }

        public override string ToString()
        {
            return $"X:{X}; Y:{Y}";
        }

        private unsafe static void PosicionaX(byte* ptr, int width, int height, int padding, int x, int y, int cor)
        {
            if (x > 0 && x < width && y > 0 && y < height)
            { 
                ptr += x * 3;

                *(ptr++) = (byte)cor;
                *(ptr++) = (byte)cor;
                *(ptr++) = (byte)cor;

                ptr += Math.Abs(width - 3 * x) + padding;

            }

        }

        public static unsafe void DesenhaPontoMedio(double x1, double y1, double x2, double y2, byte* ptr, Bitmap imagem, int padding, int cor)
        {   
            int declive;
            int deltaX, deltaY, incE, incNE, d, x, y;
            deltaX = Convert.ToInt32(x2 - x1);
            deltaY = Convert.ToInt32(y2 - y1);


            if (Math.Abs(deltaX) > Math.Abs(deltaY)) // 10 -- 5
            {
                if (x1 > x2)
                {
                    DesenhaPontoMedio(x2, y2, x1, y1, ptr, imagem, padding, cor);
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
                y = Convert.ToInt32(y1);
                // posiciona no Y de início
                ptr += (int)y * (imagem.Width * 3 + padding);
                // ---------------
                for (x = Convert.ToInt32(x1); x <= x2; x++)
                {
                    PosicionaX(ptr, imagem.Width, imagem.Height, padding, (int)x, (int)y, cor);
                    if (d <= 0)
                    {
                        d += incE;
                    }
                    else
                    {
                        d += incNE;
                        y += declive;
                        // Ponteiro anda uma linha
                        if (declive < 0)
                            ptr -= (imagem.Width * 3 + padding);
                        else
                            ptr += imagem.Width * 3 + padding;
                    }
                }
            }
            else
            {
                if (y1 > y2)
                {
                    DesenhaPontoMedio(x2, y2, x1, y1, ptr, imagem, padding, cor);
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
                x = Convert.ToInt32(x1);
                // posiciona no Y de início
                ptr += (int)y1 * (imagem.Width * 3 + padding);
                // ---------------
                for (y = Convert.ToInt32(y1); y <= y2; y++)
                {
                    PosicionaX(ptr, imagem.Width, imagem.Height, padding, (int)x, (int)y, cor);
                    if (d <= 0)
                    {
                        d += incE;
                    }
                    else
                    {
                        d += incNE;
                        x += declive;
                    }
                    // Ponteiro anda uma linha
                    ptr += imagem.Width * 3 + padding;
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

    }


}
