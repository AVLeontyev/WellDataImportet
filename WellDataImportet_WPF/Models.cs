using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WellDataImporter.Models
{
    public class Interval
    {
        public double DepthFrom { get; set; }
        public double DepthTo { get; set; }
        public string Rock { get; set; }
        public double Porosity { get; set; }
    }
    public class Well
    {
        public string WellId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public List<Interval> Intervals { get; set; } = new List<Interval>();
    }
}

