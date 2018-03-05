using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public class SimpleIterationSystem : Method, SystemMethod
    {
        public Vector Solve(SLAE system)
        {
            if (!system.SystemMatrix.IsNonDegenerate())
                throw new Exception(Constants.MethodConstants.methodIsNotApplyableString);

            double alpha = RandomNumber(0, 2.0 / (system.SystemMatrix.Transposed() * system.SystemMatrix).FirstNorm);

            Matrix bMatrix = Matrix.UnaryMatrix(system.SystemMatrix.ColumnsCount) - alpha * system.SystemMatrix.Transposed() * system.SystemMatrix;

            Vector gVector = alpha * (system.SystemMatrix.Transposed() * system.RightPart);

            Vector xCurrent = new double[system.SystemMatrix.ColumnsCount],
                   xPrev;

            int count = 0;
            int maxIterationCount = Constants.MethodConstants.maxIterationCount;
            double epsilan = Constants.epsilan;

            do
            {
                xPrev = xCurrent;
                xCurrent = bMatrix * xPrev + gVector;

                count++;
            } while (!(bMatrix.FirstNorm > 0.5 && bMatrix.FirstNorm < 1 &&
                       bMatrix.FirstNorm / (1 - bMatrix.FirstNorm) * (xCurrent - xPrev).FirstNorm <= epsilan ||
                    (xCurrent - xPrev).FirstNorm <= epsilan ||
                    count == maxIterationCount));

            if (count == maxIterationCount)
                throw new Exception(string.Format(Constants.MethodConstants.iterationOverflowString, count));

            return xCurrent;
        }
    }
}
