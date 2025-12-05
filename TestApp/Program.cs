using System.Text;
using TomiSoft.Printing.Thermal.EscPosFormatter;
using TomiSoft.Printing.Thermal.Imaging.ZXingBarcode;
using TomiSoft.Printing.Thermal.Printer.Netum;

internal class Program {
    private static void Main(string[] args) {
        string escPosXml = """
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
            """;

        using Stream s = new MemoryStream(Encoding.UTF8.GetBytes(escPosXml));

        var formatter = new XmlPosFormatter();
        var visitor = new EscPosXmlVisitor(new NT_1809DD()) {
            BarcodeImageProvider = new ZXingBarcodeImageProvider()
        };

        formatter.Format(s, visitor, options => {
            options.DefaultFont = "Font A";
            options.CodePage = "OEM852";
            options.LineFeedAtEnd = 3;
        });

        byte[] escPosData = visitor.Result;

        //open serial port targeting "COM8" with baud 11500 to transfer escPosData
        using var serialPort = new System.IO.Ports.SerialPort("COM8", 11500);
        serialPort.Open();
        serialPort.Write(escPosData, 0, escPosData.Length);
        serialPort.Close();
    }
}