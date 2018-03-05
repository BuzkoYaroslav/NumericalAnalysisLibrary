using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public class CollocationMethod : BoundaryValueTaskSolver
    {
        private GaussMethod method;

        public CollocationMethod()
        {
            method = new GaussMethod();
        }

        public KeyValuePair<double, double>[] Solve(BoundaryValueTask task, int n)
        {
            int collocationN = Constants.CollocationMethodConstants.collocationN;

            MathFunction[] u = new MathFunction[collocationN + 1];
            for (int i = 0; i <= collocationN; i++)
                u[i] = UK(task, i);

            double[,] matrix = new double[collocationN, collocationN];
            double[] vect = new double[collocationN];
            double h = (task.b - task.a) / (collocationN + 1);

            for (int i = 0; i < collocationN; i++)
            {
                double xi = task.a + (i + 1) * h;

                vect[i] = task.FX.Calculate(xi) - task.LOperator(u[0], xi);
                for (int j = 0; j < collocationN; j++)
                    matrix[i, j] = task.LOperator(u[j + 1], xi);
            }

            Vector C = method.Solve(new SLAE(matrix, vect));
            MathFunction yn = u[0];
            yn.Calculate(0);
            for (int i = 1; i <= collocationN; i++)
                yn += C[i - 1] * u[i];

            h = (task.b - task.a) / n;
            KeyValuePair<double, double>[] result = new KeyValuePair<double, double>[n + 1];
            for (int i = 0; i < result.Length; i++)
                result[i] = new KeyValuePair<double, double>(task.a + i * h, yn.Calculate(task.a + i * h));

            return result;               
        }

        private MathFunction UK(BoundaryValueTask task, int k)
        {
            if (k == 0)
            {
                double[,] m = new double[,]
                {
                    {task.alpha0 * task.a + task.alpha1, task.alpha0 },
                    {task.betta0 * task.b + task.betta1, task.betta0 }
                };
                double[] v = new double[] { task.A, task.B };

                Vector res = method.Solve(new SLAE(m, v));

                return new XFunction(res[0]) + res[1];
            }

            return (new XFunction(1.0) ^ (k - 1)) * 
                (new XFunction(1.0) - task.a) * (new XFunction(1.0) - task.b);
        }
    }
}
