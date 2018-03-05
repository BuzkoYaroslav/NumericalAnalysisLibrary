using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public interface EquationMethod
    {
        double Solve(Equation eq, double a, double b);
    }
}
