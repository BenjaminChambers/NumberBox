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
            RefreshNumber();
        }

        // Public Dependency Properties
        public double Number {
            get { return (double)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public bool IsNegative { get { return (bool)GetValue(IsNegativeProperty); } set { SetValue(IsNegativeProperty, value); RefreshText(); RefreshNumber(); } }

        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set
            {
                if (value >= 0)
                {
                    SetValue(DecimalPlacesProperty, value);

                    while (_mantissa.Count > DecimalPlaces)
                        _mantissa.RemoveAt(_mantissa.Count - 1);

                    while (_mantissa.Count < DecimalPlaces)
                        _mantissa.Add(0);
                }
                else
                {
                    SetValue(DecimalPlacesProperty, 0);
                    _mantissa.Clear();
                }

                RefreshText();
                RefreshNumber();
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
            RefreshText();
            RefreshNumber();
        }

        // Callbacks
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_processing)
            {
                _processing = true;
                ReadString(Text);
                RefreshText();
                RefreshNumber();
                _processing = false;
            }
        }

        private static void OnNumberChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var src = source as NumberBox;

            if (!src._processing)
            {
                src._processing = true;
                src.ReadString(((double)e.NewValue).ToString());
                src.RefreshText();
                src._processing = false;
            }
        }

        // Internal
        List<int> _digits = new List<int>();
        List<int> _characteristic = new List<int>();
        List<int> _mantissa = new List<int>();

        private bool _processing = false;

        private void RefreshText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Prefix);

            for (int i=0; i<_characteristic.Count; i++)
            {
                sb.Append(_characteristic[i].ToString());
                if ((i%3==1)&&(i>1))
                    sb.Append(NumberFormatInfo.CurrentInfo.NumberGroupSeparator);
            }

            sb.Append(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);

            foreach (int num in _mantissa)
                sb.Append(num.ToString());

            sb.Append(Postfix);

            Text = sb.ToString();
        }

        private void RefreshNumber()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("0");
            foreach (int num in _characteristic)
                sb.Append(num);
            sb.Append(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
            foreach (int num in _mantissa)
                sb.Append(num);
            sb.Append("0");

            string temp = sb.ToString();
            Number = double.Parse(temp);
        }

        private void ReadString(string value)
        {
            bool neg = false;

            if (AllowNegativeValues)
            {
                foreach (char c in value)
                    neg = !neg;
            }

            int place = 0;
            _characteristic.Clear();
            _mantissa.Clear();
            while ((place < value.Count()) && (value[place] != '.'))
            {
                if (char.IsDigit(value[place]))
                    _characteristic.Add(int.Parse(value[place].ToString()));
                place++;
            }
            while (place < value.Count())
            {
                if (char.IsDigit(value[place]))
                    _mantissa.Add(int.Parse(value[place].ToString()));
                place++;
            }
            while (_mantissa.Count < DecimalPlaces)
            {
                _mantissa.Add(0);
            }
            while (_mantissa.Count > DecimalPlaces)
            {
                _mantissa.RemoveAt(_mantissa.Count - 1);
            }
        }
    }
}
