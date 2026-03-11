using RiskSignals.Core.Models;
using RiskSignals.Core.Plugins;
using System.Data;

namespace RiskSignals.Core.Engine;

public class RiskEngine
{
    private readonly IEnumerable<IRiskSignalPlugin> _plugins;

    public RiskEngine(IEnumerable<IRiskSignalPlugin> plugins)
    {
        _plugins = plugins;
    }

    public IEnumerable<SignalResult> Run(DataTable table, IEnumerable<string> numericColumns)
    {
        return _plugins.Select(p => p.Execute(table, numericColumns));
    }

    public double CalculateFinalScore(IEnumerable<SignalResult> results)
    {
        return results.Any()
            ? results.Average(r => r.SignalScore * r.Confidence)
            : 0.0;
    }
}
