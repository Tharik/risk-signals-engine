using RiskSignals.Core.Models;
using RiskSignals.Core.Plugins;
using System.Data;

namespace RiskSignals.Plugins.Benford;

public class BenfordFirstDigitPlugin : IRiskSignalPlugin
{
    public string Id => "benford.first_digit";

    public SignalResult Execute(DataTable table, IEnumerable<string> numericColumns)
    {
        // Initial stub — we'll implement the actual logic later
        return new SignalResult
        {
            SignalId = Id,
            SignalScore = 0.2,
            Confidence = 0.8,
            Severity = Severity.Info,
            Explanation = "Benford check executed (stub implementation).",
            Evidence = new Dictionary<string, object>
            {
                { "rows", table.Rows.Count }
            }
        };
    }
}
