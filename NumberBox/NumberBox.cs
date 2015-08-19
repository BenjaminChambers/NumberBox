using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace NumberBox
{
    public class NumberBox : TextBox
    {
        // Constructors
        public NumberBox()
        {
            KeyDown += new KeyEventHandler(OnKeyDown);
            TextChanged += new TextChangedEventHandler(OnTextChanged);

            InputScopeName name = new InputScopeName();
            name.NameValue = InputScopeNameValue.Number;
            InputScope scope = new InputScope();
            scope.Names.Add(name);

            InputScope = scope;

            Evaluate();
        }

        // Public Dependency Properties
        new public string Text
        {
            get { return base.Text; }
            set { Evaluate(value); }
        }
        public double Number {
            get { return (double)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public bool IsNegative { get { return (bool)GetValue(IsNegativeProperty); } set { SetValue(IsNegativeProperty, value); } }

        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set
            {
                if (value >= 0)
                    SetValue(DecimalPlacesProperty, value);
                else
                    SetValue(DecimalPlacesProperty, 0);

                Evaluate();
            }
        }

        public bool AllowNegativeValues { get { return (bool)GetValue(AllowNegativeValuesProperty); } set { SetValue(AllowNegativeValuesProperty, value); Evaluate(); } }
        public string Prefix { get { return (string)GetValue(PrefixProperty); } set { SetValue(PrefixProperty, value); Evaluate(); } }
        public string Postfix { get { return (string)GetValue(PostfixProperty); } set { SetValue(PostfixProperty, value); Evaluate(); } }


        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(double), typeof(NumberBox), new PropertyMetadata((double)0, OnNumberChanged));

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(NumberBox), new PropertyMetadata((int)0));

        public static readonly DependencyProperty AllowNegativeValuesProperty =
            DependencyProperty.Register("AllowNegativeValues", typeof(bool), typeof(NumberBox), new PropertyMetadata(false));

        public static readonly DependencyProperty PrefixProperty =
            DependencyProperty.Register("Prefix", typeof(string), typeof(NumberBox), new PropertyMetadata(""));

        public static readonly DependencyProperty PostfixProperty =
            DependencyProperty.Register("Postfix", typeof(string), typeof(NumberBox), new PropertyMetadata(""));

        public static readonly DependencyProperty IsNegativeProperty =
            DependencyProperty.Register("IsNegative", typeof(bool), typeof(NumberBox), new PropertyMetadata(false));

        // Default callbacks
        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Number0:
                case Windows.System.VirtualKey.NumberPad0:
                    _digits.Add(0); break;
                case Windows.System.VirtualKey.Number1:
                case Windows.System.VirtualKey.NumberPad1:
                    _digits.Add(1); break;
                case Windows.System.VirtualKey.Number2:
                case Windows.System.VirtualKey.NumberPad2:
                    _digits.Add(2); break;
                case Windows.System.VirtualKey.Number3:
                case Windows.System.VirtualKey.NumberPad3:
                    _digits.Add(3); break;
                case Windows.System.VirtualKey.Number4:
                case Windows.System.VirtualKey.NumberPad4:
                    _digits.Add(4); break;
                case Windows.System.VirtualKey.Number5:
                case Windows.System.VirtualKey.NumberPad5:
                    _digits.Add(5); break;
                case Windows.System.VirtualKey.Number6:
                case Windows.System.VirtualKey.NumberPad6:
                    _digits.Add(6); break;
                case Windows.System.VirtualKey.Number7:
                case Windows.System.VirtualKey.NumberPad7:
                    _digits.Add(7); break;
                case Windows.System.VirtualKey.Number8:
                case Windows.System.VirtualKey.NumberPad8:
                    _digits.Add(8); break;
                case Windows.System.VirtualKey.Number9:
                case Windows.System.VirtualKey.NumberPad9:
                    _digits.Add(9); break;
                case Windows.System.VirtualKey.Delete:
                case Windows.System.VirtualKey.Back:
                    if (_digits.Count > 0)
                        _digits.RemoveAt(_digits.Count - 1);
                    break;
                case Windows.System.VirtualKey.Subtract:
                    if (AllowNegativeValues)
                        IsNegative = !IsNegative;
                    break;
            }

            e.Handled = true;
            Evaluate();
        }

        // Callbacks
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Evaluate(Text);
        }

        private static void OnNumberChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            NumberBox nb = source as NumberBox;

            if (nb._suppressParsing == false)
                nb.Text = e.NewValue.ToString();
        }

        // Internal
        List<int> _digits = new List<int>();
        private bool _suppressParsing = false;


        private void Evaluate()
        {
            StringBuilder whole = new StringBuilder();
            StringBuilder part = new StringBuilder();

            // Remove leading zeroes
            while ((_digits.Count > 0) && (_digits[0] == 0))
                _digits.RemoveAt(0);

            // Add back any required leading zeroes
            while (_digits.Count <= DecimalPlaces)
                _digits.Insert(0, 0);

            // Create strings of numbers with separators
            int separation = _digits.Count - DecimalPlaces;
            var work = _digits.Take(separation);
            int i = separation;
            foreach (int num in work)
            {
                whole.Append(num.ToString());
                if ((i % 3 == 1) && (i > 1))
                    whole.Append(NumberFormatInfo.CurrentInfo.NumberGroupSeparator);
                i--;
            }

            for (i = separation; i < _digits.Count; i++)
                part.Append(_digits[i].ToString());

            // Evaluate those strings
            string temp = (IsNegative ? NumberFormatInfo.CurrentInfo.NegativeSign : "")
                + whole.ToString()
                + (DecimalPlaces > 0 ? NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + part.ToString() : "");
            SetNumber(double.Parse(temp));
            base.Text = Prefix + temp + Postfix;
        }

        private void Evaluate(string s)
        {
            _digits.Clear();
            IsNegative = false;
            if (AllowNegativeValues)
            {
                foreach (char c in s)
                    if (c == '-')
                        IsNegative = !IsNegative;
            }

            foreach (char c in s)
            {
                if (char.IsDigit(c))
                    _digits.Add(int.Parse(c.ToString()));
            }

            Evaluate();
        }

        private void SetNumber(double value)
        {
            _suppressParsing = true;
            Number = value;
            _suppressParsing = false;
        }
    }
}
