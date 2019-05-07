using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearRegression;
using PhysLogger.Hardware;

namespace PhysLogger.Maths
{
    public class CalibrationFunction
    {
        public CalibrationFunction(Function tf, string instrumentID)
        {
            TF = tf;
            InstrumentID = instrumentID;
            menuOption = new CalibrationFunctionOption(this);
        }
        public object[] parameters { get; set; }
        public Function TF { get; set; }
        public string InstrumentID { get; set; }

        ChannelOption menuOption;
        public ChannelOption MenuItem { get { return menuOption; } }

    }
    public class FunctionParamter
    {
        public object Value { get; set; }
        public string Title { get; set; }
    }
    public class CalibrationFunctionsCollection
    {
        protected List<CalibrationFunction> Items { get; set; } = new List<CalibrationFunction>();
        public CalibrationFunction Selected
        {
            get
            {
                if (Items.Count == 1)
                    return Items[0];
                return Items.Find(item => item.MenuItem.Checked);
            }
            set
            {
                if (Items.Count == 1)
                    return;
                foreach (var item in Items)
                    item.MenuItem.Checked = item == value;
            }
        }
        public int Count { get { return Items.Count; } }
        public void Add(Function f, string InstrumentID)
        {
            var calibOp = new CalibrationFunction(f, InstrumentID);
            calibOp.MenuItem.MenuItem.Click += MenuItem_Click;
            calibOp.MenuItem.MenuItem.AssociatedChannelOption = calibOp.MenuItem;
            menuItem.SubOptions.Add(calibOp.MenuItem);
            Items.Add(calibOp);
            
            if (Items.Count == 1)
                calibOp.MenuItem.Checked = true;
        }
        ChannelOption menuItem = new GroupHeadOption("Calibration");
        public ChannelOption MenuItem { get { return menuItem; } }
        private void MenuItem_Click(object sender, EventArgs e)
        {
            var calibOp = (CalibrationFunctionOption)(((ToolStripMenuItemWithChannelOption)sender).AssociatedChannelOption);
            Selected = calibOp.TF;
        }
    }
    public class Function
    {
        public string ID { get; set; } = "";
        protected Function() { }
        public float Evaluate(float x)
        {
            float y = x;
            if (InnerFunction != null)
                y = InnerFunction.Evaluate(x);
            y = EvaluateThis(y);
            if (OuterFunction != null)
                y = OuterFunction.Evaluate(y);
            return y;
        }
        protected virtual float EvaluateThis(float input)
        { throw new NotImplementedException(); }
        public static Function Make(string type, object[] paramters)
        {
            type = type.ToLower();
            if (type == "poly")
                return new PolyFunction((float[])paramters[0]);
            else if (type == "polyfit")
            {
                if (paramters.Length == 3)
                    return PolyFunction.FromPoints((float[])paramters[0], (float[])paramters[1], (int)paramters[2]);
                else
                    return PolyFunction.FromPoints((float[])paramters[0], (float[])paramters[1]);
            }
            else if (type == "cos")
                return new CosFunction();
            else if (type == "sin")
                return new SinFunction();
            else if (type == "tan")
                return new TanFunction();
            else if (type == "d2r")
                return new DegreesToRadiansFunction();
            else if (type == "r2d")
                return new DegreesToRadiansFunction();
            else if (type == "g1")
                return new G1Function();
            else if (type == "expression")
                return new AlgebraicFunction((string)paramters[0]);
            else
                throw new NotImplementedException();
        }
        public Function InnerFunction{ get; set; }
        public Function OuterFunction { get; set; }
        public static Function Parse(string str)
        {
            string name = str.Substring(0, str.IndexOf("("));
            string value = str.Substring(str.IndexOf("(") + 1);
            value = value.Substring(0, value.Length - 1);
            var args = new List<object>();
            int starts = 0;
            bool quoteStarted = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '"')
                {
                    if (!quoteStarted)
                        quoteStarted = true;
                    else
                    {
                        string toAdd = value.Substring(0, i);
                        args.Add(value.Substring(1, i - 1));
                        quoteStarted = false;
                    }
                    continue;
                }
                if (quoteStarted)
                    continue;
                if (value[i] == '[' || value[i] == '"')
                    starts++;
                else if (value[i] == ']')
                    starts--;
                if (value[i] == ',' && starts == 0 || i == value.Length - 1)
                {
                    string toAdd = value.Substring(0, i);
                    if (value.Length - 1 == i)
                    { toAdd = value; }
                    List<float> vals = new List<float>();
                    foreach (var toadd_ in toAdd.Split(new char[] { ',', '[', ']' }, StringSplitOptions.RemoveEmptyEntries))
                        vals.Add(Convert.ToSingle(toadd_));
                    if (vals.Count > 1)
                        args.Add(vals.ToArray());
                    else
                        args.Add(vals[0]);
                    value = value.Substring(i + 1);
                    i = -1;
                }
            }
            string[] parameters = value.Split(new char[] { ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return Function.Make(name, args.ToArray());
            }
            catch
            {
                throw new FormatException("The function could not be parsed");
            }
        }   
    }
    public class AlgebraicFunction : Function
    {
        public delegate object[] GetFloatArrayHandler();
        public event GetFloatArrayHandler GetParamtersRequest;
        string expression = "";
        public AlgebraicFunction(string expression)
        {
            this.expression = expression;
        }
        
        protected override float EvaluateThis(float input)
        {
            var paramaters = GetParamtersRequest();
            var numericExpression = string.Format(expression.Replace("x", input.ToString()), (object[])paramaters);
            return (float)(new NCalc.Expression(numericExpression).Evaluate());
        }
    }
    public class PolyFunction : Function
    {
        public float [] coefficients { get; private set; } = new float[1] { 1 };
        public PolyFunction(params float[] coefficients)
        {
            this.coefficients = new float[coefficients.Length];
            for (int i = 0; i < coefficients.Length; i++)
                this.coefficients[i] = coefficients[i];
        }
        public static PolyFunction FromPoints(float[] x, float[] y, int order = -1)
        {
            if (order == -1)
                order = x.Length - 1;
            double[] X = new double[x.Length];
            double[] Y = new double[x.Length];
            x.CopyTo(X, 0);
            y.CopyTo(Y, 0);
            var coefs = MathNet.Numerics.Fit.Polynomial(X, Y, order, DirectRegressionMethod.NormalEquations);
            float[] coefficients = new float[coefs.Length];
            for (int i = 0; i < coefs.Length; i++)
                coefficients[i] = (float)coefs[i];
            return new PolyFunction(coefficients);
        }
        public override string ToString()
        {
            string funcRep = "";
            for (int i = 0; i < coefficients.Length; i++)
                funcRep += 
                    coefficients[i].ToString() + 
                    (i >1 ?("x^" + i): (i == 1 ? "x":""))
                    + " + ";
            return "f(x) = " + funcRep.TrimEnd(new char[] { ' ', '+' });
        }
        protected override float EvaluateThis(float input)
        {
            float output = 0;
            for (int i = 0; i < coefficients.Length; i++)
                output += (float)Math.Pow(input, i) * coefficients[i];
            return output;
        }
    }
    public class SinFunction : Function
    {
        protected override float EvaluateThis(float input)
        {
            return (float)Math.Sin(input);
        }
    }
    public class CosFunction : Function
    {
        protected override float EvaluateThis(float input)
        {
            return (float)Math.Cos(input);
        }
    }
    public class RadiansToDegreesFunction : Function
    {
        protected override float EvaluateThis(float input)
        {
            return input / (float)Math.PI * 180.0F;
        }
    }
    public class G1Function : Function
    {
        protected override float EvaluateThis(float input)
        {
            return input;
        }
    }
    public class DegreesToRadiansFunction : Function
    {
        protected override float EvaluateThis(float input)
        {
            return input * (float)Math.PI / 180.0F;
        }
    }
    public class TanFunction : Function
    {
        protected override float EvaluateThis(float input)
        {
            return (float)Math.Tan(input);
        }
    }
}
