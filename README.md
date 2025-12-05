
# TomiSoft.Printing.Thermal
This library lets you define what you want to print in a nice XML format using an ESC/POS compatible thermal printer.

## Usage
Define the document to print using XML:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<escpos version="1.0.0">
  <align to="center">
    <para>
      <heading level="2">My Sample Shop</heading>
      <t>12651 Tamiami Trail E, Naples</t>
    </para>

    <para>
      <t>Joe Owner Inc.</t>
      <t>Tax ID: 582128316</t>
    </para>

    <para>
      <t>With this coupon you are eligible to a</t>
      <heading level="2">10%</heading>
      <t>discount on your next visit at our store.</t>
    </para>

    <qr size="300" asimage="true">BS5EWG</qr>
    <heading level="2">BS5EWG</heading>
  </align>

  <para>
    <table>
      <row>
        <column>Issued:</column>
        <column>15-02-2024</column>
      </row>

      <row>
        <column>Expires:</column>
        <column>20-02-2024</column>
      </row>
    </table>
  </para>

  <align to="center">
    <para>
      <t>
	    Eligibility for discount. This coupon can be redeemed
	    one time within its validity period in our shop by
	    presenting the original copy. Only one coupon can be
	    redeemed at one purchase for any of our products, without
	    minimum purchase. Cannot be replaced in case of damage or
	    loss of this paper.
      </t>
    </para>

    <para>
      <t>
	    Protect from heat, sunshine moisture and tearing!
	    Ensure the readibility of the QR code above.
      </t>
    </para>

    <para>
      <t>We're looking forward to seeing you again.</t>
    </para>
  </align>
</escpos>
```

Then create your ESC/POS byte array with the following code:
```csharp
string escPosXml = "...";

using Stream s = new MemoryStream(Encoding.UTF8.GetBytes(escPosXml));

//IPrinter provides parameters about a specific thermal printer,
//eg. code page and font support, paper width, etc.
//This is used for word wrapping, encoding text in printer-specific codepage, etc.
//You may need to implement your own.
IPrinter printerDriver = new NT_1809DD();

var formatter = new XmlPosFormatter();
var visitor = new EscPosXmlVisitor(printerDriver) {
    //imaging dependencies are isolated and alternative implementations
    //can be provided
    BarcodeImageProvider = new ZXingBarcodeImageProvider()
};

formatter.Format(s, visitor, options => {
    //provide default settings specific to the printer you use
    options.DefaultFont = "Font A";
    options.CodePage = "OEM852";
    options.LineFeedAtEnd = 3;
});

byte[] escPosData = visitor.Result;

//Print with your favourite library:
await printer.Print(escPosData);

//Or if your printer is on a serial port:
using var serialPort = new System.IO.Ports.SerialPort("COM8", 11500);
serialPort.Open();
serialPort.Write(escPosData, 0, escPosData.Length);
serialPort.Close();
```
