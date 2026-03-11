using CsvHelper;
using CsvHelper.Configuration;
using RiskSignals.Core.Engine;
using RiskSignals.Plugins.Benford;
using System.Data;
using System.Globalization;
using System.Text.Json;

static void Fail(string message)
{
    Console.Error.WriteLine(message);
    Environment.Exit(1);
}

if (args.Length < 2 || args[0] != "analyze")
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  rse analyze <file.csv> [--out report.json]");
    return;
}

var inputPath = args[1];
if (!File.Exists(inputPath))
    Fail($"Input file not found: {inputPath}");

var outPath = "report.json";
for (var i = 2; i < args.Length; i++)
{
    if (args[i] == "--out" && i + 1 < args.Length)
    {
        outPath = args[i + 1];
        i++;
    }
}

var table = ReadCsvAsDataTable(inputPath);
var numericColumns = DetectNumericColumns(table).ToList();

var plugin = new BenfordFirstDigitPlugin();
var engine = new RiskEngine(new[] { plugin });

var results = engine.Run(table, numericColumns).ToList();
var finalScore = engine.CalculateFinalScore(results);

var report = new
{
    tool = "rse",
    version = "0.0.1",
    generatedAtUtc = DateTime.UtcNow,
    input = new
    {
        path = inputPath,
        rows = table.Rows.Count,
        columns = table.Columns.Count,
        numericColumns
    },
    finalScore,
    signals = results
};

var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText(outPath, json);

Console.WriteLine($"Rows: {table.Rows.Count}");
Console.WriteLine($"Numeric columns: {string.Join(", ", numericColumns)}");
Console.WriteLine($"Final Score: {finalScore:0.000}");
Console.WriteLine($"Report written: {outPath}");

static DataTable ReadCsvAsDataTable(string path)
{
    var table = new DataTable();

    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
        BadDataFound = null
    };

    using var reader = new StreamReader(path);
    using var csv = new CsvReader(reader, config);

    csv.Read();
    csv.ReadHeader();
    var headers = csv.HeaderRecord ?? Array.Empty<string>();

    foreach (var h in headers)
        table.Columns.Add(h, typeof(string));

    while (csv.Read())
    {
        var row = table.NewRow();
        foreach (var h in headers)
            row[h] = csv.GetField(h) ?? "";
        table.Rows.Add(row);
    }

    return table;
}

static IEnumerable<string> DetectNumericColumns(DataTable table)
{
    foreach (DataColumn col in table.Columns)
    {
        var ok = 0;
        var total = 0;

        foreach (DataRow row in table.Rows)
        {
            var s = row[col]?.ToString();
            if (string.IsNullOrWhiteSpace(s)) continue;

            total++;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                ok++;
        }

        // heurística simples: se >= 80% dos valores não-vazios parseiam, consideramos numérica
        if (total > 0 && (double)ok / total >= 0.8)
            yield return col.ColumnName;
    }
}