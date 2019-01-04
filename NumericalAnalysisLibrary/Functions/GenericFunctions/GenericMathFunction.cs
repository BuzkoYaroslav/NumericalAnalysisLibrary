using System;
using System.Collections.Generic;

namespace NumericalAnalysisLibrary.Functions.Generic
{
    [Serializable]
    public enum MathFunctionType { Sum, Multiplication, Division }

    [Serializable]
    public class CosFunction<T> : MathFunction<T> where T: new()
    {
        public CosFunction(T coef, MathFunction<T> foundation) : base()
        {
            this.coef = coef;

            functions.Add(foundation.Clone() as MathFunction<T>);
        }

        public override T Calculate(T x)
        {
            return coef * Math.Cos(((double)(dynamic)functions[0].Calculate(x)));
        }
        public override MathFunction<T> Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return new CosFunction<T>(coef, functions[0]);

            return (new SinFunction<T>(-(dynamic)coef, functions[0]) * functions[0].Derivative(1)).Derivative(order - 1);
        }

        public override string ToString()
        {
            string c = "";
            if (coef != 1)
                c = string.Format("{0:E} * (", coef);
            return string.Format("{0}cos[{1}]", c, functions[0].ToString());
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        protected override MathFunction<T> MinusFunction()
        {
            return new CosFunction<T>(-coef, functions[0]);
        }
        protected override MathFunction<T> Multiply(T coef)
        {
            return new CosFunction<T>(coef * this.coef, functions[0]);
        }

        public override object Clone()
        {
            return new CosFunction<T>(coef, functions[0]);
        }
    }
    [Serializable]
    public class SinFunction<T> : MathFunction<T> where T: new()
    {
        public SinFunction(T coef, MathFunction<T> foundation) : base()
        {
            this.coef = coef;

            functions.Add(foundation.Clone() as MathFunction<T>);
        }

        public override T Calculate(T x)
        {
            return coef * Math.Sin((double)(dynamic)functions[0].Calculate(x));
        }
        public override MathFunction<T> Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return new SinFunction<T>(coef, functions[0]);

            return (new CosFunction<T>(coef, functions[0]) * functions[0].Derivative(1)).Derivative(order - 1);
        }

        public override string ToString()
        {
            string c = "";
            if (coef != 1)
                c = string.Format("{0:E} * (", coef);
            return string.Format("{0}sin[{1}]", c, functions[0].ToString());
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        protected override MathFunction<T> MinusFunction()
        {
            return new SinFunction<T>(-coef, functions[0]);
        }
        protected override MathFunction<T> Multiply(T coef)
        {
            return new SinFunction<T>(coef * this.coef, functions[0]);
        }

        public override object Clone()
        {
            return new SinFunction<T>(coef, functions[0]);
        }
    }
    [Serializable]
    public class PowerFunction<T> : MathFunction<T> where T: new()
    {
        public PowerFunction(T coef, MathFunction<T> foundation, MathFunction<T> power) : base()
        {
            this.coef = coef;
            functions.Add(foundation.Clone() as MathFunction<T>);
            functions.Add(power.Clone() as MathFunction<T>);
        }

        public override T Calculate(T x)
        {
            return coef * Math.Pow((dynamic)functions[0].Calculate(x), (dynamic)functions[1].Calculate(x));
        }
        public override MathFunction<T> Derivative(int order)
        {
            dynamic f = functions[0],
                          g = functions[1];

            return new PowerFunction<T>(coef, f, g - 1) * f.Derivative(1);
        }

        public override string ToString()
        {
            string c = "";
            if (coef != 1)
                c = string.Format("{0:E} * (", coef);
            return string.Format("{0}({1}) ^ [{2}]", c, functions[0], functions[1]);
        }

        public override bool IsZero()
        {
            return functions[0].IsZero() && coef == 0;
        }

        protected override MathFunction<T> MinusFunction()
        {
            return new PowerFunction<T>(-coef, functions[0], functions[1]);
        }
        protected override MathFunction<T> Multiply(T coef)
        {
            return new PowerFunction<T>(coef * this.coef, functions[0], functions[1]);
        }

