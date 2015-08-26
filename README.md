# NumberBox
A calculator-style number box for Windows Desktop/Phone 8.1 and later.

The control only allows digits, and places the decimal appropriately (based on the DecimalPlaces property).

So, why should you use this?

Because it's an easy way to enter numbers within a TextBox. It only allows digits to be entered, it maintains the property number of significant digits after the decimal place, and you can block negative numbers if desired.


Use is simple. In your Xaml, add the correct namespace:
```
xmlns:nb="using:NumberBox"
```

Then, the control itself:
```
<nb:NumberBox
    Prefix="$"
    Postfix=" Dollars Spent"
    DecimalPlaces="2"
    AllowNegatives="false"
/>

<nb:NumberBox
    Prefix=""
    Postfix=""
    DecimalPlaces="0"
    AllowNegatives="true"
/>
```

There are two properties you may access to get the contents:

NumberBox.Text is a string representation of the number, including the prefix, postfix, and formatting (using the localized
digit grouping separater and decimal separator). While you may bind to this value, any updates to it are ignored and immediately restored to the proper representation.

NumberBox.Number is a double representing the number itself. Binding to this value works as expected, both ways.
