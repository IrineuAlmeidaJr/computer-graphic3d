using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGraphic.Models.Rasterizacao
{
    public class ItemEdgeTable
    {
        public double Ymax { get; set; }
        public double Xmin { get; set; }
        public double IncX { get; set; }

        public ItemEdgeTable(double Ymax, double Xmin, double IncX)
        {
            this.Xmin = Xmin;
            this.Ymax = Ymax;
            this.IncX = IncX;
        }

        public void Incrementa()
        {
            Xmin += IncX;
        }
    }
}
