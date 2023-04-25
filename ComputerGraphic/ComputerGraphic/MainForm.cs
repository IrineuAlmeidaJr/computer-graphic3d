using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComputerGraphic.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ComputerGraphic
{
    public partial class ComputerGraphic : Form
    {

        private Objeto3d _objeto3D;
        private Bitmap _imagem;
        private int _width;
        private int _height;

        private bool pX, pY, pZ;

        public ComputerGraphic()
        {
            InitializeComponent();
            _objeto3D = null;
            _width = pictureBox.Width;
            _height = pictureBox.Height;

            pX = pY = pZ = false;

            pictureBox.MouseWheel += PictureBoxMouseWheel;

            CarregarTela();
        }

        private void abrirObjeto3D_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    //ofd.Filter = "txt fil"
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamReader arquivo = new StreamReader(ofd.FileName))
                        {
                            double x, y, z;
                            string linha;
                            string[] dados;
                            string[] dadosFace;

                            _objeto3D = new Objeto3d();
                            CarregarTela();

                            NumberFormatInfo provider = new NumberFormatInfo();
                            provider.NumberDecimalSeparator = ".";
                            
                            if (FazEscala(arquivo))
                            {

                                double area = (_height + _width) / 4;
                                StreamReader arquivo2 = new StreamReader(ofd.FileName);
                                while ((linha = arquivo2.ReadLine()) != null)
                                {
                                    dados = linha.Split(' ');

                                    switch (dados[0])
                                    {
                                        case "v":

                                            x = Convert.ToDouble(dados[1], provider) * area;
                                            y = Convert.ToDouble(dados[2], provider) * area;
                                            z = Convert.ToDouble(dados[3], provider);

                                            _objeto3D.ListaVerticesOriginais
                                                .Add(new Vertice(x, y, z));
                                            _objeto3D.ListaVerticesAtuais
                                                .Add(new Vertice(x, y, z));
                                            break;

                                        case "f":
                                            List<int> posVertices = new List<int>();
                                            for (int i = 1; i < dados.Length; i++)
                                            {
                                                dadosFace = dados[i].Split('/');
                                                // Coloca "-1", porque, as posições no arquivo iniciam de 1,
                                                // mas, na lista inicia de 0
                                                posVertices.Add(Convert.ToInt32(dadosFace[0], provider) - 1);
                                            }

                                            _objeto3D.ListaFaces.Add(posVertices);
                                            break;
                                        default:
                                            Console.WriteLine("Nenhuma das Opções");
                                            break;
                                    }
                                }
                                // Translada para o Centro da Tela
                                _objeto3D.Translacao(_width / 2, _height / 8, 0);
                            }
                            else
                            {
                                StreamReader arquivo2 = new StreamReader(ofd.FileName);
                                while ((linha = arquivo2.ReadLine()) != null)
                                {
                                    dados = linha.Split(' ');

                                    switch (dados[0])
                                    {
                                        case "v":
                                            // Pode dar Erro na converção
                                            x = Convert.ToDouble(dados[1], provider);
                                            y = Convert.ToDouble(dados[2], provider);
                                            z = Convert.ToDouble(dados[3], provider);

                                            _objeto3D.ListaVerticesOriginais
                                                .Add(new Vertice(x, y, z));
                                            _objeto3D.ListaVerticesAtuais
                                                .Add(new Vertice(x, y, z));
                                            break;

                                        case "f":
                                            List<int> posVertices = new List<int>();
                                            for (int i = 1; i < dados.Length; i++)
                                            {
                                                dadosFace = dados[i].Split('/');
                                                // Coloca "-1", porque, as posições no arquivo iniciam de 1,
                                                // mas, na lista inicia de 0
                                                posVertices.Add(Convert.ToInt32(dadosFace[0], provider) - 1);
                                            }

                                            _objeto3D.ListaFaces.Add(posVertices);
                                            break;
                                        default:
                                            Console.WriteLine("Nenhuma das Opções");
                                            break;
                                    }
                                }
                                // Translada para o Centro da Tela
                                _objeto3D.Translacao(_width / 2, _height / 2, 0);
                            }

                            
                            // Desenha Objeto3D                                
                            CarregarTela();

                            _objeto3D.PreencherComVertices(dtGridVertices);
                        }
                       
                    }
                }
                
            }
            catch (Exception error)
            {
                Console.WriteLine("Exception: " + error.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }


        private void PictureBoxMouseWheel(object sender, MouseEventArgs e)
        {
            double escala = e.Delta > 0 ? 1.05 : 0.95;           

            if (pX)
            {
                _objeto3D.Escala(escala, 1, 1);
            } 
            else
            {
                if (pY)
                {
                    _objeto3D.Escala(1, escala, 1);
                }
                else
                {
                    if (pZ)
                    {
                        _objeto3D.Escala(1, 1, escala);
                    }
                    else
                    {
                        _objeto3D.Escala(escala, escala, escala);
                    }
                }
            }

            CarregarTela();
        }

        private void CarregarTela()
        {
            _imagem =
                new Bitmap(_width, _height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(_imagem);
            pictureBox.Image = _imagem; 
            
            if (_objeto3D != null )
            {
                _objeto3D.Desenhar(_imagem, pictureBox);
            }
            
        }

        public bool FazEscala(StreamReader arquivo)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            bool flag = true;
            string[] dados;
            string linha = arquivo.ReadLine();
            if (linha != null)
            {
                dados = linha.Split(' ');
                while (linha != null && dados[0] == "v" && flag)
                {
                    if (Math.Abs(Convert.ToDouble(dados[1], provider)) > 2
                    && Math.Abs(Convert.ToDouble(dados[2], provider)) > 2
                    && Math.Abs(Convert.ToDouble(dados[3], provider)) > 2)
                    {
                        flag = false;
                    }
                    linha = arquivo.ReadLine();
                    dados = linha.Split(' ');
                }                
            }

            return flag;
        }

        private void ComputerGraphic_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.X:
                    pX = true;
                    break;
                case Keys.Y: 
                    pY = true; 
                    break;
                case Keys.Z: 
                    pZ = true;
                    break;
            }
        }

        private void ComputerGraphic_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.X:
                    pX = false;
                    break;
                case Keys.Y:
                    pY = false;
                    break;
                case Keys.Z:
                    pZ = false;
                    break;
            }
        }

        
             
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_objeto3D != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    int tX = e.X - (int)(_objeto3D.ListaVerticesAtuais[0].X);
                    int tY = e.Y - (int)(_objeto3D.ListaVerticesAtuais[0].Y);
                    _objeto3D.Translacao(tX, tY, 0);
                    // Desenha Objeto3D
                    CarregarTela();
                }                

                if (e.Button == MouseButtons.Right)
                {
                    int grau = e.Delta > 0 ? 2 : -2;
                    if (pX)
                    {
                        _objeto3D.RotacaoX(grau);
                        CarregarTela();
                    }
                    else
                    {
                        if (pY)
                        {
                            _objeto3D.RotacaoY(grau);
                            CarregarTela();
                        }
                        else
                        {
                            if (pZ)
                            {
                                _objeto3D.RotacaoZ(grau);
                                CarregarTela();
                            }
                        }
                    }
                    
                }                

            }

            


        }

        
    }
}
