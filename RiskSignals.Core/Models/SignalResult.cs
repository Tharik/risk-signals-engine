namespace RiskSignals.Core.Models;

public class SignalResult
{
    public string SignalId { get; set; } = default!;
    public double SignalScore { get; set; }  // 0..1
    public double Confidence { get; set; }   // 0..1
    public Severity Severity { get; set; }

    public string Explanation { get; set; } = default!;
    public Dictionary<string, object>? Evidence { get; set; }
}
