using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public interface BoundaryValueTaskSolver
    {
        KeyValuePair<double, double>[] Solve(BoundaryValueTask task, int n);
    }
}
