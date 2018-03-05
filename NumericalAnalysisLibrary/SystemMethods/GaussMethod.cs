using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public class GaussMethod : Method, SystemMethod
    {
        public Vector Solve(SLAE system)
        {
            for (int i = 0; i < system.EquasionsCount; i++)
            {
                if (system[i, i] == 0)
                {
                    int index = system.FindNoZeroColumn(i, i);

                    if (index == -1)
                        throw new Exception(SLAE.ResponseAboutExceptionalSolution(system[i] == 0));

                    system.SwapColumns(i, index);
                }

                system.MakeCoeficientEqualToOne(i, i);
                system.MakeDownEqualToZero(i, i);
            }

            return system.InitialOrderSolution(false);
        }
    }
}
