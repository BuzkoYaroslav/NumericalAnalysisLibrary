using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    using Condition = Tuple<double, double, double, double>;

    public class BoundaryValueTask
    {
        public KeyValuePair<double, double> Boundaries { get; private set; }
        public MathFunction PX { get; private set; }
        public MathFunction QX { get; private set; }
        public MathFunction FX { get; private set; }

        // 1 - alpha0, 2 - alpha1, 3 - a, 4 - A
        public Condition Condition1 { get; private set; }
        // 1 - betta0, 2 - betta1, 3 - b, 4 - B
        public Condition Condition2 { get; private set; }

        public double alpha0 { get { return Condition1.Item1; } }
        public double alpha1 { get { return Condition1.Item2; } }
        public double a { get { return Condition1.Item3; } }
        public double A { get { return Condition1.Item4; } }
        public double betta0 { get { return Condition2.Item1; } }
        public double betta1 { get { return Condition2.Item2; } }
        public double b { get { return Condition2.Item3; } }
        public double B { get { return Condition2.Item4; } }

        public BoundaryValueTask(MathFunction px, MathFunction qx, MathFunction fx, double a, double b, Condition condition1, Condition condition2)
        {
            this.PX = px;
            this.QX = qx;
            this.FX = fx;

            Boundaries = new KeyValuePair<double, double>(a, b);
            Condition1 = new Condition(condition1.Item1, condition1.Item2,
                condition1.Item3, condition1.Item4);
            Condition2 = new Condition(condition2.Item1, condition2.Item2,
                condition2.Item3, condition2.Item4);
        }

        public double LOperator(MathFunction yx, double x)
        {
            MathFunction der2 = yx.Derivative(2),
                         der1 = yx.Derivative(1);

            return der2.Calculate(x) + PX.Calculate(x) * der1.Calculate(x) +
                QX.Calculate(x) * yx.Calculate(x);
        }

        public override string ToString()
        {
            string result = string.Format("Equation:\ny''[x] + {0} * y'[x] + {1} * y[x] = {2}\n",
                PX.ToString(), QX.ToString(), FX.ToString());

            result += string.Format("Range: [{0}, {1}]\n", Math.Round(Boundaries.Key, 2), Math.Round(Boundaries.Value, 2));

            result += string.Format("Conditions:\n{0} * y[{1}] + {2} * y'[{1}] = {3}\n",
                alpha0, a, alpha1, A);
            result += string.Format("{0} * y[{1}] + {2} * y'[{1}] = {3}\n",
                betta0, b, betta1, B);

            return result;
        }
    }
}
