using ComputerGraphic.Models.Rasterizacao;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGraphic.Models
{
    public class Objeto3d
    {
        public double[] Centroide { get; set; }
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

        // -- VER SE PODE TER UMA PARA DESENHAR
        public List<Vertice> ListaDesenhar { get; set; }



        public Objeto3d()
        {
            InicializaMatrizAcumulada();
            
            Centroide = new double[3];
            
            ListaVerticesOriginais = new List<Vertice>();
            ListaVerticesAtuais = new List<Vertice>();
            
            ListaFaces = new List<List<int>>();
            
            ListaNormaisFaces = new List<Vertice>();
            ListaNormaisVertices = new List<Vertice>();

            ListaDesenhar = new List<Vertice>();
        }

        private void InicializaMatrizAcumulada()
        {
            MatrizAcumulada = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                MatrizAcumulada[i, i] = 1;
            }
        }

        public void CalculaNormais()
        {
            double a, b, c, d, e, f;
            double i, j, k;
            double N;
            double nI, nJ, nK;
            int ultimo;

            ListaNormaisFaces.Clear();
            foreach (var face in ListaFaces)
            {
                ultimo = face.Count - 1;

                a = ListaVerticesAtuais[face[1]].X - ListaVerticesAtuais[face[0]].X;
                b = ListaVerticesAtuais[face[1]].Y - ListaVerticesAtuais[face[0]].Y;
                c = ListaVerticesAtuais[face[1]].Z - ListaVerticesAtuais[face[0]].Z;

                d = ListaVerticesAtuais[face[ultimo]].X - ListaVerticesAtuais[face[0]].X;
                e = ListaVerticesAtuais[face[ultimo]].Y - ListaVerticesAtuais[face[0]].Y;
                f = ListaVerticesAtuais[face[ultimo]].Z - ListaVerticesAtuais[face[0]].Z;

                i = (b * f) - (c * e);
                j = (c * d) - (f * a);
                k = (a * e) - (b * d);

                N = Math.Sqrt(i * i + j * j + k * k);

                nI = i / N;
                nJ = j / N;
                nK = k / N;

                ListaNormaisFaces.Add(new Vertice(nI, nJ, nK));
            }

            ListaNormaisVertices.Clear();
            foreach (var vertice in ListaVerticesAtuais)
            {
                ListaNormaisVertices.Add(new Vertice(0, 0, 0));
            }

            for (int pos = 0; pos < ListaNormaisVertices.Count(); pos++)
            {
                ListaNormaisVertices[pos].X /= ListaNormaisVertices[pos].NumFaceCompartilhadas;
                ListaNormaisVertices[pos].Y /= ListaNormaisVertices[pos].NumFaceCompartilhadas;
                ListaNormaisVertices[pos].Z /= ListaNormaisVertices[pos].NumFaceCompartilhadas;
            }
        }

        public void Desenhar(Bitmap imagem, PictureBox pictureBox, int corPincel, bool faceOculta, int obliqua = 0)
        {
            if (ListaDesenhar.Count > 1)
            {
                double x1, y1, x2, y2;

                int width = pictureBox.Width/2;
                int height = pictureBox.Height/2;

                int totalFace = ListaFaces.Count;
                int totalVertice;
                if (faceOculta)
                {
                    for (int i = 0; i < totalFace; i++)
                    {
                        if (ListaNormaisFaces[i].Z >= 0)
                        {
                            totalVertice = ListaFaces[i].Count - 1;
                            for (int j = 0; j < totalVertice; j++)
                            {
                                x1 = ListaDesenhar[ListaFaces[i][j]].X + width;
                                y1 = ListaDesenhar[ListaFaces[i][j]].Y + height;
                                x2 = ListaDesenhar[ListaFaces[i][j + 1]].X + width;
                                y2 = ListaDesenhar[ListaFaces[i][j + 1]].Y + height;

                                Vertice.PontoMedio(x1, y1, x2, y2, imagem, corPincel);
                            }
                            x1 = ListaDesenhar[ListaFaces[i][totalVertice]].X + width;
                            y1 = ListaDesenhar[ListaFaces[i][totalVertice]].Y + height;
                            x2 = ListaDesenhar[ListaFaces[i][0]].X + width;
                            y2 = ListaDesenhar[ListaFaces[i][0]].Y + height;

                            Vertice.PontoMedio(x1, y1, x2, y2, imagem, corPincel);

                        }
                    }
                }
                else
                {
                    for (int i = 0; i < totalFace; i++)
                    {
                        totalVertice = ListaFaces[i].Count - 1;
                        for (int j = 0; j < totalVertice; j++)
                        {
                            x1 = ListaDesenhar[ListaFaces[i][j]].X + width;
                            y1 = ListaDesenhar[ListaFaces[i][j]].Y + height;
                            x2 = ListaDesenhar[ListaFaces[i][j + 1]].X + width;
                            y2 = ListaDesenhar[ListaFaces[i][j + 1]].Y + height;

                            Vertice.PontoMedio(x1, y1, x2, y2, imagem, corPincel);
                        }
                        x1 = ListaDesenhar[ListaFaces[i][totalVertice]].X + width;
                        y1 = ListaDesenhar[ListaFaces[i][totalVertice]].Y + height;
                        x2 = ListaDesenhar[ListaFaces[i][0]].X + width;
                        y2 = ListaDesenhar[ListaFaces[i][0]].Y + height;

                        Vertice.PontoMedio(x1, y1, x2, y2, imagem, corPincel);
                                
                    }
                           
                }

               


                // Aqui arrumar para pintar, pois, tem que passar tambem a lista de faces
                //
                //lista.Inicializar(ListaVerticesAtuais);

                pictureBox.Image = imagem;
            }
        }

        public void LimpaTela(Bitmap imagem, PictureBox pictureBox, int corBoracha)
        {
            if (ListaVerticesAtuais.Count > 1)
            {
                double x1, y1, x2, y2;

                int width = pictureBox.Width / 2;
                int height = pictureBox.Height / 2;

                int totalFace = ListaFaces.Count;
                int totalVertice;


                for (int i = 0; i < totalFace; i++)
                {
                    totalVertice = ListaFaces[i].Count - 1;
                    for (int j = 0; j < totalVertice; j++)
                    {
                        x1 = ListaDesenhar[ListaFaces[i][j]].X + width;
                        y1 = ListaDesenhar[ListaFaces[i][j]].Y + height;
                        x2 = ListaDesenhar[ListaFaces[i][j + 1]].X + width;
                        y2 = ListaDesenhar[ListaFaces[i][j + 1]].Y + height;

                        Vertice.PontoMedio(x1, y1, x2, y2, imagem, corBoracha);
                    }
                    x1 = ListaDesenhar[ListaFaces[i][totalVertice]].X + width;
                    y1 = ListaDesenhar[ListaFaces[i][totalVertice]].Y + height;
                    x2 = ListaDesenhar[ListaFaces[i][0]].X + width;
                    y2 = ListaDesenhar[ListaFaces[i][0]].Y + height;

                    Vertice.PontoMedio(x1, y1, x2, y2, imagem, corBoracha);

                }


                pictureBox.Image = imagem;
            }
        }

        public void PreencherComVertices(DataGridView dtGrid)
        {
            dtGrid.Rows.Clear();
            foreach (var vertice in ListaNormaisFaces)
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
            double cX, cY, cZ;
            cX = cY = cZ = 0;            
            ListaVerticesAtuais.Clear();
            foreach (var vertice in ListaVerticesOriginais)
            {
                x = (int)(vertice.X * MatrizAcumulada[0, 0] + vertice.Y * MatrizAcumulada[0, 1] + vertice.Z * MatrizAcumulada[0, 2] + MatrizAcumulada[0, 3]);
                y = (int)(vertice.X * MatrizAcumulada[1, 0] + vertice.Y * MatrizAcumulada[1, 1] + vertice.Z * MatrizAcumulada[1, 2] + MatrizAcumulada[1, 3]);
                z = (int)(vertice.X * MatrizAcumulada[2, 0] + vertice.Y * MatrizAcumulada[2, 1] + vertice.Z * MatrizAcumulada[2, 2] + MatrizAcumulada[2, 3]);

                cX += x;
                cY += y;
                cZ += z;
                
                ListaVerticesAtuais.Add(new Vertice(x, y, z));
            }

            Centroide[0] = cX / ListaVerticesAtuais.Count;
            Centroide[1] = cY / ListaVerticesAtuais.Count;
            Centroide[2] = cZ / ListaVerticesAtuais.Count;

            CalculaNormais();
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

        public void Projecao(bool ortografica, CheckedListBox listOrtografica, CheckedListBox listObliqua)
        {
            double x, y, z;
            if (ortografica)
            {

                int selecionado = 0;
                int total = listOrtografica.Items.Count;
                for (int i = 0; i < total; i++)
                {
                    if (selecionado != i)
                    {
                        if (listOrtografica.GetItemChecked(i))
                        {
                            selecionado = i;
                        }
                    }
                }

                if (selecionado == 0)
                {
                    ListaDesenhar.Clear();
                    foreach (var vertice in ListaVerticesAtuais)
                    {
                        x = vertice.X;
                        y = vertice.Y;
                        z = vertice.Z;

                        ListaDesenhar.Add(new Vertice(x, y, z));
                    }
                } 
                else
                {
                    if (selecionado == 1)
                    {
                        ListaDesenhar.Clear();
                        foreach (var vertice in ListaVerticesAtuais)
                        {
                            x = vertice.X;
                            y = vertice.Y;
                            z = vertice.Z;

                            ListaDesenhar.Add(new Vertice(x, z, y));
                        }
                    }
                    else
                    {
                        if (selecionado == 2)
                        {
                            ListaDesenhar.Clear();
                            foreach (var vertice in ListaVerticesAtuais)
                            {
                                x = vertice.X;
                                y = vertice.Y;
                                z = vertice.Z;

                                ListaDesenhar.Add(new Vertice(y, z, x));
                            }
                        }
                    }
                }
            }
            else
            {
                int selecionado = 0;
                int total = listObliqua.Items.Count;
                for (int i = 0; i < total; i++)
                {
                    if (selecionado != i)
                    {
                        if (listObliqua.GetItemChecked(i))
                        {
                            selecionado = i;
                        }
                    }
                }

                double l, graus;
                if (selecionado == 0)
                {
                    graus = 45;
                    l = 1;
                    ListaDesenhar.Clear();
                    foreach (var vertice in ListaVerticesAtuais)
                    {
                        x = vertice.X + vertice.Z * (l * Math.Cos((Math.PI / 180) * graus));
                        y = vertice.Y + vertice.Z * (l * Math.Sin((Math.PI / 180) * graus));
                        z = 0;

                        ListaDesenhar.Add(new Vertice(x, y, z));
                    }
                }
                else
                {
                    if (selecionado == 1)
                    {
                        graus = 63.4;
                        l = 0.5;
                        ListaDesenhar.Clear();
                        foreach (var vertice in ListaVerticesAtuais)
                        {
                            x = vertice.X + vertice.Z * (l * Math.Cos((Math.PI / 180) * graus));
                            y = vertice.Y + vertice.Z * (l * Math.Sin((Math.PI / 180) * graus));
                            z = 0;

                            ListaDesenhar.Add(new Vertice(x, y, z));
                        }
                    }
                }
            }
        }

        public void Escala(double sX, double sY, double sZ)
        {   
            double[,] matrizEscala = new double[4, 4];
            double[,] matrizTranslacao_origem = new double[4, 4];
            double[,] matrizTranslacao_centroide = new double[4, 4];
            double[,] matrizResultante_1 = new double[4, 4];
            double[,] matrizResultante_2 = new double[4, 4];
            double[,] novaMatrizAcumulada = new double[4, 4];

            // Matriz Identidade
            for (int i = 0; i < 4; i++)
            {
                matrizEscala[i, i] = 1;
                matrizTranslacao_origem[i, i] = 1;
                matrizTranslacao_centroide[i, i] = 1;
            }

            matrizEscala[0, 0] = sX;
            matrizEscala[1, 1] = sY;
            matrizEscala[2, 2] = sZ;

            matrizTranslacao_origem[0, 3] = -Centroide[0]; // X
            matrizTranslacao_origem[1, 3] = -Centroide[1]; // Y
            matrizTranslacao_origem[2, 3] = -Centroide[2]; // Z

            matrizTranslacao_centroide[0, 3] = Centroide[0]; // X
            matrizTranslacao_centroide[1, 3] = Centroide[1]; // Y
            matrizTranslacao_centroide[2, 3] = Centroide[2]; // Y


            // NovaMA = T(cent) * E * T(ori) * MA
            // matrizResultante_1 = T(cent) * E
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        matrizResultante_1[linha, coluna] += matrizTranslacao_centroide[linha, i] * matrizEscala[i, coluna];
                    }
                }
            }
            // matrizResultante_2 = matrizResultante_1 *  T(ori)
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        matrizResultante_2[linha, coluna] += matrizResultante_1[linha, i] * matrizTranslacao_origem[i, coluna];
                    }
                }
            }
            // NovaMA = matrizResultante_2 *  MA
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        novaMatrizAcumulada[linha, coluna] += matrizResultante_2[linha, i] * this.MatrizAcumulada[i, coluna];
                    }
                }
            }

            this.MatrizAcumulada = novaMatrizAcumulada;

            MatrizAcumuladaVertices();


        }

        public void RotacaoX(int graus)
        {
            double radiano = (Math.PI / 180) * graus;
            double[,] matrizRotacao = new double[4, 4];
            double[,] matrizTranslacao_origem = new double[4, 4];
            double[,] matrizTranslacao_centroide = new double[4, 4];
            double[,] matrizResultante_1 = new double[4, 4];
            double[,] matrizResultante_2 = new double[4, 4];
            double[,] novaMatrizAcumulada = new double[4, 4];

            // Matriz Identidade
            for (int i = 0; i < 4; i++)
            {
                matrizRotacao[i, i] = 1;
                matrizTranslacao_origem[i, i] = 1;
                matrizTranslacao_centroide[i, i] = 1;
            }

            matrizTranslacao_origem[0, 3] = -Centroide[0];
            matrizTranslacao_origem[1, 3] = -Centroide[1];
            matrizTranslacao_origem[2, 3] = -Centroide[2];

            matrizTranslacao_centroide[0, 3] = Centroide[0];
            matrizTranslacao_centroide[1, 3] = Centroide[1];
            matrizTranslacao_centroide[2, 3] = Centroide[2];

            // Para X
            matrizRotacao[1, 1] = Math.Cos(radiano);
            matrizRotacao[1, 2] = -Math.Sin(radiano);
            matrizRotacao[2, 1] = Math.Sin(radiano);
            matrizRotacao[2, 2] = Math.Cos(radiano);

            // MA_Nova = T(cent) * R * T(ori) * P(MA)
            // matrizResultante_1 = T(cent) * R 
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        matrizResultante_1[linha, coluna] += matrizTranslacao_centroide[linha, i] * matrizRotacao[i, coluna];
                    }
                }
            }
            // matrizResultante_2 = matrizResultante_1 * T(ori)
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        matrizResultante_2[linha, coluna] += matrizResultante_1[linha, i] * matrizTranslacao_origem[i, coluna];
                    }
                }
            }
            // Nova Matriz Acumulada
            // Nova_MatrizAcumulada = matrizResultante_2 * MatrizAcumulada
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        novaMatrizAcumulada[linha, coluna] += matrizResultante_2[linha, i] * this.MatrizAcumulada[i, coluna];
                    }
                }
            }

            this.MatrizAcumulada = novaMatrizAcumulada;

            MatrizAcumuladaVertices();
        }

        public void RotacaoY(int graus)
        {
            double radiano = (Math.PI / 180) * graus;
            double[,] matrizRotacao = new double[4, 4];
            double[,] matrizTranslacao_origem = new double[4, 4];
            double[,] matrizTranslacao_centroide = new double[4, 4];
            double[,] matrizResultante_1 = new double[4, 4];
            double[,] matrizResultante_2 = new double[4, 4];
            double[,] novaMatrizAcumulada = new double[4, 4];

            // Matriz Identidade
            for (int i = 0; i < 4; i++)
            {
                matrizRotacao[i, i] = 1;
                matrizTranslacao_origem[i, i] = 1;
                matrizTranslacao_centroide[i, i] = 1;
            }

            matrizTranslacao_origem[0, 3] = -Centroide[0];
            matrizTranslacao_origem[1, 3] = -Centroide[1];
            matrizTranslacao_origem[2, 3] = -Centroide[2];

            matrizTranslacao_centroide[0, 3] = Centroide[0];
            matrizTranslacao_centroide[1, 3] = Centroide[1];
            matrizTranslacao_centroide[2, 3] = Centroide[2];

            // Para Y
            matrizRotacao[0, 0] = Math.Cos(radiano);
            matrizRotacao[0, 2] = Math.Sin(radiano);
            matrizRotacao[2, 0] = -Math.Sin(radiano);
            matrizRotacao[2, 2] = Math.Cos(radiano);


            // MA_Nova = T(cent) * R * T(ori) * P(MA)
            // matrizResultante_1 = T(cent) * R 
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        matrizResultante_1[linha, coluna] += matrizTranslacao_centroide[linha, i] * matrizRotacao[i, coluna];
                    }
                }
            }
            // matrizResultante_2 = matrizResultante_1 * T(ori)
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        matrizResultante_2[linha, coluna] += matrizResultante_1[linha, i] * matrizTranslacao_origem[i, coluna];
                    }
                }
            }
            // Nova Matriz Acumulada
            // Nova_MatrizAcumulada = matrizResultante_2 * MatrizAcumulada
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        novaMatrizAcumulada[linha, coluna] += matrizResultante_2[linha, i] * this.MatrizAcumulada[i, coluna];
                    }
                }
            }

            this.MatrizAcumulada = novaMatrizAcumulada;

            MatrizAcumuladaVertices();
        }

        public void RotacaoZ(int graus)
        {
            double radiano = (Math.PI / 180) * graus;
            double[,] matrizRotacao = new double[4, 4];
            double[,] matrizTranslacao_origem = new double[4, 4];
            double[,] matrizTranslacao_centroide = new double[4, 4];
            double[,] matrizResultante_1 = new double[4, 4];
            double[,] matrizResultante_2 = new double[4, 4];
            double[,] novaMatrizAcumulada = new double[4, 4];

            // Matriz Identidade
            for (int i = 0; i < 4; i++)
            {
                matrizRotacao[i, i] = 1;
                matrizTranslacao_origem[i, i] = 1;
                matrizTranslacao_centroide[i, i] = 1;
            }

            matrizTranslacao_origem[0, 3] = -Centroide[0];
            matrizTranslacao_origem[1, 3] = -Centroide[1];
            matrizTranslacao_origem[2, 3] = -Centroide[2];

            matrizTranslacao_centroide[0, 3] = Centroide[0];
            matrizTranslacao_centroide[1, 3] = Centroide[1];
            matrizTranslacao_centroide[2, 3] = Centroide[2];

            // Para Z
            matrizRotacao[0, 0] = Math.Cos(radiano);
            matrizRotacao[0, 1] = -Math.Sin(radiano);
            matrizRotacao[1, 0] = Math.Sin(radiano);
            matrizRotacao[1, 1] = Math.Cos(radiano);

            // MA_Nova = T(cent) * R * T(ori) * P(MA)
            // matrizResultante_1 = T(cent) * R 
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        matrizResultante_1[linha, coluna] += matrizTranslacao_centroide[linha, i] * matrizRotacao[i, coluna];
                    }
                }
            }
            // matrizResultante_2 = matrizResultante_1 * T(ori)
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        matrizResultante_2[linha, coluna] += matrizResultante_1[linha, i] * matrizTranslacao_origem[i, coluna];
                    }
                }
            }
            // Nova Matriz Acumulada
            // Nova_MatrizAcumulada = matrizResultante_2 * MatrizAcumulada
            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 4; coluna++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        novaMatrizAcumulada[linha, coluna] += matrizResultante_2[linha, i] * this.MatrizAcumulada[i, coluna];
                    }
                }
            }

            this.MatrizAcumulada = novaMatrizAcumulada;

            MatrizAcumuladaVertices();
        }



    }

    
}
