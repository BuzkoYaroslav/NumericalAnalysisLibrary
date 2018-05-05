using System;
using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.FunctionApproximation
{
    public class NewtonFunction : Method, FunctionMethod
    {
        public Polynomial Solve(Vector x, Vector f)
        {
            if (x.Count != f.Count)
                throw new ArgumentOutOfRangeException("Incorrect arguments!");

            Polynomial result = new Polynomial(f[0]);

            for (int i = 1; i < x.Count; i++)
            {
                Polynomial multi = new Polynomial(1);
                for (int j = 0; j < i; j++)
                    multi *= new Polynomial(-x[j], 1);

                multi *= DividedDifference(x, f, 0, i);

                result += multi;
            }

            return result;
        }
        private static double DividedDifference(Vector x, Vector f, int left, int right)
        {
            if (left < 0 || right < 0 ||
                right < left)
                throw new Exception("Incorrect arguments!");

            if (right == left)
                return f[left];

            return (DividedDifference(x, f, left + 1, right) - DividedDifference(x, f, left, right - 1)) /
                (x[right] - x[left]);
        }

    }
}