        public override object Clone()
        {
            return new PowerFunction<T>(coef, functions[0], functions[1]);
        }
    }
    [Serializable]
    public class StepFunction<T> : PowerFunction<T> where T: new()
    {
        public StepFunction(T coef, MathFunction<T> foundation, MathFunction<T> power) : base(coef, foundation, power)
        {
        }

        public override MathFunction<T> Derivative(int order)
        {
            var f = functions[0];
            var g = functions[1];

            return new PowerFunction<T>((dynamic)1.0, f, g) * new LnFunction<T>((dynamic)1.0, f) * g.Derivative(1);

        }

        public override object Clone()
        {
            return new StepFunction<T>(coef, functions[0], functions[1]);
        }
    }
    [Serializable]
    public class LnFunction<T> : MathFunction<T> where T: new()
    {
        public LnFunction(T coef, MathFunction<T> foundation) : base()
        {
            this.coef = coef;

            functions.Add(foundation.Clone() as MathFunction<T>);
        }

        public override T Calculate(T x)
        {
            return coef * Math.Log((double)(dynamic)functions[0].Calculate(x));
        }
        public override MathFunction<T> Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return new LnFunction<T>(coef, functions[0]);

            return (coef * functions[0].Derivative(1) / functions[0]).Derivative(order - 1);
        }

        public override string ToString()
        {
            string c = "";
            if (coef != 1)
                c = string.Format("{0:E} * (", coef);
            return string.Format("{0}ln[{1}]", c, functions[0].ToString());
        }

        public override bool IsZero()
        {
            return coef == 0;
        }
        protected override MathFunction<T> MinusFunction()
        {
            return new LnFunction<T>(-coef, functions[0]);
        }
        protected override MathFunction<T> Multiply(T coef)
        {
            return new LnFunction<T>(coef * this.coef, functions[0]);
        }

