using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGraphic.Models
{
    public class Objeto3d
    {
        public double[,] MatrizAcumulada { get; set; }
        public List<Vertice> ListaVerticesOriginais { get; set; }
        public List<Vertice> ListaVerticesAtuais { get; set; }
        // ListaFaces tem uma lista interna que guarda a posição da lista de vertices
        public List<List<int>> ListaFaces { get; set; }
        // Vai ter {x,y,z} só que um para cada face, vai ser do tamanho
        // da lista de faces
        public List<Vertice> ListaNormaisFaces { get; set; }
        // Vai ter {x,y,z} só que um para cada vertice, vai ser do tamanho
        // da lista de vertices, nesse caso aqui é média das normais da faces
        // que incidem nele
        public List<Vertice> ListaNormaisVertices { get; set; }

        public Objeto3d()
        {
            InicializaMatriz();
            ListaVerticesOriginais = new List<Vertice>();
            ListaVerticesAtuais = new List<Vertice>();
            ListaFaces = new List<List<int>>();
            ListaNormaisFaces = new List<Vertice>();
            ListaNormaisVertices = new List<Vertice>();
        }

        private void InicializaMatriz()
        {
            MatrizAcumulada = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                MatrizAcumulada[i, i] = 1;
            }
        }

        public void DesenharPoligono(Bitmap imagem, PictureBox pictureBox, int iniX, int iniY)
        {

            if (ListaVerticesAtuais.Count > 1)
            {
                int ate = ListaFaces.Count - 1;
                for (int i = 0; i < ate; i++)
                {
                    // --> DUVIDA
                    // Não precisa passar o Z, porque aqui considera ele como 0
                    Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][0]].X + iniX, ListaVerticesAtuais[ListaFaces[i][0]].Y + iniY,
                        ListaVerticesAtuais[ListaFaces[i][1]].X + iniX, ListaVerticesAtuais[ListaFaces[i][1]].Y + iniY, imagem);

                    Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][1]].X + iniX, ListaVerticesAtuais[ListaFaces[i][1]].Y + iniY,
                        ListaVerticesAtuais[ListaFaces[i][2]].X + iniX, ListaVerticesAtuais[ListaFaces[i][2]].Y + iniY, imagem);

                    Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][2]].X + iniX, ListaVerticesAtuais[ListaFaces[i][2]].Y + iniY,
                        ListaVerticesAtuais[ListaFaces[i][0]].X + iniX, ListaVerticesAtuais[ListaFaces[i][0]].Y + iniY, imagem);
                }

                //pictureBox.Image = imagem;
            }


            //if (this.Pontos.Count > 1)
            //{
            //    int ate = this.Pontos.Count - 1;
            //    for (int i = 0; i < ate; i++)
            //    {
            //        Ponto.PontoMedio(this.Pontos[i].X, this.Pontos[i].Y,
            //        this.Pontos[i + 1].X, this.Pontos[i + 1].Y, imagem);
            //    }
            //    Ponto.PontoMedio(this.Pontos[ate].X, this.Pontos[ate].Y,
            //        this.Pontos[0].X, this.Pontos[0].Y, imagem);

            //    if (_colorido)
            //    {
            //        //FloodFill(imagem, pictureBox);
            //        Rasterizacao(pictureBox, imagem);
            //    }


            //    pictureBox.Image = imagem;
            //}

        }

        public void PreencherComVertices(DataGridView dtGrid)
        {
            dtGrid.Rows.Clear();
            foreach (var vertice in ListaVerticesAtuais)
            {
                dtGrid.Rows.Add(new object[]
                {
                    vertice.X,
                    vertice.Y,
                    vertice.Z
                });
            }
        }



    }

    
}
