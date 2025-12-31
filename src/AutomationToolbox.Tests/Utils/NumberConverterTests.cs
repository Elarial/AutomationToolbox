using Xunit;
using AutomationToolbox.Core.Utils;
using AutomationToolbox.Core.Models;
using System;

namespace AutomationToolbox.Tests.Utils
{
    public class NumberConverterTests : IDisposable
    {
        private readonly System.Globalization.CultureInfo _originalCulture;

        public NumberConverterTests()
        {
            _originalCulture = System.Globalization.CultureInfo.CurrentCulture;
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        }

        public void Dispose()
        {
            System.Globalization.CultureInfo.CurrentCulture = _originalCulture;
        }

        [Theory]
        [InlineData("10", ConversionFormat.Decimal, ConversionFormat.Binary, "1010")]
        [InlineData("10", ConversionFormat.Decimal, ConversionFormat.Hexadecimal, "A")]
        [InlineData("10", ConversionFormat.Decimal, ConversionFormat.Octal, "12")]
        [InlineData("1010", ConversionFormat.Binary, ConversionFormat.Decimal, "10")]
        [InlineData("A", ConversionFormat.Hexadecimal, ConversionFormat.Decimal, "10")]
        [InlineData("F", ConversionFormat.Hexadecimal, ConversionFormat.Binary, "1111")]
        public void Convert_StandardBases_ReturnsCorrectResult(string input, ConversionFormat fromFormat, ConversionFormat toFormat, string expected)
        {
            var result = NumberConverter.Convert(input, fromFormat, toFormat);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Convert_Ieee754_float_To_Hex_ReturnsCorrectBitRepresentation()
        {
            // 12.5 in IEEE 754 single precision:
            // Sign: 0, Exponent: 127 + 3 = 130 (10000010), Mantissa: 1.1001 -> 100100...
            // Binary: 0 10000010 10010000000000000000000
            // Hex: 41 48 00 00
            
            string input = "12.5";
            string expectedHex = "41480000";
            
            var result = NumberConverter.Convert(input, ConversionFormat.IEEE754, ConversionFormat.Hexadecimal);
            
            Assert.Equal(expectedHex, result);
        }

        [Fact]
        public void Convert_Ieee754_Hex_To_Float_ReturnsCorrectValue()
        {
            // 0x41480000 -> 12.5f
            string input = "41480000";
            string expectedFloat = "12.5";
            
            var result = NumberConverter.Convert(input, ConversionFormat.Hexadecimal, ConversionFormat.IEEE754);
            
            Assert.Equal(expectedFloat, result);
        }

        [Fact]
        public void Convert_Ieee754_NegativeFloat_To_Hex()
        {
            // -12.5 -> C1480000
            string input = "-12.5";
            string expectedHex = "C1480000";
            
            var result = NumberConverter.Convert(input, ConversionFormat.IEEE754, ConversionFormat.Hexadecimal);
            
            Assert.Equal(expectedHex, result);
        }

        [Fact]
        public void Convert_NullOrEmpty_ReturnsNull()
        {
            Assert.Null(NumberConverter.Convert(null, ConversionFormat.Decimal, ConversionFormat.Binary));
            Assert.Null(NumberConverter.Convert("", ConversionFormat.Decimal, ConversionFormat.Binary));
            Assert.Null(NumberConverter.Convert("  ", ConversionFormat.Decimal, ConversionFormat.Binary));
        }

        [Fact]
        public void Convert_InvalidInput_ReturnsNull()
        {
            // "Z" is not valid in base 10
            Assert.Null(NumberConverter.Convert("Z", ConversionFormat.Decimal, ConversionFormat.Binary));
        }

        [Fact]
        public void Convert_Overflow_ThrowsOverflowException()
        {
            // Number larger than long.MaxValue
            string hugeNumber = "9999999999999999999999999";
            
            Assert.Throws<OverflowException>(() => NumberConverter.Convert(hugeNumber, ConversionFormat.Decimal, ConversionFormat.Binary));
        }
        [Fact]
        public void Convert_Ieee754_WithCulture_RespectsDecimalSeparator()
        {
            // Store original culture
            var originalCulture = System.Globalization.CultureInfo.CurrentCulture;
            
            try
            {
                // Test French (Comma separator)
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
                
                // "12,5" -> Float 12.5 -> Hex 41480000
                string inputFr = "12,5"; 
                string resultFr = NumberConverter.Convert(inputFr, ConversionFormat.IEEE754, ConversionFormat.Hexadecimal);
                Assert.Equal("41480000", resultFr);
                
                // Hex 41480000 -> Float -> "12,5"
                string resultBackFr = NumberConverter.Convert("41480000", ConversionFormat.Hexadecimal, ConversionFormat.IEEE754);
                Assert.Equal("12,5", resultBackFr);

                // Test US (Dot separator)
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                
                // "12.5" -> Float 12.5 -> Hex 41480000
                string inputEn = "12.5";
                string resultEn = NumberConverter.Convert(inputEn, ConversionFormat.IEEE754, ConversionFormat.Hexadecimal);
                Assert.Equal("41480000", resultEn);
                
                 // Hex 41480000 -> Float -> "12.5"
                string resultBackEn = NumberConverter.Convert("41480000", ConversionFormat.Hexadecimal, ConversionFormat.IEEE754);
                Assert.Equal("12.5", resultBackEn);
            }
            finally
            {
                // Restore culture
                System.Globalization.CultureInfo.CurrentCulture = originalCulture;
            }
        }
    }
}
