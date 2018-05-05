using System;
using NumericalAnalysisLibrary.MathStructures;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.EquationSolution
{
    public class HalfDivision : GenericIteration, EquationMethod
    {
        public double Solve(Equation eq, double a, double b)
        {
            MathFunction func = eq.Left - eq.Right;
            double epsilanIter = Constants.MethodConstants.epsilanIter;

            if (func.Calculate(a) * func.Calculate(b) > 0)
                throw new Exception("Method cannot be applied!");

            if (Math.Abs(func.Calculate(a)) <= epsilanIter) return a;
            if (Math.Abs(func.Calculate(b)) <= epsilanIter) return b;

            double xn = (a + b) / 2;

            while (Math.Abs(func.Calculate(xn)) > epsilanIter)
            {
                if (func.Calculate(xn) * func.Calculate(a) < 0) b = xn;
                else if (func.Calculate(xn) * func.Calculate(b) < 0) a = xn;
                xn = (a + b) / 2;
            }

            return xn;
        }
    }
}
