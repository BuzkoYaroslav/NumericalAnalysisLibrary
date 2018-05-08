using System;
using System.Collections.Generic;
using System.IO;
using NumericalAnalysisLibrary.Functions;

namespace NumericalAnalysisLibrary.ExtremumSearch
{
    public class FibonacciExtremum : ExtremumMethod
    {
        private const double epsilon = 0.00001;

        public override string ToString()
        {
            return string.Format("FibonacciExtremum (eps = {0})", epsilon);
        }

        protected override double? FindExtremum(MathFunction func, ExtremumType type, double a, double b, string fileName)
        {
            double lambda_k, mu_k;
            double f_lambda, f_mu;
            double a_k = a, b_k = b;
            double multiplier = type == ExtremumType.Maximum ? -1 : 1;

            StreamWriter writer = new StreamWriter(fileName);

            writer.Write(GapForTable(func, epsilon));

            List<double> fibonacci = new List<double>();

            fibonacci.Add(1);
            fibonacci.Add(1);

            int n = 0;

            for (int j = 0; ; j++)
            {
                fibonacci.Add(fibonacci[j] + fibonacci[j + 1]);

                double val = (b - a) / epsilon;

                if (fibonacci[j + 1] <= val &&
                    fibonacci[j + 2] >= val)
                {
                    n = j;
                    break;
                }
            }

            Func<double, double, double> coef = (a_i, f_i) => { return a_i + f_i / fibonacci[n + 2] * (b - a); };

            lambda_k = coef(a, fibonacci[n]);
            mu_k = coef(a, fibonacci[n + 1]);

            f_lambda = multiplier * func.Calculate(lambda_k);
            f_mu = multiplier * func.Calculate(mu_k);

            writer.Write(RowForIteration(1, a_k, b_k, lambda_k, mu_k, multiplier * f_lambda, multiplier * f_mu));

            if (f_lambda <= f_mu)
                b_k = mu_k;
            else
                a_k = lambda_k;

            for (int k = 2; k <= n; k++)
            {
                if (f_lambda <= f_mu)
                {
                    mu_k = lambda_k;
                    f_mu = f_lambda;

                    lambda_k = coef(a_k, fibonacci[n - k]);
                    f_lambda = multiplier * func.Calculate(lambda_k);
                }
                else
                {
                    a_k = lambda_k;
                    lambda_k = mu_k;
                    f_lambda = f_mu;

                    mu_k = coef(a_k, fibonacci[n - k + 1]);
                    f_mu = multiplier * func.Calculate(f_mu);
                }

                writer.Write(RowForIteration(k, a_k, b_k, lambda_k, mu_k, multiplier * f_lambda, multiplier * f_mu));

                if (f_lambda <= f_mu)
                    b_k = mu_k;
                else
                    a_k = lambda_k;
            }

            writer.Close();

            return (a_k + b_k) / 2;
        }

        protected override double? FindMinimum(MathFunction func, double a, double b)
        {
            double lambda_k, mu_k;
            double f_lambda, f_mu;
            double a_k = a, b_k = b;

            List<double> fibonacci = new List<double>();

            fibonacci.Add(1);
            fibonacci.Add(1);

            int n = 0;

            for (int j = 0; ; j++)
            {
                fibonacci.Add(fibonacci[j] + fibonacci[j + 1]);

                double val = (b - a) / epsilon;

                if (fibonacci[j + 1] <= val &&
                    fibonacci[j + 2] >= val)
                {
                    n = j;
                    break;
                }
            }

            Func<double, double, double> coef = (a_i, f_i) => { return a_i + f_i / fibonacci[n + 2] * (b - a); };

            lambda_k = coef(a, fibonacci[n]);
            mu_k = coef(a, fibonacci[n + 1]);

            f_lambda = func.Calculate(lambda_k);
            f_mu = func.Calculate(mu_k);

            if (f_lambda <= f_mu)
                b_k = mu_k;
            else
                a_k = lambda_k;

            for (int k = 2; k <= n; k++)
            {
                if (f_lambda <= f_mu)
                {
                    mu_k = lambda_k;
                    f_mu = f_lambda;

                    lambda_k = coef(a_k, fibonacci[n - k]);
                    f_lambda = func.Calculate(lambda_k);                    
                } else
                {
                    a_k = lambda_k;
                    lambda_k = mu_k;
                    f_lambda = f_mu;

                    mu_k = coef(a_k, fibonacci[n - k + 1]);
                    f_mu = func.Calculate(f_mu);
                }

                if (f_lambda <= f_mu)
                    b_k = mu_k;
                else
                    a_k = lambda_k;
            }

            return (a_k + b_k) / 2;
        }
    }
}
