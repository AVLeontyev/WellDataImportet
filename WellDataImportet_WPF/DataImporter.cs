using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellDataImporter.Models;


namespace WellDataImporter.Services
{
    public class DataImporter
    {
        public async Task<(List<Well> wells, List<string> errors)> ImportDataAsync(string filePath)
        {
            List<Well> wells = new List<Well>();
            List<string> errors = new List<string>();
            string[] lines = File.ReadAllLines(filePath); // await File.ReadAllLinesAsync, из-за проблем с версиями не запускается, поэтому в комменте

            for (int i = 1; i < lines.Length; i++) 
            {
                string[] values = lines[i].Split(';');
                if (values.Length != 7)
                {
                    errors.Add($"Ошибка в строке {i + 1}: Неверное количество значений (" + values.Length + ").");
                    continue;
                }

                string wellId = values[0];
                double x = double.Parse(values[1]);
                double y = double.Parse(values[2]);
                double depthFrom = double.Parse(values[3]);
                double depthTo = double.Parse(values[4]);
                string rock = values[5];
                double porosity = double.Parse(values[6]);

                Interval interval = new Interval
                {
                    DepthFrom = depthFrom,
                    DepthTo = depthTo,
                    Rock = rock,
                    Porosity = porosity
                };

                Well well = wells.FirstOrDefault(w => w.WellId == wellId);
                if (well == null)
                {
                    well = new Well { WellId = wellId, X = x, Y = y };
                    wells.Add(well);
                }

                well.Intervals.Add(interval);
            }

            return (wells, errors);
        }
    }
}

