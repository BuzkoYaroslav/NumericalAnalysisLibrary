using System;
using NumericalAnalysisLibrary.MathStructures;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.EquationSolution
{
    public class ChordMethod : GenericIteration, EquationMethod
    {
        public double Solve(Equation eq, double a, double b)
        {
            MathFunction func = eq.Left - eq.Right;

            MathFunction der1 = func.Derivative(1),
                         der2 = func.Derivative(2);

            if (!IsSuitable(der1, a, b) ||
                !IsSuitable(der2, a, b))
                throw new Exception("Method can not be applied!");

            double c = SuitableRandomArgumnent((double x) => { return func.Calculate(x) * der2.Calculate(x) > 0; }, a, b);

            MathFunction ksi = IterationFunc(func, a, b, c);

            double x0 = InitialX(func, a, b, c);

            return GenericIterationMethod(func, ksi, a, b, x0);
        }

        protected double InitialX(MathFunction func, double a, double b, double c)
        {
            double val = func.Calculate(c);
            return SuitableRandomArgumnent((double x) => { return func.Calculate(x) * val < 0; }, a, b);
        }
        private MathFunction IterationFunc(MathFunction func, double a, double b, double c)
        {
            return -(new XFunction(1.0d) - c) / (func - func.Calculate(c));
        }

    }
}
