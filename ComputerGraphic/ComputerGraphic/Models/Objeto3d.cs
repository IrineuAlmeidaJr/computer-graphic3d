﻿using System;
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

        public void Desenhar(Bitmap imagem, PictureBox pictureBox)
        {

            if (ListaVerticesAtuais.Count > 1)
            {
                int ate = ListaFaces.Count - 1;
                for (int i = 0; i < ate; i++)
                {
                    // --> DUVIDA
                    // Não precisa passar o Z, porque aqui considera ele como 0
                    Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][0]].X, ListaVerticesAtuais[ListaFaces[i][0]].Y,
                        ListaVerticesAtuais[ListaFaces[i][1]].X, ListaVerticesAtuais[ListaFaces[i][1]].Y, imagem);

                    Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][1]].X, ListaVerticesAtuais[ListaFaces[i][1]].Y,
                        ListaVerticesAtuais[ListaFaces[i][2]].X, ListaVerticesAtuais[ListaFaces[i][2]].Y, imagem);

                    Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][2]].X, ListaVerticesAtuais[ListaFaces[i][2]].Y,
                        ListaVerticesAtuais[ListaFaces[i][0]].X, ListaVerticesAtuais[ListaFaces[i][0]].Y, imagem);
                }

                

                pictureBox.Image = imagem;
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

        private void MatrizAcumuladaVertices()
        {
            int x, y, z;
            ListaVerticesAtuais.Clear();
            foreach (var vertice in ListaVerticesOriginais)
            {
                x = (int)(vertice.X * MatrizAcumulada[0, 0] + vertice.Y * MatrizAcumulada[0, 1] + vertice.Z * MatrizAcumulada[0, 2] + MatrizAcumulada[0, 3]);
                y = (int)(vertice.X * MatrizAcumulada[1, 0] + vertice.Y * MatrizAcumulada[1, 1] + vertice.Z * MatrizAcumulada[1, 2] + MatrizAcumulada[1, 3]);
                z = (int)(vertice.X * MatrizAcumulada[2, 0] + vertice.Y * MatrizAcumulada[2, 1] + vertice.Z * MatrizAcumulada[2, 2] + MatrizAcumulada[2, 3]);
                ListaVerticesAtuais.Add(new Vertice(x, y, z));
            }
        }

        public void Translacao(int tX, int tY, int tZ)
        {
            double[,] matrizTranslacao = new double[4, 4];
            double[,] novaMatrizAcumulada = new double[4, 4];

            // Matriz Identidade
            for (int i = 0; i < 4; i++)
            {
                matrizTranslacao[i, i] = 1;
            }

            // Matriz Translação
            matrizTranslacao[0, 3] = tX;
            matrizTranslacao[1, 3] = tY;
            matrizTranslacao[2, 3] = tZ;

            //  NovaMA = T * MA
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        novaMatrizAcumulada[linha, coluna] += matrizTranslacao[linha, i] * this.MatrizAcumulada[i, coluna];
                    }
                }
            }

            this.MatrizAcumulada = novaMatrizAcumulada;

            MatrizAcumuladaVertices();

        }

        public void Escala(int sx,  int sy, int sz)
        {

        }


    }

    
}
