using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public class MinSquareFunction : Method, FunctionMethod
    {
        int m;

        public MinSquareFunction(int m): base()
        {
            this.m = m;
        }

        public Polynomial Solve(Vector x, Vector f)
        {
            if (x.Count != f.Count)
                throw new ArgumentOutOfRangeException("Incorrect input data!");
            if (m < 0)
                throw new Exception("m index cannot be less than zero!");

            double[,] aMatrix = new double[m + 1, m + 1];
            double[] right = new double[m + 1];

            for (int i = 0; i <= m; i++)
            {
                for (int p = 0; p < x.Count; p++)
                    right[i] += Math.Pow(x[p], i) * f[p];

                for (int j = 0; j <= m; j++)
                    for (int p = 0; p < x.Count; p++)
                        aMatrix[i, j] += Math.Pow(x[p], i + j);
            }

            Matrix A = aMatrix;
            Vector F = right;

            

            return new Polynomial((double[])(new GaussMethod()).Solve(new SLAE(A, F)));
        }
        public double StandardDeviation(double[] x, double[] f, Polynomial Q)
        {
            if (x.Length != f.Length)
                throw new ArgumentOutOfRangeException("Incorrect input data!");

            double s = 0;

            for (int i = 0; i < x.Length; i++)
                s += Math.Pow(f[i] - Q.GetValue(x[i]), 2);

            return Math.Sqrt(s / x.Length);
        }
    }
}
