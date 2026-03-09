using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WellDataImporter.Services;

namespace WellDataImportet_WPF
{
    public partial class MainWindow : Window
    {
        private DataImporter importer;
        private DataValidator validator;

        public MainWindow()
        {
            InitializeComponent();
            importer = new DataImporter();
            validator = new DataValidator();
        }

        private async void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Выберите CSV файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                await ImportDataAsync(filePath);
            }
        }

        private async Task ImportDataAsync(string filePath)
        {
            txtOutput.Clear();
            var (wells, errors) = await importer.ImportDataAsync(filePath);
            var validationErrors = validator.ValidateWells(wells);

            if (errors.Any())
            {
                txtOutput.AppendText("Ошибки: \n");
                foreach (var error in errors)
                {
                    txtOutput.AppendText(error + "\n");
                }
            }

            if (wells.Any())
            {
                txtOutput.AppendText("Сводная информация по скважинам: \n");
                foreach (var well in wells)
                {
                    var totalDepth = well.Intervals.Max(i => i.DepthTo);
                    var intervalCount = well.Intervals.Count;
                    var averagePorosity = well.Intervals.Sum(i => i.Porosity * (i.DepthTo - i.DepthFrom)) /
                                          well.Intervals.Sum(i => i.DepthTo - i.DepthFrom);
                    var mostCommonRock = well.Intervals.GroupBy(i => i.Rock)
                        .OrderByDescending(g => g.Sum(i => i.DepthTo - i.DepthFrom))
                        .First().Key;

                    txtOutput.AppendText($"WellId: {well.WellId}, Total Depth: {totalDepth}, Interval Count: {intervalCount}, " +
                                      $"Average Porosity: {averagePorosity:F2}, Most Common Rock: {mostCommonRock} \n");
                }
            }
        }
    }
}
