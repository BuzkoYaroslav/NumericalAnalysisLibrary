using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalAnalysisLibrary
{
    public enum MultiMathFunctionType { Addition = 0, Multiplication, Division, Special };

    public class MultiMathFunction: ICloneable
    {
        private delegate void CalculatingVoid(ref double result, double newValue);
        private delegate void TransformingVoid(ref MathFunction result, MathFunction newFunc);

        protected double coef;
        protected List<MultiMathFunction> functions;
        protected MultiMathFunctionType type;

        public override string ToString()
        {
            string result = coef != 1 ? Math.Round(coef, 2) + " * " : "" + "(";
            string splitter = type == MultiMathFunctionType.Addition ? " + " : type == MultiMathFunctionType.Multiplication ? " * " : " : ";

            for (int i = 0; i < functions.Count; i++)
                result += functions[i].ToString() + (i != (functions.Count - 1) ? splitter : "");

            result += ")";

            return result;
        }

        public MultiMathFunction(double coef, MultiMathFunctionType type, params MultiMathFunction[] functions)
        {
            Initialize(coef, type, functions);
        }

        public MultiMathFunction()
        {
            coef = 1.0;
            type = MultiMathFunctionType.Addition;
        }
        private void Initialize(double coef, MultiMathFunctionType type, IEnumerable<MultiMathFunction> functions)
        {
            this.coef = coef;
            this.type = type;
            this.functions = new List<MultiMathFunction>();

            if (functions != null)
                foreach (MultiMathFunction f in functions)
                    this.functions.Add(f.Clone() as MultiMathFunction);
        }

        public virtual object Clone()
        {
            return new MultiMathFunction(coef, type, functions.ToArray());
        }

        public virtual double Calculate(Dictionary<uint, double> variables)
        {
            if (coef == 0)
                return 0;

            if (functions.Count == 0)
                return coef;

            if (functions.Count == 1)
                return coef * functions[0].Calculate(variables);

            switch(type)
            {
                case MultiMathFunctionType.Addition:
                    return Calculate(variables, (ref double result, double newValue) => { result += newValue; });
                case MultiMathFunctionType.Multiplication:
                    return Calculate(variables, (ref double result, double newValue) => { result *= newValue; });
                case MultiMathFunctionType.Division:
                    return Calculate(variables, (ref double result, double newValue) => { result /= newValue; });
                default:
                    return coef;
            }
        }
        private double Calculate(Dictionary<uint, double> variables, CalculatingVoid cVoid)
        {
            if (functions == null || functions.Count == 0)
                return coef;

            double result = functions[0].Calculate(variables);

            for (int i = 1; i < functions.Count; i++)
                cVoid(ref result, functions[i].Calculate(variables));

            result *= coef;
            return result;
        }

        public virtual MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            if (functions == null || functions.Count == 0)
                return new ConstFunction(coef);

            if (coef == 0)
                return new ConstFunction(0);

            switch(type)
            {
                case MultiMathFunctionType.Addition:
                    return TransformToSimpleFunction(variables, (ref MathFunction result, MathFunction newFunc) =>
                    {
                        result += newFunc;
                    });
                case MultiMathFunctionType.Multiplication:
                    return TransformToSimpleFunction(variables, (ref MathFunction result, MathFunction newFunc) =>
                    {
                        result *= newFunc;
                    });
                case MultiMathFunctionType.Division:
                    return TransformToSimpleFunction(variables, (ref MathFunction result, MathFunction newFunc) =>
                    {
                        result /= newFunc;
                    });
                default:
                    return coef;
            }
        }
        private MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables, TransformingVoid tVoid)
        {
            if (functions == null || functions.Count == 0)
                return new ConstFunction(coef);

            MathFunction result = functions[0].TransformToSimpleFunction(variables);

            for (int i = 1; i < functions.Count; i++)
                tVoid(ref result, functions[i].TransformToSimpleFunction(variables));

            result *= coef;

            return result;
        }

        public static MultiMathFunction operator+(MultiMathFunction f1, MultiMathFunction f2)
        {
            return AddFunctions(f1, f2);
        }
        public static MultiMathFunction operator-(MultiMathFunction f1, MultiMathFunction f2)
        {
            return AddFunctions(f1, -f2);
        }
        private static MultiMathFunction AddFunctions(MultiMathFunction f1, MultiMathFunction f2)
        {
            if (f1.type == MultiMathFunctionType.Addition)
            {
                MultiMathFunction f1Copy = f1.Clone() as MultiMathFunction;
                f1Copy.functions.Add(f2.Clone() as MultiMathFunction);
                return f1Copy;
            }

            return new MultiMathFunction(1.0, MultiMathFunctionType.Addition, f1, f2);
        }
        public static MultiMathFunction operator-(MultiMathFunction f)
        {
            MultiMathFunction fCopy = f.Clone() as MultiMathFunction;
            fCopy.coef = -fCopy.coef;

            return fCopy;
        }

        public static MultiMathFunction operator*(MultiMathFunction f1, MultiMathFunction f2)
        {
            if (f1.type == MultiMathFunctionType.Multiplication)
            {
                MultiMathFunction f1Copy = f1.Clone() as MultiMathFunction;
                f1Copy.functions.Add(f2.Clone() as MultiMathFunction);
                return f1Copy;
            }

            return new MultiMathFunction(1.0, MultiMathFunctionType.Multiplication, f1, f2);
        }
        public static MultiMathFunction operator*(double coef, MultiMathFunction f)
        {
            MultiMathFunction newF = f.Clone() as MultiMathFunction;
            newF.coef *= coef;

            return newF;
        }
        public static MultiMathFunction operator*(MultiMathFunction f, double coef)
        {
            return coef * f;
        }

        public static MultiMathFunction operator/(MultiMathFunction f1, MultiMathFunction f2)
        {
            if (f1.type == MultiMathFunctionType.Division)
            {
                MultiMathFunction f1Copy = f1.Clone() as MultiMathFunction;
                f1Copy.functions.Add(f2.Clone() as MultiMathFunction);
                return f1Copy;
            }

            return new MultiMathFunction(1.0, MultiMathFunctionType.Division, f1, f2);
        }
        public static MultiMathFunction operator/(MultiMathFunction f, double coef)
        {
            MultiMathFunction fCopy = f.Clone() as MultiMathFunction;
            fCopy.coef /= coef;

            return fCopy;
        }

        public static implicit operator MultiMathFunction(double coef)
        {
            return new ConstMultiFunction(coef);
        }
        public static MultiMathFunction operator^(MultiMathFunction f1, MultiMathFunction f2)
        {
            return new PowerMultiFunction(1.0, f1, f2);
        }
    }
    public class FoundationFunction : MultiMathFunction
    {
        protected MultiMathFunction foundation;

        public FoundationFunction(double coef, MultiMathFunction foundation)
        {
            this.coef = coef;
            this.foundation = foundation.Clone() as MultiMathFunction;
            this.type = MultiMathFunctionType.Special;
        }
    }

    public class ArgumentFunction : MultiMathFunction
    {
        private uint index;

        public ArgumentFunction(double coef, uint index)
        {
            this.coef = coef;
            this.index = index;
            this.type = MultiMathFunctionType.Special;
        }

        public override object Clone()
        {
            return new ArgumentFunction(coef, index);
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", index);
        }

        public override double Calculate(Dictionary<uint, double> variables)
        {
            return coef * variables[index];
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            if (variables.ContainsKey(index))
                return new ConstFunction(coef * variables[index]);
            else
                return new XFunction(coef);

        }
    }
    public class CosMultiFunction : FoundationFunction
    {
        public CosMultiFunction(double coef, MultiMathFunction foundation) : base(coef, foundation)
        {
        }

        public override object Clone()
        {
            return new CosMultiFunction(coef, foundation);
        }

        public override string ToString()
        {
            return string.Format("{0}cos[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", foundation);
        }

        public override double Calculate(Dictionary<uint, double> variables)
        {
            return coef * Math.Cos(foundation.Calculate(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new CosFunction(coef, foundation.TransformToSimpleFunction(variables));
        }
    }
    public class SinMultiFunction : FoundationFunction
    {
        public SinMultiFunction(double coef, MultiMathFunction foundation) : base(coef, foundation)
        {
        }

        public override object Clone()
        {
            return new SinMultiFunction(coef, foundation);
        }

        public override string ToString()
        {
            return string.Format("{0}sin[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", foundation);
        }

        public override double Calculate(Dictionary<uint, double> variables)
        {
            return coef * Math.Sin(foundation.Calculate(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new SinFunction(coef, foundation.TransformToSimpleFunction(variables));
        }
    }
    public class LnMultiFunction : FoundationFunction
    {
        public LnMultiFunction(double coef, MultiMathFunction foundation) : base(coef, foundation)
        {
        }

        public override object Clone()
        {
            return new LnMultiFunction(coef, foundation);
        }

        public override string ToString()
        {
            return string.Format("{0}ln[{1}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", foundation.ToString());
        }

        public override double Calculate(Dictionary<uint, double> variables)
        {
            return coef * Math.Log(foundation.Calculate(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new LnFunction(coef, foundation.TransformToSimpleFunction(variables));
        }
    }
    public class PowerMultiFunction : MultiMathFunction
    {
        MultiMathFunction foundation;
        MultiMathFunction power;

        public PowerMultiFunction(double coef, MultiMathFunction foundation, MultiMathFunction power)
        {
            this.coef = coef;
            this.foundation = foundation.Clone() as MultiMathFunction;
            this.power = power.Clone() as MultiMathFunction;
            this.type = MultiMathFunctionType.Special;
        }

        public override object Clone()
        {
            return new PowerMultiFunction(coef, foundation, power);
        }
        
        public override string ToString()
        {
            return string.Format("{0}({1}) ^ [{2}]", coef != 1 ? Math.Round(coef, 2).ToString() + " * " : "", foundation, power);
        }

        public override double Calculate(Dictionary<uint, double> variables)
        {
            return coef * Math.Pow(foundation.Calculate(variables), power.Calculate(variables));
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new PowerFunction(coef, foundation.TransformToSimpleFunction(variables), power.TransformToSimpleFunction(variables));
        }
    }
    public class ConstMultiFunction : MultiMathFunction
    {
        public ConstMultiFunction(double coef)
        {
            this.coef = coef;
            this.type = MultiMathFunctionType.Special;
        }

        public override object Clone()
        {
            return new ConstMultiFunction(coef);
        }


        public override string ToString()
        {
            return Math.Round(coef, 2).ToString();
        }

        public override double Calculate(Dictionary<uint, double> variables)
        {
            return coef;
        }
        public override MathFunction TransformToSimpleFunction(Dictionary<uint, double> variables)
        {
            return new ConstFunction(coef);
        }
    }
}
