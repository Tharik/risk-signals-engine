using RiskSignals.Core.Models;
using System.Data;

namespace RiskSignals.Core.Plugins;

public interface IRiskSignalPlugin
{
    string Id { get; }

    SignalResult Execute(DataTable table, IEnumerable<string> numericColumns);
}
