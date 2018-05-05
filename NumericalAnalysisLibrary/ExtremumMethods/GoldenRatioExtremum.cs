using System;
using System.IO;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.ExtremumSearch
{
    public class GoldenRatioExtremum : ExtremumMethod
    {
        private const double epsilon = 0.00001;

        private Func<double, double, double> lambda = (a_k, b_k) => { return a_k + (3.0 - Math.Sqrt(5)) / 2 * (b_k - a_k); },
                mu = (a_k, b_k) => { return a_k + (Math.Sqrt(5) - 1.0) / 2 * (b_k - a_k); };

        public override string ToString()
        {
            return string.Format("GoldenRatioExtremum (eps = {0})", epsilon);
        }

        protected override double? FindExtremum(MathFunction func, ExtremumType type, double a, double b, string fileName)
        {
            double lambda_k, mu_k;
            double f_lambda, f_mu;

            double multiplier = type == ExtremumType.Maximum ? -1 : 1;

            StreamWriter writer = new StreamWriter(fileName);

            writer.Write(GapForTable(func, epsilon));

            lambda_k = lambda(a, b);
            mu_k = mu(a, b);
            f_lambda = multiplier * func.Calculate(lambda_k);
            f_mu = multiplier * func.Calculate(mu_k);

            writer.Write(RowForIteration(1, a, b, lambda_k, mu_k, multiplier * f_lambda, multiplier * f_mu));

            for (int k = 2; b - a > epsilon; k++)
            {
                if (f_lambda > f_mu)
                {
                    a = lambda_k;
                    lambda_k = mu_k;
                    mu_k = mu(a, b);
                }
                else
                {
                    b = mu_k;
                    mu_k = lambda_k;
                    lambda_k = lambda(a, b);
                }
                f_lambda = multiplier * func.Calculate(lambda_k);
                f_mu = multiplier * func.Calculate(mu_k);

                writer.Write(RowForIteration(k, a, b, lambda_k, mu_k, multiplier * f_lambda, multiplier * f_mu));
            }

            writer.Close();

            return (a + b) / 2;
        }

        protected override double? FindMinimum(MathFunction func, double a, double b)
        {
            double lambda_k, mu_k;

            lambda_k = lambda(a, b);
            mu_k = mu(a, b);

            while (b - a > epsilon)
            {
                if (func.Calculate(lambda_k) > func.Calculate(mu_k))
                {
                    a = lambda_k;
                    lambda_k = mu_k;
                    mu_k = mu(a, b);
                } else
                {
                    b = mu_k;
                    mu_k = lambda_k;
                    lambda_k = lambda(a, b);
                }
            }

            return (a + b) / 2;
        }
    }
}
