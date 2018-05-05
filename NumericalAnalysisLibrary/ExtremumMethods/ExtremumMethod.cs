using System;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.ExtremumSearch
{
    public abstract class ExtremumMethod : IExtremumSolver
    {
        private const string invalidRangeErrorMessage = "Invalid range!";
        private const string tableFormat = "{0,10}{1,15:F7}{2,15:F7}{3,15:F7}{4,15:F7}{5,15:F7}{6,15:F7}\n";

        public double? FindExtremum(MathFunction func, double a, double b, ExtremumType type, string fileName = null)
        {
            if (b <= a)
                throw new Exception(invalidRangeErrorMessage);

            MathFunction f = type == ExtremumType.Minimum ? func : -func;

            if (fileName != null)
                return FindExtremum(func, type, a, b, fileName);
            else
                return FindMinimum(f, a, b); 
        }

        protected abstract double? FindMinimum(MathFunction func, double a, double b);
        protected abstract double? FindExtremum(MathFunction func, ExtremumType type, double a, double b, string fileName);

        protected string GapForTable(MathFunction func, double epsilon)
        {
            string result = "f(x) = " + func 
                + " Epsilon = " + epsilon 
                + " method: " + this + "\n";

            result += string.Format(tableFormat,
                "k", "a_k", "b_k", "lambda_k", "mu_k", "f(lambda_k)", "f(mu_k)");

            return result;
        }
        protected string RowForIteration(params double[] args)
        {
            if (args.Length != 7)
                throw new InvalidOperationException();

            return string.Format(tableFormat,
                (int)args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
        }
    }
}
