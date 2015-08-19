# NumberBox
A calculator-style number box for Windows Desktop/Phone 8.1 and later.

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
digit grouping separater and decimal separator).

NumberBox.Number is a double representing the number itself.
