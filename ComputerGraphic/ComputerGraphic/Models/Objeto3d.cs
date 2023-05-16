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

        public List<Vertice> ListaNormaisFacesOriginais { get; set; }
        public List<Vertice> ListaNormaisFacesAtuais { get; set; }



        // Vai ter {x,y,z} só que um para cada vertice, vai ser do tamanho
        // da lista de vertices, nesse caso aqui é média das normais da faces
        // que incidem nele
        public List<Vertice> ListaNormaisVerticesOriginais { get; set; }
        public List<Vertice> ListaNormaisVerticesAtuais { get; set; }

        public Objeto3d()
        {
            InicializaMatrizAcumulada();
            
            Centroide = new double[3];
            
            ListaVerticesOriginais = new List<Vertice>();
            ListaVerticesAtuais = new List<Vertice>();
            
            ListaFaces = new List<List<int>>();
            
            ListaNormaisFacesOriginais = new List<Vertice>();
            ListaNormaisFacesAtuais = new List<Vertice>();
            
            ListaNormaisVerticesOriginais = new List<Vertice>();
            ListaNormaisVerticesAtuais = new List<Vertice>();
        }

        private void InicializaMatrizAcumulada()
        {
            MatrizAcumulada = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                MatrizAcumulada[i, i] = 1;
            }
        }

        public void inicializaNormais()
        {
            double a, b, c, d, e, f;
            double i, j, k;
            double N;
            double nI, nJ, nK;
            int ultimo = ListaFaces[0].Count() - 1;
            foreach (var face in ListaFaces)
            {
                a = ListaVerticesOriginais[face[1]].X - ListaVerticesOriginais[face[0]].X;
                b = ListaVerticesOriginais[face[1]].Y - ListaVerticesOriginais[face[0]].Y;
                c = ListaVerticesOriginais[face[1]].Z - ListaVerticesOriginais[face[0]].Z;

                d = ListaVerticesOriginais[face[ultimo]].X - ListaVerticesOriginais[face[0]].X;
                e = ListaVerticesOriginais[face[ultimo]].Y - ListaVerticesOriginais[face[0]].Y;
                f = ListaVerticesOriginais[face[ultimo]].Z - ListaVerticesOriginais[face[0]].Z;

                i = (b * f) - (c * e);
                j = (c * d) - (f * a);
                k = (a * e) - (b * d);

                N = Math.Sqrt(i * i + j * j + k * k);

                //nI = i / N;
                //nJ = j / N;
                //nK = k / N;

                nI = i ;
                nJ = j ;
                nK = k ;

                ListaNormaisFacesOriginais.Add(new Vertice(nI, nJ, nK));
                ListaNormaisFacesAtuais.Add(new Vertice(nI, nJ, nK));
            }

            for (int pos = 0; pos < ListaFaces.Count(); pos++)
            {
                foreach (var posVertice in ListaFaces[pos])
                {
                    ListaNormaisVerticesOriginais[posVertice].X += ListaNormaisFacesAtuais[pos].X;
                    ListaNormaisVerticesOriginais[posVertice].Y += ListaNormaisFacesAtuais[pos].Y;
                    ListaNormaisVerticesOriginais[posVertice].Z += ListaNormaisFacesAtuais[pos].Z;
                    ListaNormaisVerticesOriginais[posVertice].NumFaceCompartilhadas++;

                    ListaNormaisVerticesAtuais[posVertice].X += ListaNormaisFacesAtuais[pos].X;
                    ListaNormaisVerticesAtuais[posVertice].Y += ListaNormaisFacesAtuais[pos].Y;
                    ListaNormaisVerticesAtuais[posVertice].Z += ListaNormaisFacesAtuais[pos].Z;
                    ListaNormaisVerticesAtuais[posVertice].NumFaceCompartilhadas++;
                }
            }

            for (int pos = 0; pos < ListaNormaisVerticesOriginais.Count(); pos++)
            {
                ListaNormaisVerticesOriginais[pos].X /= ListaNormaisVerticesOriginais[pos].NumFaceCompartilhadas;
                ListaNormaisVerticesOriginais[pos].Y /= ListaNormaisVerticesOriginais[pos].NumFaceCompartilhadas;
                ListaNormaisVerticesOriginais[pos].Z /= ListaNormaisVerticesOriginais[pos].NumFaceCompartilhadas;

                ListaNormaisVerticesAtuais[pos].X /= ListaNormaisVerticesAtuais[pos].NumFaceCompartilhadas;
                ListaNormaisVerticesAtuais[pos].Y /= ListaNormaisVerticesAtuais[pos].NumFaceCompartilhadas;
                ListaNormaisVerticesAtuais[pos].Z /= ListaNormaisVerticesAtuais[pos].NumFaceCompartilhadas;
            }
        }

        public void MudarVista(char atualP1, char atualP2, char novoP1, char novoP2)
        {
            double aux;
            if (atualP1 == 'X' && atualP2 == 'Y')
            {
                if (novoP1 == 'X' && novoP2 == 'Z')
                {
                    // --> X, Z, Y
                    foreach (var vertice in ListaVerticesAtuais)
                    {                        
                        // aux = Y                        
                        aux = vertice.Y;
                        // Y = Z
                        vertice.Y = vertice.Z;
                        // Z = Y
                        vertice.Z = aux;    
                    }
                }
                else
                {
                    // --> Y, Z, X
                    foreach (var vertice in ListaVerticesAtuais)
                    {
                        // aux = Y                        
                        aux = vertice.Y;
                        // Y = Z
                        vertice.Y = vertice.Z;
                        // Z = Y
                        vertice.Z = aux;

                        aux = vertice.X;
                        vertice.X = vertice.Z;
                        vertice.Z = aux;
                        // --> Y, Z, X
                    }                 
                }
            }
            else
            {
                if (atualP1 == 'X' && atualP2 == 'Z')
                {
                    if (novoP1 == 'X' && novoP2 == 'Y')
                    {
                        // --> X, Y, Z
                        foreach (var vertice in ListaVerticesAtuais)
                        {
                            // aux = Y                        
                            aux = vertice.Y;
                            // Y = Z
                            vertice.Y = vertice.Z;
                            // Z = aux
                            vertice.Z = aux;
                        }
                    }
                    else
                    {
                        // --> Y, Z, X
                        foreach (var vertice in ListaVerticesAtuais)
                        {
                            // aux = X                        
                            aux = vertice.X;
                            // X = Z
                            vertice.X = vertice.Z;
                            // Z = aux
                            vertice.Z = aux;
                        }                       
                    }
                }
                else
                {
                    if (atualP1 == 'Y' && atualP2 == 'Z')
                    {
                        if (novoP1 == 'X' && novoP2 == 'Y')
                        {
                            // --> X, Y, Z
                            foreach (var vertice in ListaVerticesAtuais)
                            {
                                // aux = X                        
                                aux = vertice.X;
                                // Y = Z
                                vertice.X = vertice.Y;
                                // Z = aux
                                vertice.Y = aux;
                                // --> Z, Y, X

                                // aux = X                        
                                aux = vertice.X;
                                // X = Z
                                vertice.X = vertice.Z;
                                // Z = aux
                                vertice.Z = aux;
                            }
                        }
                        else
                        {
                            // --> Y, Z, X

                            // --> X, Z, Y
                            foreach (var vertice in ListaVerticesAtuais)
                            {
                                // aux = X                        
                                aux = vertice.X;
                                // X = Z
                                vertice.X = vertice.Z;
                                // Z = X
                                vertice.Z = aux;
                            }
                        }
                    }
                }
            }


            InicializaMatrizAcumulada();
            ListaVerticesOriginais = new List<Vertice>(ListaVerticesAtuais);
        }

        public void Desenhar(Bitmap imagem, PictureBox pictureBox, int corPincel, bool faceOculta)
        {
            if (ListaVerticesAtuais.Count > 1)
            {
                int width = pictureBox.Width/2;
                int height = pictureBox.Height/2;

                int totalFace = ListaFaces.Count ;
                int totalVertice;
                if (faceOculta)
                {
                    for (int i = 0; i < totalFace; i++)
                    {

                        if (ListaNormaisFacesAtuais[i].Z >= 0) frt
                        {
                            totalVertice = ListaFaces[i].Count - 1;
                            for (int j = 0; j < totalVertice; j++)
                            {
                                Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][j]].X + width, ListaVerticesAtuais[ListaFaces[i][j]].Y + height,
                                    ListaVerticesAtuais[ListaFaces[i][j + 1]].X + width, ListaVerticesAtuais[ListaFaces[i][j + 1]].Y + height, imagem, corPincel);
                            }
                            Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][totalVertice]].X + width, ListaVerticesAtuais[ListaFaces[i][totalVertice]].Y + +height,
                                ListaVerticesAtuais[ListaFaces[i][0]].X + width, ListaVerticesAtuais[ListaFaces[i][0]].Y + height, imagem, corPincel);
                        }

                        // Preenchimento
                        //EdgeTable lista = new EdgeTable(pictureBox.Height);
                        //lista.Inicializar(new List<Vertice>() { 
                        //    ListaVerticesAtuais[ListaFaces[i][0]], 
                        //    ListaVerticesAtuais[ListaFaces[i][1]],
                        //    ListaVerticesAtuais[ListaFaces[i][2]]
                        //});

                        //lista.Preencher(imagem, Color.Yellow, pictureBox);

                    }
                }
                else
                {
                    for (int i = 0; i < totalFace; i++)
                    {
                        totalVertice = ListaFaces[i].Count - 1;
                        for (int j = 0; j < totalVertice; j++)
                        {
                            Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][j]].X + width, ListaVerticesAtuais[ListaFaces[i][j]].Y + height,
                                ListaVerticesAtuais[ListaFaces[i][j + 1]].X + width, ListaVerticesAtuais[ListaFaces[i][j + 1]].Y + height, imagem, corPincel);
                        }
                        Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][totalVertice]].X + width, ListaVerticesAtuais[ListaFaces[i][totalVertice]].Y + +height,
                            ListaVerticesAtuais[ListaFaces[i][0]].X + width, ListaVerticesAtuais[ListaFaces[i][0]].Y + height, imagem, corPincel);
                        

                        // Preenchimento
                        //EdgeTable lista = new EdgeTable(pictureBox.Height);
                        //lista.Inicializar(new List<Vertice>() { 
                        //    ListaVerticesAtuais[ListaFaces[i][0]], 
                        //    ListaVerticesAtuais[ListaFaces[i][1]],
                        //    ListaVerticesAtuais[ListaFaces[i][2]]
                        //});

                        //lista.Preencher(imagem, Color.Yellow, pictureBox);

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
                int width = pictureBox.Width / 2;
                int height = pictureBox.Height / 2;

                int totalFace = ListaFaces.Count;
                int totalVertice;
                for (int i = 0; i < totalFace; i++)
                {
                    totalVertice = ListaFaces[i].Count - 1;
                    for (int j = 0; j < totalVertice; j++)
                    {
                        Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][j]].X + width, ListaVerticesAtuais[ListaFaces[i][j]].Y + height,
                            ListaVerticesAtuais[ListaFaces[i][j + 1]].X + width, ListaVerticesAtuais[ListaFaces[i][j + 1]].Y + height, imagem, corBoracha);
                    }
                    Vertice.PontoMedio(ListaVerticesAtuais[ListaFaces[i][totalVertice]].X + width, ListaVerticesAtuais[ListaFaces[i][totalVertice]].Y + height,
                        ListaVerticesAtuais[ListaFaces[i][0]].X + width, ListaVerticesAtuais[ListaFaces[i][0]].Y + height, imagem, corBoracha);
                    
                }

                pictureBox.Image = imagem;
            }
        }

        public void PreencherComVertices(DataGridView dtGrid)
        {
            dtGrid.Rows.Clear();
            foreach (var vertice in ListaNormaisFacesAtuais)
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
            double normalX, normalY, normalZ;
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

            // <--------- NORMAL FACE --------->
            ListaNormaisFacesAtuais.Clear();
            foreach (var normalFace in ListaNormaisFacesOriginais)
            {
                normalX = normalFace.X * MatrizAcumulada[0, 0] + normalFace.Y * MatrizAcumulada[0, 1] + normalFace.Z * MatrizAcumulada[0, 2] + MatrizAcumulada[0, 3];
                normalY = normalFace.X * MatrizAcumulada[1, 0] + normalFace.Y * MatrizAcumulada[1, 1] + normalFace.Z * MatrizAcumulada[1, 2] + MatrizAcumulada[1, 3];
                normalZ = normalFace.X * MatrizAcumulada[2, 0] + normalFace.Y * MatrizAcumulada[2, 1] + normalFace.Z * MatrizAcumulada[2, 2] + MatrizAcumulada[2, 3];

                ListaNormaisFacesAtuais.Add(new Vertice(normalX, normalY, normalZ));
            }

            // <--------- NORMAL VERTICE --------->
            ListaNormaisVerticesAtuais.Clear();
            foreach (var normalVertice in ListaNormaisVerticesOriginais)
            {
                normalX = normalVertice.X * MatrizAcumulada[0, 0] + normalVertice.Y * MatrizAcumulada[0, 1] + normalVertice.Z * MatrizAcumulada[0, 2] + MatrizAcumulada[0, 3];
                normalY = normalVertice.X * MatrizAcumulada[1, 0] + normalVertice.Y * MatrizAcumulada[1, 1] + normalVertice.Z * MatrizAcumulada[1, 2] + MatrizAcumulada[1, 3];
                normalZ = normalVertice.X * MatrizAcumulada[2, 0] + normalVertice.Y * MatrizAcumulada[2, 1] + normalVertice.Z * MatrizAcumulada[2, 2] + MatrizAcumulada[2, 3];

                ListaNormaisVerticesAtuais.Add(new Vertice(normalX, normalY, normalZ));
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
