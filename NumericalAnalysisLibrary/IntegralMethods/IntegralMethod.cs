using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
     public interface IntegralMethod
    {
        double Solve(MathFunction func, double a, double b, int n);
    }
}
