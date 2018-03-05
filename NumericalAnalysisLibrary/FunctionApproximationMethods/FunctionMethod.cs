using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public interface FunctionMethod
    {
        Polynomial Solve(Vector x, Vector f);
       
    }
}
