using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellDataImporter.Models;

namespace WellDataImporter.Services
{
    public class DataValidator
    {
        public List<string> ValidateWells(List<Well> wells)
        {
            List<string> errors = new List<string>();

            foreach (Well well in wells)
            {
                List<Interval> intervals = well.Intervals;

                for (int i = 0; i < intervals.Count; i++)
                {
                    Interval interval = intervals[i];

                    if (interval.DepthFrom >= interval.DepthTo)
                        errors.Add($"Ошибка в скважине {well.WellId}: DepthFrom должно быть меньше DepthTo (строка {i + 1}).");
                    if (interval.DepthFrom < 0)
                        errors.Add($"Ошибка в скважине {well.WellId}: DepthFrom не может быть отрицательным (строка {i + 1}).");
                    if (interval.Porosity < 0 || interval.Porosity > 1)
                        errors.Add($"Ошибка в скважине {well.WellId}: Porosity должно быть в диапазоне [0..1] (строка {i + 1}).");
                    if (string.IsNullOrWhiteSpace(interval.Rock))
                        errors.Add($"Ошибка в скважине {well.WellId}: Rock не должно быть пустым (строка {i + 1}).");
                }

                List<Interval> overlappingIntervals = intervals
                    .SelectMany((x, i) => intervals.Skip(i + 1).Where(y => x.DepthTo > y.DepthFrom && x.DepthFrom < y.DepthTo))
                    .ToList();

                foreach (Interval overlapping in overlappingIntervals)
                {
                    errors.Add($"Ошибка в скважине {well.WellId}: Интервалы пересекаются (строка {well.Intervals.IndexOf(overlapping) + 1}).");
                }
            }

            return errors;
        }
    }
}
