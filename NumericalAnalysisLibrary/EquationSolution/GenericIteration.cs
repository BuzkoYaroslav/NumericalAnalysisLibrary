using System;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.EquationSolution
{
    public class GenericIteration: Method
    {
        protected virtual double GenericIterationMethod(MathFunction func, MathFunction ksi, double a, double b, double x0)
        {
            double xn = x0, counter = 0;
            double fDerivativeMin = func.Derivative(1).MinValue(a, b);
            int maxIterationCount = Constants.MethodConstants.maxIterationCount;
            double epsilanIter = Constants.MethodConstants.epsilanIter;

            MathFunction iterFunc = new XFunction(1.0d) + ksi * func;

            do
            {
                xn = iterFunc.Calculate(xn);
                counter++;
            } while (counter < maxIterationCount && !(Math.Abs(func.Calculate(xn)) / fDerivativeMin <= epsilanIter));

            if (counter == maxIterationCount) return xn;//  double.PositiveInfinity;

            return xn;
        }
        protected bool IsSuitable(MathFunction func, double a, double b)
        {
            return func.IsContinuous(a, b) && func.IsWithConstSign(a, b);
        }
    }
}
