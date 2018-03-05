using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public class HoleckiiMethod : Method, SystemMethod
    {
        public Vector Solve(SLAE system)
        {
            if (!system.SystemMatrix.IsNonDegenerate())
                throw new Exception(Constants.MethodConstants.methodIsNotApplyableString);

            Matrix low, up;

            DetermineLowAndUpMatrix(system.SystemMatrix, out low, out up);

            return new SLAE(up, new SLAE(low, system.RightPart).InitialOrderSolution(true)).InitialOrderSolution(false);
        }
        private static void DetermineLowAndUpMatrix(Matrix matrix, out Matrix low, out Matrix up)
        {
            low = new double[matrix.RowsCount, matrix.ColumnsCount];
            up = new double[matrix.RowsCount, matrix.ColumnsCount];

            int index = 0;

            while (index < matrix.ColumnsCount)
            {
                for (int i = index; i < matrix.ColumnsCount; i++)
                {
                    low[i, index] = matrix[i, index];
                    for (int k = 0; k < index; k++)
                        low[i, index] -= low[i, k] * up[k, index];
                }

                up[index, index] = 1;
                for (int j = index + 1; j < matrix.ColumnsCount; j++)
                {
                    up[index, j] = matrix[index, j];
                    for (int k = 0; k < index; k++)
                        up[index, j] -= low[index, k] * up[k, j];

                    up[index, j] /= low[index, index];
                }

                index++;
            }
        }
    }
}
