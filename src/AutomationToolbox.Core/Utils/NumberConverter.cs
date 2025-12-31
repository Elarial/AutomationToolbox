using System;

using AutomationToolbox.Core.Models;

namespace AutomationToolbox.Core.Utils
{
    /// <summary>
    /// Provides methods for converting numbers between different bases.
    /// </summary>
    public static class NumberConverter
    {

        /// <summary>
        /// Converts a string representation of a number from a source format to a target format.
        /// </summary>
        public static string? Convert(string input, ConversionFormat fromFormat, ConversionFormat toFormat)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            try
            {
                // standardize input
                input = input.Trim().ToUpperInvariant();

                // IEEE 754 Float Logic
                if (fromFormat == ConversionFormat.IEEE754)
                {
                    // Input is Decimal Float String (e.g. "12.5" or "12,5")
                    // Use CurrentCulture to respect the user's decimal separator preference
                    if (!float.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out float floatVal))
                    {
                        return null; 
                    }

                    if (toFormat == ConversionFormat.IEEE754) return input;

                    // Convert Float to Int Bits
                    int bits = BitConverter.SingleToInt32Bits(floatVal);
                    
                    if (toFormat == ConversionFormat.Decimal) 
                    {
                        // Output integer representation in invariant culture (it's just an int bits value, typically viewed as language-agnostic numbers)
                        // BUT user might expect grouping separators? Usually tools output raw "123456". Keep Invariant for Integer Bits avoids confusion.
                        // However, the request is about INPUT separator.
                        // The output conversion for IEEE754 -> Decimal (e.g. 1095237632) is an INTEGER.
                        return bits.ToString(System.Globalization.CultureInfo.InvariantCulture); 
                    }
                    
                    // For Hex/Bin/Octal, use unsigned 32-bit view
                    uint uBits = (uint)bits;
                    string res = System.Convert.ToString(uBits, (int)toFormat);
                    if (toFormat == ConversionFormat.Hexadecimal) res = res.ToUpperInvariant();
                    return res;
                }

                if (toFormat == ConversionFormat.IEEE754)
                {
                    // Convert from Integer Base to Float Value
                    long val = System.Convert.ToInt64(input, (int)fromFormat);
                    
                    // Truncate to 32 bits and reinterpret
                    int intBits = (int)val;
                    float floatVal = BitConverter.Int32BitsToSingle(intBits);
                    
                    // Output float as string using CurrentCulture (so it shows ',' for FR)
                    return floatVal.ToString(System.Globalization.CultureInfo.CurrentCulture);
                }

                // Standard Integer Conversion
                long value = System.Convert.ToInt64(input, (int)fromFormat);
                string result = System.Convert.ToString(value, (int)toFormat);
                if (toFormat == ConversionFormat.Hexadecimal) result = result.ToUpperInvariant();
                return result;
            }
            catch (OverflowException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }
    }
}
