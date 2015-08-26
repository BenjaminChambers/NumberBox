using System;
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

            RefreshText();
        }

        // Public Dependency Properties
        public double Number
        {
            get { return (double)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public bool IsNegative { get { return (bool)GetValue(IsNegativeProperty); } set { SetValue(IsNegativeProperty, value); RefreshText(); } }

        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set
            {
                if (value >= 0)
                {
                    SetValue(DecimalPlacesProperty, value);
                }
                else
                {
                    SetValue(DecimalPlacesProperty, 0);
                }

                RefreshText();
            }
        }

        public bool AllowNegativeValues { get { return (bool)GetValue(AllowNegativeValuesProperty); } set { SetValue(AllowNegativeValuesProperty, value); } }
        public string Prefix { get { return (string)GetValue(PrefixProperty); } set { SetValue(PrefixProperty, value); RefreshText(); } }
        public string Postfix { get { return (string)GetValue(PostfixProperty); } set { SetValue(PostfixProperty, value); RefreshText(); } }


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
                    AddDigit(0); break;
                case Windows.System.VirtualKey.Number1:
                case Windows.System.VirtualKey.NumberPad1:
                    AddDigit(1); break;
                case Windows.System.VirtualKey.Number2:
                case Windows.System.VirtualKey.NumberPad2:
                    AddDigit(2); break;
                case Windows.System.VirtualKey.Number3:
                case Windows.System.VirtualKey.NumberPad3:
                    AddDigit(3); break;
                case Windows.System.VirtualKey.Number4:
                case Windows.System.VirtualKey.NumberPad4:
                    AddDigit(4); break;
                case Windows.System.VirtualKey.Number5:
                case Windows.System.VirtualKey.NumberPad5:
                    AddDigit(5); break;
                case Windows.System.VirtualKey.Number6:
                case Windows.System.VirtualKey.NumberPad6:
                    AddDigit(6); break;
                case Windows.System.VirtualKey.Number7:
                case Windows.System.VirtualKey.NumberPad7:
                    AddDigit(7); break;
                case Windows.System.VirtualKey.Number8:
                case Windows.System.VirtualKey.NumberPad8:
                    AddDigit(8); break;
                case Windows.System.VirtualKey.Number9:
                case Windows.System.VirtualKey.NumberPad9:
                    AddDigit(9); break;
                case Windows.System.VirtualKey.Delete:
                case Windows.System.VirtualKey.Back:
                    RemoveDigit();
                    break;
                case Windows.System.VirtualKey.Subtract:
                    if (AllowNegativeValues)
                        IsNegative = !IsNegative;
                    break;
            }

            e.Handled = true;
            RefreshText();
        }

        // Callbacks
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshText();
        }

        private static void OnNumberChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var src = source as NumberBox;

            double n = (double)e.NewValue;

            // Process negative values
            src.IsNegative = src.AllowNegativeValues && (n < 0);

            // Convert to list of char
            int decPlace = -1;
            List<char> work = new List<char>();

            string s = n.ToString();
            for (int i=0; i<s.Count(); i++)
            {
                if (s[i] == '.')
                    decPlace = i;
                if (char.IsDigit(s[i]))
                    work.Add(s[i]);
            }

            // Truncate to appropriate length
            if (decPlace == -1)
                decPlace = work.Count;
            int mantissaPlaces = work.Count - decPlace;

            if (mantissaPlaces > src.DecimalPlaces)
                work = work.Take(decPlace + src.DecimalPlaces).ToList();

            // Store
            src._digits.Clear();
            foreach (char c in work)
            { switch (c)
                { // I know we can subtract to get the number, but I prefer to be explicit in case things change. Switch statements are highly optimized anyway
                    case '0': src._digits.Add(0); break;
                    case '1': src._digits.Add(1); break;
                    case '2': src._digits.Add(2); break;
                    case '3': src._digits.Add(3); break;
                    case '4': src._digits.Add(4); break;
                    case '5': src._digits.Add(5); break;
                    case '6': src._digits.Add(6); break;
                    case '7': src._digits.Add(7); break;
                    case '8': src._digits.Add(8); break;
                    case '9': src._digits.Add(9); break;
                }
            }
            // Update display
            src.RefreshText();
        }

        // Internal
        List<byte> _digits = new List<byte>();

        private void AddDigit(byte Digit)
        {
            if ((Digit >= 0) && (Digit <= 9))
                _digits.Add(Digit);
        }
        private void RemoveDigit()
        {
            if (_digits.Count > 0)
                _digits.RemoveAt(_digits.Count - 1);
        }

        private void RefreshText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Prefix);

            if (IsNegative)
                sb.Append("-");

            var characteristic = _digits.Take(_digits.Count - DecimalPlaces).ToList();
            if (characteristic.Count == 0)
                sb.Append("0");
            else
            {
                for (int i=0; i<characteristic.Count; i++)
                {
                    sb.Append(characteristic[i].ToString());
                    if ((i > 1) && (i % 3 == 1))
                        sb.Append(NumberFormatInfo.CurrentInfo.NumberGroupSeparator);
                }
            }

            if (DecimalPlaces>0)
            {
                sb.Append(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                for (int i = _digits.Count - DecimalPlaces; i < _digits.Count; i++)
                    sb.Append(_digits[i].ToString());
            }

            sb.Append(Postfix);

            Text = sb.ToString();
        }
    }
}
