using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public static class AdamsMethod
    {
        public static KeyValuePair<double, double>[] Solve(KoshiTask task, double step)
        {
            int n = Constants.AdamsMethodConstants.n;
            double epsilan = Constants.epsilan;

            List<KeyValuePair<double, double>> result = new List<KeyValuePair<double, double>>();

            KoshiTask supportTask = new KoshiTask(task.Derivative, task.StartCondition, 
                new KeyValuePair<double, double>(task.Range.Key, task.Range.Key + step * n));
            result.AddRange(RungeKuttaMethod.Solve(supportTask, n));

            double[] A = new double[n + 1];
            double aInterpol;
            for (int i = 0; i < n + 1; i++)
                A[i] = AkInter(i);
            aInterpol = AkInter(-1);

            int index = n;

            while (result[index].Key + step <= task.Range.Value)
            {
                Dictionary<uint, double> vars = new Dictionary<uint, double>();
                vars.Add(0, 1);
                vars.Add(1, 1);

                double sum = result[index].Value;
                for (int i = 0; i < n + 1; i++)
                {
                    vars[0] = result[index - i].Key;
                    vars[1] = result[index - i].Value;
                    sum += step * task.Derivative.Calculate(vars) * A[i];
                }

                double yprev, 
                    ynext = sum;

                vars.Remove(1);
                vars[0] = result[index].Key + step;

                MathFunction func = sum + step * aInterpol * task.Derivative.TransformToSimpleFunction(vars);

                do
                {
                    yprev = ynext;
                    ynext = func.Calculate(sum);
                } while (Math.Abs(yprev - ynext) > epsilan);

                result.Add(new KeyValuePair<double, double>(vars[0], ynext));
                index++;
            }

            return result.ToArray();
        }

        
        private static KeyValuePair<double, double> NewtonBoundaries(double startLeft, double difference, MathFunction func)
        {
            MathFunction func1 = func.Derivative(1),
                    func2 = func.Derivative(2);

            Func<double, MathFunction, bool, bool> condition = (x, function, isPositive) =>
            {
                double value = function.Calculate(x);

                return !double.IsInfinity(value) && !double.IsNaN(value) &&
                       (isPositive && value >= 0 || !isPositive && value <= 0);
            };

            bool isPositive1 = func1.Calculate(startLeft) > 0,
                 isPositive2 = func2.Calculate(startLeft) > 0;

            double right = difference, left = -difference;
            double funcConditionStep = Constants.AdamsMethodConstants.funcConditionStep;

            for (double i = startLeft; i < difference; i += funcConditionStep)
                if (!(condition(i, func1, isPositive1) && condition(i, func2, isPositive2)))
                {
                    right = i - funcConditionStep;
                    break;
                }

            for (double i = startLeft; i > -difference; i -= funcConditionStep)
                if (!(condition(i, func1, isPositive1) && condition(i, func2, isPositive2)))
                {
                    left = i + funcConditionStep;
                    break;
                }

            return new KeyValuePair<double, double>(left, right);
        }
        private static double AkInter(int k)
        {
            MathFunction res = 1;
            int n = Constants.AdamsMethodConstants.n;

            for (int i = -1; i <= n; i++)
                if (i != k)
                    res *= new XFunction(1.0) + i;

            double result = new SimpsonMethod().Solve(res, 0, 1, 10);

            Func<long, long> Factorial = null;
            Factorial = x => x == 0 ? 1 : x * Factorial(x - 1);

            result *= Math.Pow(-1, k + 1) / (Factorial(k + 1) * Factorial(n - k));

            return result;
        }
        private static double AkExtra(int k)
        {
            MathFunction res = 1;
            int n = Constants.AdamsMethodConstants.n;

            for (int i = 0; i <= n; i++)
                if (i != k)
                    res *= new XFunction(1.0) + i;

            double result = new SimpsonMethod().Solve(res, 0, 1, 10);

            Func<long, long> Factorial = null;
            Factorial = x => x == 0 ? 1 : x * Factorial(x - 1);

            result *= Math.Pow(-1, k) / (Factorial(k) * Factorial(n - k));

            return result;
        }

    }
}
