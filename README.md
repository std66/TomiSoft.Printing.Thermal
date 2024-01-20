# TomiSoft.Printing.Thermal
This library lets you define what you want to print in a nice XML format.

## Usage
Define the document to print using XML:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<escpos version="1.0.0" paperWidthMM="57" lineWidthChars="32" lineFeedAtEnd="8">
  <align to="center">
    <para>
      <heading level="7">My Sample Shop</heading>
      <t>12651 Tamiami Trail E, Naples</t>
    </para>

    <para>
      <t>Joe Owner Inc.</t>
      <t>Tax ID: 582128316</t>
    </para>

    <para>
      <t>With this coupon you are</t>
      <t>eligible to a</t>
      <heading level="3">10%</heading>
      <t>discount on your next</t>
      <t>visit at our store.</t>
    </para>

    <para>
      <qr size="300" asimage="true">BS5EWG</qr>
      <heading level="3">BS5EWG</heading>
    </para>
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
      <t>Eligibility for discount.</t>
      <t>This coupon can be redeemed</t>
      <t>one time within its validity</t>
      <t>period in our shop by</t>
      <t>presenting the original copy.</t>
      <t>Only one coupon can be</t>
      <t>redeemed at one purchase for</t>
      <t>any of our products, without</t>
      <t>minimum purchase. Cannot be</t>
      <t>replaced in case of damage or</t>
      <t>loss of this paper.</t>
    </para>

    <para>
      <t>Protect from heat, sunshine</t>
      <t>moisture and tearing!</t>
      <t>Ensure the readibility of</t>
      <t>the QR code above.</t>
    </para>

    <para>
      <t>We're looking forward to</t>
      <t>seeing you again.</t>
    </para>
  </align>
</escpos>
```

Then create your ESC/POS byte array with the following code:
```csharp
string escPosXml = "...";

using Stream s = new MemoryStream(Encoding.UTF8.GetBytes(escPosXml));

var formatter = new XmlPosFormatter();
var visitor = new XmlVisitor();

formatter.Format(s, visitor);
byte[] escPosData = visitor.Result;

//Print with your favourite library
await printer.Print(escPosData);
```
