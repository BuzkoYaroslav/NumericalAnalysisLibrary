using System.IO;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.ExtremumSearch
{
    public class HalfDivisionExtremum : ExtremumMethod
    {
        private const double epsilon = 0.00001;
        private const double delta = 0.000001;

        public override string ToString()
        {
            return string.Format("HalfDivisionExtremum (eps = {0}, delta = {1})", epsilon, delta);
        }

        protected override double? FindMinimum(MathFunction func, double a, double b)
        {
            double lambda, mu;

            while (b - a > epsilon)
            {
                lambda = (a + b - delta) / 2;
                mu = a + b - lambda;

                if (func.Calculate(lambda) <= func.Calculate(mu))
                    b = mu;
                else
                    a = lambda;
            }

            return (a + b) / 2;
        }

        protected override double? FindExtremum(MathFunction func, ExtremumType type, double a, double b, string fileName)
        {
            double multiplier = type == ExtremumType.Maximum ? -1 : 1;

            StreamWriter writer = new StreamWriter(fileName);

            writer.Write(GapForTable(func, epsilon));

            for (int k = 1; b - a > epsilon; k++)
            {
                double lambda = (a + b - delta) / 2;
                double mu = a + b - lambda;

                double f_lambda = multiplier * func.Calculate(lambda),
                    f_mu = multiplier * func.Calculate(mu);

                writer.Write(RowForIteration(k, a, b, lambda, mu, multiplier * f_lambda, multiplier * f_mu));

                if (f_lambda <= f_mu)
                    b = mu;
                else
                    a = lambda;
            }
            writer.Close();

            return (a + b) / 2;
        }
    }
}
