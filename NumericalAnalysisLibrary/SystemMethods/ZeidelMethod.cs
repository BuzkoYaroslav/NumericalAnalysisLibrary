using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public class ZeidelMethod : Method, SystemMethod
    {
        public Vector Solve(SLAE system)
        {
            if (!system.SystemMatrix.IsNonDegenerate())
                throw new Exception(Constants.MethodConstants.methodIsNotApplyableString);

            double alpha = RandomNumber(0, 2.0 / (system.SystemMatrix.Transposed() * system.SystemMatrix).FirstNorm);

            Matrix bMatrix = Matrix.UnaryMatrix(system.SystemMatrix.ColumnsCount) - alpha * system.SystemMatrix.Transposed() * system.SystemMatrix;
            Matrix HMatrix, FMatrix;

            DivideMatrix(bMatrix, out HMatrix, out FMatrix);

            Vector gVector = alpha * system.SystemMatrix.Transposed() * system.RightPart;

            Vector xCurrent = new double[system.SystemMatrix.ColumnsCount],
                   xPrev;

            int count = 0;
            double epsilan = Constants.epsilan;
            int maxIterationCount = Constants.MethodConstants.maxIterationCount;

            do
            {
                xPrev = xCurrent;
                xCurrent = new double[system.SystemMatrix.ColumnsCount];

                Vector prevPart = FMatrix * xPrev + gVector;

                for (int i = 0; i < xCurrent.Count; i++)
                {
                    xCurrent[i] = prevPart[i];

                    for (int j = 0; j < i; j++)
                        xCurrent[i] += xCurrent[j] * HMatrix[i, j];
                }

                count++;
            } while (!(bMatrix.FirstNorm > 0.5 && bMatrix.FirstNorm < 1 &&
                       bMatrix.FirstNorm / (1 - bMatrix.FirstNorm) * (xCurrent - xPrev).FirstNorm <= epsilan ||
                    (xCurrent - xPrev).FirstNorm <= epsilan ||
                    count == maxIterationCount));

            if (count == maxIterationCount)
                throw new Exception(string.Format(Constants.MethodConstants.iterationOverflowString, count));

            return xCurrent;
        }
        private static void DivideMatrix(Matrix A, out Matrix H, out Matrix F)
        {
            H = new double[A.RowsCount, A.ColumnsCount];
            F = new double[A.RowsCount, A.ColumnsCount];

            for (int i = 0; i < A.RowsCount; i++)
                for (int j = 0; j < A.ColumnsCount; j++)
                    if (i > j)
                        H[i, j] = A[i, j];
                    else
                        F[i, j] = A[i, j];
        }

    }

}
