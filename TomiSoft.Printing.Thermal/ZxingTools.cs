using System;
using System.Collections.Generic;
using TomiSoft.Printing.Thermal.EscPosFormatter;
using ZXing;
using ZXing.Common;
using ZXing.OneD;
using ZXing.QrCode;

namespace TomiSoft.Printing.Thermal {
    public static class ZxingTools {
        public static readonly byte GS = 0x1D;// Group separator

        public static byte[] GetQRCodeImage(string value, int size) {
            QRCodeWriter qrCodeWriter = new QRCodeWriter();
            BitMatrix byteMatrix = qrCodeWriter.encode(value, BarcodeFormat.QR_CODE, 100, 100);

            return BitMatrixToEscPos(byteMatrix, size);
        }

        public static byte[] GetBarcodeImage(BarcodeKind kind, string value, int size) {
            ZXing.BarcodeFormat format = kind switch {
                BarcodeKind.UPC_A => BarcodeFormat.UPC_A,
                BarcodeKind.UPC_E => BarcodeFormat.UPC_E,
                BarcodeKind.EAN13 => BarcodeFormat.EAN_13,
                BarcodeKind.EAN8 => BarcodeFormat.EAN_8,
                BarcodeKind.CODE39 => BarcodeFormat.CODE_39,
                BarcodeKind.ITF => BarcodeFormat.ITF,
                BarcodeKind.CODABAR => BarcodeFormat.CODABAR,
                BarcodeKind.CODE93 => BarcodeFormat.CODE_93,
                BarcodeKind.CODE128 => BarcodeFormat.CODE_128,
                _ => throw new ArgumentException($"Parameter has invalid value: '{kind}'", nameof(kind))
            };


            Writer writer = kind switch {
                BarcodeKind.UPC_A => new UPCAWriter(),
                BarcodeKind.UPC_E => new UPCEWriter(),
                BarcodeKind.EAN13 => new EAN13Writer(),
                BarcodeKind.EAN8 => new EAN8Writer(),
                BarcodeKind.CODE39 => new Code39Writer(),
                BarcodeKind.ITF => new ITFWriter(),
                BarcodeKind.CODABAR => new CodaBarWriter(),
                BarcodeKind.CODE93 => new Code93Writer(),
                BarcodeKind.CODE128 => new Code128Writer(),
                _ => throw new ArgumentException($"Parameter has invalid value: '{kind}'", nameof(kind))
            };

            BitMatrix byteMatrix = writer.encode(value, format, 100, 100);

            return BitMatrixToEscPos(byteMatrix, size);
        }

        public static byte[] BitMatrixToEscPos(BitMatrix byteMatrix, int size) {
            List<byte> result = new List<byte>() { };

            int width = byteMatrix.Width;
            int height = byteMatrix.Height;
            int coefficient = (int)Math.Round((float)size / (float)width);
            int imageWidth = width * coefficient;
            int imageHeight = height * coefficient;
            int bytesByLine = (int)Math.Ceiling(((float)imageWidth) / 8f);
            int i = 8;

            if (coefficient < 1) {
                return InitGSv0Command(0, 0);
            }

            byte[] imageBytes = InitGSv0Command(bytesByLine, imageHeight);

            for (int y = 0; y < height; y++) {
                byte[] lineBytes = new byte[bytesByLine];
                int x = -1, multipleX = coefficient;
                bool isBlack = false;
                for (int j = 0; j < bytesByLine; j++) {
                    int b = 0;
                    for (int k = 0; k < 8; k++) {
                        if (multipleX == coefficient) {
                            isBlack = ++x < width && byteMatrix[x, y];
                            multipleX = 0;
                        }
                        if (isBlack) {
                            b |= 1 << (7 - k);
                        }
                        ++multipleX;
                    }
                    lineBytes[j] = (byte)b;
                }

                for (int multipleY = 0; multipleY < coefficient; ++multipleY) {
                    Array.Copy(lineBytes, 0, imageBytes, i, lineBytes.Length);
                    i += lineBytes.Length;
                }
            }


            result.AddRange(imageBytes);

            return result.ToArray();
        }

        public static byte[] InitGSv0Command(int bytesByLine, int bitmapHeight) {
            int
                xH = bytesByLine / 256,
                xL = bytesByLine - (xH * 256),
                yH = bitmapHeight / 256,
                yL = bitmapHeight - (yH * 256);

            byte[] imageBytes = new byte[8 + bytesByLine * bitmapHeight];
            imageBytes[0] = 0x1D;
            imageBytes[1] = 0x76;
            imageBytes[2] = 0x30;
            imageBytes[3] = 0x00;
            imageBytes[4] = (byte)xL;
            imageBytes[5] = (byte)xH;
            imageBytes[6] = (byte)yL;
            imageBytes[7] = (byte)yH;
            return imageBytes;
        }
    }
}
