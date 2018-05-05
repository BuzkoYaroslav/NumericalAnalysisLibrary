using System;
using System.Collections.Generic;
using NumericalAnalysisLibrary.MathStructures;
using NumericalAnalysisLibrary.SLAESolution;

namespace NumericalAnalysisLibrary.BoundaryValueProblem
{
    public class GridMethod : BoundaryValueTaskSolver
    {
        public KeyValuePair<double, double>[] Solve(BoundaryValueTask task, int n)
        {
            double[,] matrix = new double[n + 1, n + 1];
            double[] vect = new double[n + 1];

            double h = (task.b - task.a) / n;

            vect[0] = task.A * h;
            vect[n] = task.B * h;
            matrix[0, 0] = task.alpha0 * h - task.alpha1;
            matrix[0, 1] = task.alpha1;
            matrix[n, n] = task.betta0 * h + task.betta1;
            matrix[n, n - 1] = -task.betta1;
            
            for (int i = 1; i <= n - 1; i++)
            {
                double xi = task.a + i * h;

                vect[i] = 2 * Math.Pow(h, 2) * task.FX.Calculate(xi);

                matrix[i, i + 1] = 2 + task.PX.Calculate(xi) * h;
                matrix[i, i] = task.QX.Calculate(xi) * 2 * Math.Pow(h, 2) - 4;
                matrix[i, i - 1] = 2 - task.PX.Calculate(xi) * h;
            }

            Vector yx = new GaussMethod().Solve(new SLAE(matrix, vect));
            KeyValuePair<double, double>[] result = new KeyValuePair<double, double>[yx.Count];

            for (int i = 0; i < result.Length; i++)
                result[i] = new KeyValuePair<double, double>(task.a + i * h, yx[i]);

            return result;
        }
    }
}
