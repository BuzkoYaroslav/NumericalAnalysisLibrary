using System;
using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.FunctionApproximation
{
    public class LagranzFunction : Method, FunctionMethod
    {
        public Polynomial Solve(Vector x, Vector f)
        {
            if (x.Count != f.Count)
                throw new Exception("Incorrect input data!");

            Polynomial result = new Polynomial();

            for (int i = 0; i < x.Count; i++)
            {
                Polynomial multi = new Polynomial(1);

                for (int j = 0; j < x.Count; j++)
                    if (i != j)
                        multi *= 1.0d / (x[i] - x[j]) * new Polynomial(-x[j], 1);

                multi *= f[i];

                result += multi;
            }

            return result;
        }
    }
}
