using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGraphic.Models.Rasterizacao
{
    public class EdgeTable
    {
        public List<ItemEdgeTable>[] ET { get; set; }

        public EdgeTable(int maxY)
        {
            ET = new List<ItemEdgeTable>[maxY];
            for (int i = 0; i < maxY; i++)
            {
                ET[i] = new List<ItemEdgeTable>();
            }
        }

        public void Inicializar(List<Vertice> pontos)
        {
            double Ymax, Ymin, Xmin, IncX;
            Vertice cx1, cx2;
            for (int i = 0; i < pontos.Count - 1; i++)
            {
                cx1 = pontos[i];
                cx2 = pontos[i + 1];
                if (cx1.Y > cx2.Y)
                {
                    Ymax = cx1.Y;
                    Xmin = cx2.X;
                    Ymin = cx2.Y;
                    IncX = (double)(cx1.X - cx2.X) / (cx1.Y - cx2.Y);
                }
                else
                {
                    Ymax = cx2.Y;
                    Xmin = cx1.X;
                    Ymin = cx1.Y;
                    IncX = (double)(cx2.X - cx1.X) / (cx2.Y - cx1.Y);
                }

                Adicionar((int)Ymin, Ymax, Xmin, IncX);
            }

            cx1 = pontos[0];
            cx2 = pontos[pontos.Count - 1];
            if (cx1.Y > cx2.Y)
            {
                Ymax = cx1.Y;
                Xmin = cx2.X;
                Ymin = cx2.Y;
                IncX = (double)(cx1.X - cx2.X) / (cx1.Y - cx2.Y);
            }
            else
            {
                Ymax = cx2.Y;
                Xmin = cx1.X;
                Ymin = cx1.Y;
                IncX = (double)(cx2.X - cx1.X) / (cx2.Y - cx1.Y);
            }

            Adicionar((int)Ymin, Ymax, Xmin, IncX);
        }

        public void Adicionar(int Y, double Ymax, double Xmin, double IncX)
        {
            ET[Y].Add(new ItemEdgeTable(Ymax, Xmin, IncX));
        }

        public void Preencher(Bitmap imagem, Color cor, PictureBox pictureBox)
        {
            List<ItemEdgeTable> AET = new List<ItemEdgeTable>();
            for (int y = 0; y < ET.Length; y++)
            {
                // Adiciona a AET
                foreach (var item in ET[y])
                {
                    AET.Add(new ItemEdgeTable(item.Ymax, item.Xmin, item.IncX));
                }
                if (AET.Count > 0)
                {
                    // Retirar Ymax == Y
                    for (int pos = AET.Count - 1; pos >= 0; pos--)
                    {
                        if (AET[pos].Ymax == y)
                        {
                            AET.RemoveAt(pos);
                        }
                    }
                    // Ordenar pelo Xmin                    
                    AET.Sort((Cx1, Cx2) => Cx1.Xmin.CompareTo(Cx2.Xmin));
                    // Desenha se tive ao menor 2 caixas
                    for (int pos = 0; pos < AET.Count - 1; pos += 2)
                    {
                        for (int x = (int)AET[pos].Xmin; x < (int)AET[pos + 1].Xmin; x++)
                        {
                            // TIRA ISSO DEPOIS COLOQUEI PQ TAVA DANDO BUG
                            if (x > 0 && x < pictureBox.Width &&
                            y > 0 && y < pictureBox.Height)
                            {
                                imagem.SetPixel(x, y, cor);
                            }
                        }
                    }
                    // Incrementar o Xmin
                    foreach (var item in AET)
                    {
                        item.Incrementa();
                    }
                }
            }
        }

    }
}
