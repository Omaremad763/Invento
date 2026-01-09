using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ReadOnlyObjects
{
    public class DashboardWidgetMetric
    {
        public string Name { get; init; } = default!;
        public decimal Value { get; init; }
        public string Unit { get; init; } = default!;
    }
}
