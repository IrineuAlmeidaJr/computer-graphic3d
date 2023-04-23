using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        Objeto3d objeto3D;

        public ComputerGraphic()
        {
            InitializeComponent();
            objeto3D = new Objeto3d();
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
                                        x = Convert.ToDouble(dados[1]);
                                        y = Convert.ToDouble(dados[1]);
                                        z = 0.0;
                                        objeto3D.ListaVerticesOriginais
                                            .Add(new Vertice(x, y, z));
                                        objeto3D.ListaVerticesAtuais
                                            .Add(new Vertice(x, y, z));
                                        break;

                                    case "f":
                                        List<int> posVertices = new List<int>();
                                        for (int i = 1; i < dados.Length; i++)
                                        {
                                            dadosFace = dados[i].Split('/');
                                            // Coloca "-1", porque, as posições no arquivo iniciam de 1,
                                            // mas, na lista inicia de 0
                                            posVertices.Add(Convert.ToInt32(dadosFace[0]) - 1);
                                        }

                                        objeto3D.ListaFaces.Add(posVertices);
                                        break;
                                    default:
                                        Console.WriteLine("Nenhuma das Opções");
                                        break;
                                }



                            }




                        }
                    }




                }


                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("C:\\Sample.txt");
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the line to console window
                    Console.WriteLine(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
                Console.ReadLine();
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
