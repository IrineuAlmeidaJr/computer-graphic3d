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

namespace ComputerGraphic
{
    public partial class ComputerGraphic : Form
    {

        private Objeto3d _objeto3D;
        private Bitmap _imagem;
        private int _width;
        private int _height;

        public ComputerGraphic()
        {
            InitializeComponent();
            _objeto3D = new Objeto3d();
            _width = pictureBox.Height;
            _height = pictureBox.Width;

            CarregarTela();
        }

        private void CarregarTela()
        {
            _imagem =
                new Bitmap(_height, _width, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(_imagem);
            pictureBox.Image = _imagem;                        
            
        }

        private void abrirObjeto3D_Click(object sender, EventArgs e)
        {
            String line;
            try
            {

                using (var ofd = new OpenFileDialog())
                {
                    //ofd.Filter = "txt fil"
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamReader arquivo = new StreamReader(ofd.FileName))
                        {
                            _objeto3D = new Objeto3d();

                            NumberFormatInfo provider = new NumberFormatInfo();
                            provider.NumberDecimalSeparator = ".";
                            //provider.NumberGroupSeparator = ",";

                            double x, y, z;
                            string linha;
                            string[] dados;
                            string[] dadosFace;
                            while ((linha = arquivo.ReadLine()) != null)
                            {
                                dados = linha.Split(' ');

                                switch (dados[0])
                                {
                                    case "v":
                                        // Pode dar Erro na converção
                                        x = Convert.ToDouble(dados[1], provider);
                                        y = Convert.ToDouble(dados[2], provider);
                                        z = Convert.ToDouble(dados[3], provider);

                                        //x = Convert.ToDouble(dados[1], provider) * 200;
                                        //y = Convert.ToDouble(dados[2], provider) * 200;
                                        //z = Convert.ToDouble(dados[3], provider);

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
                            // Desenha Poligono
                            CarregarTela();
                            _objeto3D.DesenharPoligono(_imagem, pictureBox, _height/2, _width/2);

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
    }
}
