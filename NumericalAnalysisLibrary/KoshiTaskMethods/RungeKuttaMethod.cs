using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public static class RungeKuttaMethod
    {
        public static KeyValuePair<double, double>[] Solve(KoshiTask task)
        {
            List<KeyValuePair<double, double>> result = new List<KeyValuePair<double, double>>();

            result.Add(task.StartCondition);

            double h = Constants.RungeKuttaMethodConstants.startStep;
            double x = task.StartCondition.Key,
                   y = task.StartCondition.Value;

            while (true)
            {
                KeyValuePair<double, double> firstH = CalculateApproximation(task.Derivative, 
                    new KeyValuePair<double, double>(x, y), h),
                    firstH2 = CalculateApproximation(task.Derivative,
                    new KeyValuePair<double, double>(x, y), h / 2),
                    secondH2 = CalculateApproximation(task.Derivative,
                   firstH2, h / 2);

                int methodPower = Constants.RungeKuttaMethodConstants.methodPower;
                double epsilan = Constants.epsilan;
                double epsH = (secondH2.Value - firstH.Value) * Math.Pow(2, methodPower) / (Math.Pow(2, methodPower) - 1),
                       epsH2 = (secondH2.Value - firstH.Value) / (Math.Pow(2, methodPower) - 1);

                if (Math.Abs(epsH2) <= epsilan)
                {
                    y = secondH2.Value + epsH2;
                    x = secondH2.Key;

                    result.Add(new KeyValuePair<double, double>(x, y));
                } else
                {
                    h /= 2;
                    continue;
                }

                if (Math.Abs(epsH) <= epsilan)
                    h *= 2;

                double delta = task.Range.Value - x;
                if (delta <= epsilan)
                    break;
                else if (h > delta)
                    h = delta;
            }

            return result.ToArray();
        }
        public static KeyValuePair<double, double>[] Solve(KoshiTask task, int countOfSteps)
        {
            List<KeyValuePair<double, double>> result = new List<KeyValuePair<double, double>>();

            result.Add(task.StartCondition);

            double h = (task.Range.Value - task.Range.Key) / countOfSteps;
            double x = task.StartCondition.Key,
                   y = task.StartCondition.Value;
            double epsilan = Constants.epsilan;

            while (task.Range.Value - x >= epsilan)
            {
                if (x + h > task.Range.Value)
                    h = task.Range.Value - x;

                KeyValuePair<double, double> next = CalculateApproximation(task.Derivative, new KeyValuePair<double, double>(x, y), h);

                x = next.Key;
                y = next.Value;

                result.Add(next);
            }

            return result.ToArray();
        } 

        private static KeyValuePair<double, double> CalculateApproximation(MultiMathFunction func, KeyValuePair<double, double> prev, double h)
        {
            double x = prev.Key, y = prev.Value;

            Dictionary<uint, double> vars = new Dictionary<uint, double>()
                {
                    {0, x},
                    {1, y}
                };
            double k1 = h * func.Calculate(vars);

            vars[0] = x + h / 2;
            vars[1] = y + k1 / 2;
            double k2 = h * func.Calculate(vars);

            vars[0] = x + h;
            vars[1] = y - k1 + 2 * k2;
            double k3 = h * func.Calculate(vars);

            double newY = y + (k1 + 4 * k2 + k3) / 6;

            return new KeyValuePair<double, double>(x + h, newY);
        }
    }
}
