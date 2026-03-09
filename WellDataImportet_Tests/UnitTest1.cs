
using WellDataImporter.Services;
using System.IO;

namespace WellDataImportet_Tests
{
    public class WellServiceTests
    {
        private DataImporter importer;
        private DataValidator validator;

        public WellServiceTests()
        {
            importer = new DataImporter();
            validator = new DataValidator();
        }

        [Fact]
        public async Task ImportFromCsvAsync_ValidData_ShouldImportCorrectly()
        {
            // Arrange
            string csvData = "WellId;X;Y;DepthFrom;DepthTo;Rock;Porosity\n" +
                             "A-001;82,10;55,20;0;10;Sandstone;0,18\n" +
                             "A-001;82,10;55,20;10;25;Limestone;0,07\n" +
                             "A-002;90,00;60,00;0;15;Shale;0,04\n";

            var filePath = "test.csv";
            await File.WriteAllTextAsync(filePath, csvData);

            // Act
            var(wells, errors) = await importer.ImportDataAsync(filePath);
            var validationErrors = validator.ValidateWells(wells);

            // Assert
            Assert.Empty(errors);
            Assert.Equal(2, wells.Count);
            Assert.Equal("A-001", wells[0].WellId);
            Assert.Equal(2, wells[0].Intervals.Count);
            Assert.Equal("A-002", wells[1].WellId);

            // Clean up
            File.Delete(filePath);
        }

        [Fact]
        public async Task ImportFromCsvAsync_InvalidData_ShouldReturnErrors()
        {
            // Arrange
            string csvData = "WellId;X;Y;DepthFrom;DepthTo;Rock;Porosity\n" +
                             "A-001;82,10;55,20;0;10;Sandstone;0,18\n" + // Invalid interval
                             "A-002;90,00;60,00;-5;15;Shale;0,04\n"; // DepthFrom < 0

            var filePath = "test_invalid.csv";
            await File.WriteAllTextAsync(filePath, csvData);

            // Act
            var (wells, errors) = await importer.ImportDataAsync(filePath);
            var validationErrors = validator.ValidateWells(wells);

            // Assert
            Assert.NotEmpty(errors);
            Assert.Contains(errors, e => e.Contains("DepthFrom должно быть < DepthTo"));
            Assert.Contains(errors, e => e.Contains("DepthFrom должен быть >= 0"));

            // Clean up
            File.Delete(filePath);
        }
    }
}