        public override object Clone()
        {
            return new LnFunction<T>(coef, functions[0]);
        }
    }
    [Serializable]
    public class ConstFunction<T> : MathFunction<T> where T: new()
    {
        public ConstFunction(T coef) : base()
        {
            this.coef = coef;
        }

        public override T Calculate(T x)
        {
            return coef;
        }
        public override MathFunction<T> Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect order!");

            return order == 0 ? coef : (T)(dynamic)0;
        }

        public override string ToString()
        {
            return string.Format("{0:E}", coef);
        }

        public override bool IsZero()
        {
            return coef == 0;
        }
        protected override MathFunction<T> MinusFunction()
        {
            return new ConstFunction<T>(-coef);
        }
        protected override MathFunction<T> Multiply(T coef)
        {
            return new ConstFunction<T>(coef * this.coef);
        }

        public override object Clone()
        {
            return new ConstFunction<T>(coef);
        }
    }
    [Serializable]
    public class XFunction<T> : MathFunction<T> where T: new()
    {
        public XFunction(T coef) : base()
        {
            this.coef = coef;
        }

        public override T Calculate(T x)
        {
            return coef * x;
        }
        public override MathFunction<T> Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            return order == 0 ? new XFunction<T>(coef) : ((MathFunction<T>)coef).Derivative(order - 1);
        }

        public override string ToString()
        {
            string c = "";
            if (coef != 1)
                c = string.Format("{0:E} * (", coef);
            return string.Format("{0}x", c);
        }

        public override bool IsZero()
        {
            return coef == 0;
        }

        protected override MathFunction<T> MinusFunction()
        {
            return new XFunction<T>(-coef);
        }
        protected override MathFunction<T> Multiply(T coef)
        {
            return new XFunction<T>(coef * this.coef);
        }

        public override object Clone()
        {
            return new XFunction<T>(coef);
        }
    }
    [Serializable]
    public class AbsFunction<T> : MathFunction<T> where T: new()
    {
        public AbsFunction(T coef, MathFunction<T> foundation)
        {
            this.coef = coef;

            if (this.coef != 0)
                functions.Add(foundation.Clone() as MathFunction<T>);
        }

        public override MathFunction<T> Derivative(int order)
        {
            throw new InvalidOperationException(Constants.MathFunctionConstants.derivativeDoesNotExistExceptionMessage);
        }

        public override T Calculate(T x)
        {
            return Math.Abs((dynamic)functions[0].Calculate(x));
        }

        public override string ToString()
        {
            string c = "";
            if (coef != 1)
                c = string.Format("{0:E} * (", coef);
            return string.Format("{0}|{1}|", c, functions[0]);
        }

        public override bool IsZero()
        {
            return functions[0].IsZero();
        }

        protected override MathFunction<T> MinusFunction()
        {
            return new AbsFunction<T>(-coef, functions[0]);
        }

        public override object Clone()
        {
            return new AbsFunction<T>(coef, functions[0]);
        }
    }
    [Serializable]
    public class ACosFunction<T> : MathFunction<T> where T: new()
    {
        public ACosFunction(T coef, MathFunction<T> foundation) : base()
        {
            this.coef = coef;

            functions.Add(foundation.Clone() as MathFunction<T>);
        }

        public override T Calculate(T x)
        {
            return coef * Math.Acos((double)(dynamic)functions[0].Calculate(x));
        }
        public override MathFunction<T> Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return this;

            return (-1 / (1 + (functions[0] ^ (dynamic)2)) * functions[0].Derivative(1)).Derivative(order - 1);
        }

        public override string ToString()
        {
            return string.Format("{0:F2}{1}ACos[{2}]", coef != 1 ? coef : "", coef != 1 ? " * " : "", functions[0].ToString());
        }

        public override bool IsZero()
        {
            return coef == 0 || ((dynamic)functions[0] - 1).IsZero();
        }

        protected override MathFunction<T> MinusFunction()
        {
            return new ACosFunction<T>(-coef, functions[0]);
        }
        protected override MathFunction<T> Multiply(T coef)
        {
            return new ACosFunction<T>(coef * this.coef, functions[0]);
        }

        public override object Clone()
        {
            return new ACosFunction<T>(coef, functions[0]);
        }
    }
    [Serializable]
    public class ASinFunction<T> : MathFunction<T> where T: new()
    {
        public ASinFunction(T coef, MathFunction<T> foundation) : base()
        {
            this.coef = coef;

            functions.Add(foundation.Clone() as MathFunction<T>);
        }

        public override T Calculate(T x)
        {
            return coef * Math.Asin((double)(dynamic)functions[0].Calculate(x));
        }
        public override MathFunction<T> Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return this;

            return (1 / (1 + (functions[0] ^ (dynamic)2)) * functions[0].Derivative(1)).Derivative(order - 1);
        }

        public override string ToString()
        {
            return string.Format("{0:F2}{1}ASin[{2}]", coef != 1 ? coef : "", coef != 1 ? " * " : "", functions[0].ToString());
        }

        public override bool IsZero()
        {
            return coef == 0 || functions[0].IsZero();
        }

        protected override MathFunction<T> MinusFunction()
        {
            return new ASinFunction<T>(-coef, functions[0]);
        }
        protected override MathFunction<T> Multiply(T coef)
        {
            return new ASinFunction<T>(coef * this.coef, functions[0]);
        }

        public override object Clone()
        {
            return new ASinFunction<T>(coef, functions[0]);
        }
    }

    [Serializable]
    public class MathFunction<T> : ICloneable where T: new()
    {
        private delegate bool Condition(T f, T best);
        private delegate bool SingleCondition(T f);

        protected dynamic coef;
        protected MathFunctionType type;
        protected List<MathFunction<T>> functions;

        #region Constructor
        public MathFunction()
        {
            functions = new List<MathFunction<T>>();
            type = MathFunctionType.Sum;
            coef = new T();
        }
        public MathFunction(T coef, MathFunctionType type, params MathFunction<T>[] functions)
        {
            this.functions = new List<MathFunction<T>>();

            this.coef = coef;
            this.type = type;

            switch (type)
            {
                case MathFunctionType.Sum:
                    bool allZero = true;

                    foreach (var func in functions)
                        if (!func.IsZero())
                        {
                            this.functions.Add(func);
                            allZero = false;
                        }

                    if (allZero)
                        this.coef = new T();
                    break;
                case MathFunctionType.Multiplication:
                    for (int i = 0; i < functions.Length; i++)
                        if (functions[i].IsZero())
                        {
                            coef = new T();
                            break;
                        }

                    foreach (var func in functions)
                        this.functions.Add(func);
                    break;
                case MathFunctionType.Division:
                    InitializeDivision(functions);

                    if (functions[0].IsZero())
                    {
                        coef = new T();
                        this.functions.Clear();
                        break;
                    }

                    break;
            }
        }
        #endregion

        public virtual object Clone()
        {
            List<MathFunction<T>> funcs = new List<MathFunction<T>>();

            foreach (var func in functions)
                funcs.Add(func.Clone() as MathFunction<T>);

            return new MathFunction<T>(coef, type, funcs.ToArray());
        }

        #region Bool features
        public virtual bool IsZero()
        {
            foreach (var func in functions)
                if (func.IsZero())
                    return true;

            return false;
        }

        private bool IsGood(SingleCondition cond, T a, T b, T step)
        {
            try
            {
                for (T i = a; (dynamic)i <= (dynamic)b; i += (dynamic)Constants.epsilan)
                {
                    if (!cond(Calculate(i)))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (NotFiniteNumberException)
            {
                return false;
            }
            catch (DivideByZeroException)
            {
                return false;
            }
        }

        public bool IsGreaterThanZero(T a, T b)
        {
            return IsGood(new SingleCondition((T func) => { return (dynamic)func > (dynamic)0; }), a, b, (dynamic)Constants.epsilan);
        }
        public bool IsSmallerThanZero(T a, T b)
        {
            return IsGood(new SingleCondition((T f) => { return (dynamic)f < 0; }), a, b, (dynamic)Constants.epsilan);
        }
        public bool IsContinuous(T a, T b)
        {
            return IsGood(new SingleCondition((T func) => { return !double.IsInfinity((double)(dynamic)func) && !double.IsNaN((double)(dynamic)func); }),
                          a, b, (dynamic)Constants.epsilan);
        }
        public bool IsContinuous(T a, T b, T downYBound, T upYBound, T eps)
        {
            return IsGood(new SingleCondition((T func) => { return (dynamic)func > downYBound && (dynamic)func < upYBound; }),
                          a, b, eps);
        }
        public bool IsWithConstSign(T a, T b)
        {
            return IsSmallerThanZero(a, b) || IsGreaterThanZero(a, b);
        }

        public Tuple<T, T>[] ContinuousRegions(T a, T b, T downYBound, T upYBound)
        {
            List<Tuple<T, T>> regions = new List<Tuple<T, T>>();
            dynamic epsilan = Math.Pow(10, -2);

            dynamic left = a, right = a + epsilan;

            while (true)
            {
                if ((dynamic)right > b)
                {
                    if (IsContinuous(left, right - epsilan, downYBound, upYBound, epsilan))
                        regions.Add(Tuple.Create(left, right - epsilan));

                    break;
                }
                if (IsContinuous(left, right, downYBound, upYBound, epsilan))
                {
                    right += epsilan;
                    continue;
                }

                if (left < right - epsilan &&
                    IsContinuous(left, right - epsilan, downYBound, upYBound, epsilan))
                    regions.Add(Tuple.Create(left, right - epsilan));

                left = right;
                right += epsilan;
            }

            return regions.ToArray();
        }

        #endregion

        #region Public methods
        private void InitializeDivision(MathFunction<T>[] functions)
        {
            dynamic up = functions[0];
            dynamic low = functions[1];

            for (int i = 2; i < functions.Length; i++)
            {
                if (i % 2 == 0)
                    up *= functions[i];
                else
                    low *= functions[i];
            }

            this.functions.Add(up);
            this.functions.Add(low);
        }

        public virtual T Calculate(T x)
        {
            if (functions.Count == 0)
                return coef;

            dynamic result = functions[0].Calculate(x);

            for (int i = 1; i < functions.Count; i++)
                switch (type)
                {
                    case MathFunctionType.Sum:
                        result += functions[i].Calculate(x);
                        break;
                    case MathFunctionType.Multiplication:
                        result *= functions[i].Calculate(x);
                        break;
                    case MathFunctionType.Division:
                        result /= functions[i].Calculate(x);
                        break;
                }

            result *= coef;

            return result;
        }
        public virtual MathFunction<T> Derivative(int order)
        {
            if (order < 0)
                throw new Exception("Incorrect derivative order!");

            if (order == 0)
                return new MathFunction<T>(coef, type, functions.ToArray());

            if (functions.Count == 0)
                return new MathFunction<T>();

            dynamic result = new T();

            switch (type)
            {
                case MathFunctionType.Sum:
                    result = functions[0].Derivative(order);

                    for (int i = 1; i < functions.Count; i++)
                        result += functions[i].Derivative(order);

                    break;
                case MathFunctionType.Multiplication:
                    result = new ConstFunction<T>((dynamic)0);

                    for (int i = 0; i < functions.Count; i++)
                    {
                        dynamic func = functions[i].Derivative(1);
                        for (int j = 0; j < functions.Count; j++)
                            if (i != j)
                                func *= functions[j];

                        result += func;
                    }

                    result = result.Derivative(order - 1);

                    break;
                case MathFunctionType.Division:
                    var f = functions[0];
                    var g = functions[1];

                    result = (f.Derivative(1) * g - g.Derivative(1) * f) / (g ^ (T)(dynamic)2);

                    result = result.Derivative(order - 1);
                    break;
            }

            return result;
        }
        public override string ToString()
        {
            string result;
            if ((dynamic)coef != 1)
                result = string.Format("{0:E} * (", coef);
            else
                result = "(";

            string splitter = type == MathFunctionType.Sum ? " + " : type == MathFunctionType.Multiplication ? " * " : " : ";

            for (int i = 0; i < functions.Count; i++)
                result += functions[i].ToString() + (i != (functions.Count - 1) ? splitter : "");

            result += ")";

            return result;
        }
        #endregion

        #region Overrided operators
        protected virtual MathFunction<T> MinusFunction()
        {
            return new MathFunction<T>(-(dynamic)coef, type, functions.ToArray());
        }
        protected virtual MathFunction<T> Multiply(T coef)
        {
            return new MathFunction<T>((dynamic)coef * this.coef, type, functions.ToArray());
        }

        public static MathFunction<T> operator -(MathFunction<T> func)
        {
            return func.MinusFunction();
        }
        public static MathFunction<T> operator +(MathFunction<T> func1, MathFunction<T> func2)
        {
            return new MathFunction<T>((dynamic)1, MathFunctionType.Sum, func1, func2);
        }
        public static MathFunction<T> operator -(MathFunction<T> func1, MathFunction<T> func2)
        {
            return func1 + (-func2);
        }
        public static MathFunction<T> operator *(MathFunction<T> func1, MathFunction<T> func2)
        {
            return new MathFunction<T>((dynamic)1, MathFunctionType.Multiplication, func1, func2);
        }
        public static MathFunction<T> operator /(MathFunction<T> func1, MathFunction<T> func2)
        {
            return new MathFunction<T>((dynamic)1, MathFunctionType.Division, func1, func2);
        }
        public static MathFunction<T> operator *(MathFunction<T> func, T coef)
        {
            return (dynamic)coef * func;
        }
        public static MathFunction<T> operator *(T coef, MathFunction<T> func)
        {
            return func.Multiply(coef);
        }
        public static MathFunction<T> operator /(MathFunction<T> func, T coef)
        {
            return ((dynamic)1 / coef) * func;
        }
        public static MathFunction<T> operator ^(MathFunction<T> func, T coef)
        {
            return new PowerFunction<T>((dynamic)1, func, coef);
        }
        public static MathFunction<T> operator ^(T coef, MathFunction<T> func)
        {
            return new StepFunction<T>((dynamic)1, coef, func);
        }

        public static implicit operator MathFunction<T>(T number)
        {
            return new ConstFunction<T>(number);
        }
        #endregion

        #region Global extremum

        public double MaxValue(T a, T b, bool isAbsolute = true)
        {
            return FindValue(a, b, new Condition((T f, T best) => { return (dynamic)f > (dynamic)best; }), (dynamic)double.MinValue, isAbsolute);
        }
        public double MinValue(T a, T b, bool isAbsolute = true)
        {
            return FindValue(a, b, new Condition((T f, T best) => { return (dynamic)f < (dynamic)best; }), (dynamic)double.MaxValue, isAbsolute);
        }

        private double FindValue(T a, T b, Condition cond, T startVal, bool isAbsolute)
        {
            dynamic result = startVal;

            for (dynamic x = a; x <= b; x += Constants.epsilan)
            {
                dynamic f = Calculate(x);
                f = isAbsolute ? Math.Abs(f) : f;

                if (cond(f, result)) result = f;
            }

            return result;
        }
        #endregion
    }
}
