using System.Collections.Generic;

namespace NumericalAnalysisLibrary.BoundaryValueProblem
{
    public interface BoundaryValueTaskSolver
    {
        KeyValuePair<double, double>[] Solve(BoundaryValueTask task, int n);
    }
}
