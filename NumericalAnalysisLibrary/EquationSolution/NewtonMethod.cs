using System;
using NumericalAnalysisLibrary.Functions;
using NumericalAnalysisLibrary.MathStructures;

namespace NumericalAnalysisLibrary.EquationSolution
{
    public class NewtonMethod : GenericIteration, EquationMethod
    {
        public double Solve(Equation eq, double a, double b)
        {
            MathFunction func = eq.Left - eq.Right;

            MathFunction der1 = func.Derivative(1),
                         der2 = func.Derivative(2);

            if (!IsSuitable(der1, a, b) ||
                !IsSuitable(der2, a, b))
                throw new Exception("Method can not be applied!");

            MathFunction ksi = IterationFunc(der1, a, b);

            double x0 = InitialX(func, a, b);

            return GenericIterationMethod(func, ksi, a, b, x0);
        }
        public double UnsafeSolve(Equation eq, double startX, double eps)
        {
            MathFunction func = eq.Left - eq.Right;

            MathFunction der1 = func.Derivative(1),
                         der2 = func.Derivative(2);

            MathFunction ksi = IterationFunc(der1, 0, 0);

            double xprev = startX;
            double xn = xprev, counter = 0;

            MathFunction iterFunc = new XFunction(1.0d) + ksi * func;
            int maxIterationCount = Constants.MethodConstants.maxIterationCount;

            do
            {
                xprev = xn;
                xn = iterFunc.Calculate(xn);
                counter++;
            } while (counter < maxIterationCount && !(Math.Abs(xprev - xn) < eps));

            if (counter == maxIterationCount) return xn;//  double.PositiveInfinity;

            return xn;
        }

        protected double InitialX(MathFunction func, double a, double b)
        {
            return a;
            //return SuitableRandomArgumnent((double arg) => { return func.Calculate(arg) * func.Derivative(2).Calculate(arg) < 0; }, a, b);
        }
        protected MathFunction IterationFunc(MathFunction func, double a, double b)
        {
            return -1.0 / func;
        }
    }
}
