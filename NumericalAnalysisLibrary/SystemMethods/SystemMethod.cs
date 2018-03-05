using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public interface SystemMethod
    {
        Vector Solve(SLAE system);
    }
}
