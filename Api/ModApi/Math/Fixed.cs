using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace ModApi.Math
{
	/// <summary>
	/// Implements Q31.32
	/// </summary>
	public struct Fixed
	{
		public const int BitsOfPrecision = 32;
		public static readonly Fixed MaxValue = new Fixed(long.MaxValue);
		public static readonly Fixed MinValue = new Fixed(long.MinValue);
		public static readonly Fixed Epsilon = new Fixed(1);
		public static readonly Fixed Zero = new Fixed(0);
		public static readonly Fixed One = new Fixed(Multiplier);
		public static readonly Fixed Pi = System.Math.PI;

		private const long Multiplier = 1L << BitsOfPrecision;
		private const long DecimalMask = Multiplier - 1;

		private readonly long rawValue;

		private Fixed(long _rawValue)
		{
			rawValue = _rawValue;
		}

		public static implicit operator Fixed(int _i)
		{
			var adjusted = (long) _i << BitsOfPrecision;
			return new Fixed(adjusted);
		}

		public static explicit operator int(Fixed _d)
		{
			return (int) (_d.rawValue >> BitsOfPrecision);
		}

		public static implicit operator Fixed(float _f)
		{
			var adjusted = _f * Multiplier;
			return new Fixed(Convert.ToInt64(adjusted));
		}

		public static explicit operator float(Fixed _f)
		{
			double asDouble = _f.rawValue;
			return (float) (asDouble / Multiplier);
		}

		public static implicit operator Fixed(double _d)
		{
			var adjusted = _d * Multiplier;
			return new Fixed(RoundEven(adjusted));
		}

		public static explicit operator double(Fixed _f)
		{
			double asDouble = _f.rawValue;
			return asDouble / Multiplier;
		}

		public static Fixed operator -(Fixed _op)
		{
			return new Fixed(-_op.rawValue);
		}

		public static Fixed operator +(Fixed _lhs, Fixed _rhs)
		{
			return new Fixed(_lhs.rawValue + _rhs.rawValue);
		}

		public static Fixed operator -(Fixed _lhs, Fixed _rhs)
		{
			return new Fixed(_lhs.rawValue - _rhs.rawValue);
		}

		public static Fixed operator *(Fixed _lhs, Fixed _rhs)
		{
			var upperL = (ulong) (_lhs.rawValue >> BitsOfPrecision);
			var lowerL = (ulong) (_lhs.rawValue & DecimalMask);
			var upperR = (ulong) (_rhs.rawValue >> BitsOfPrecision);
			var lowerR = (ulong) _rhs.rawValue & DecimalMask;

			var uLuR = upperL * upperR;
			var uLlR = upperL * lowerR;
			var lLuR = lowerL * upperR;
			var lLlR = lowerL * lowerR;

			var lowerRes = uLlR + lLuR + (lLlR >> BitsOfPrecision);
			var result = lowerRes + (uLuR << BitsOfPrecision);

			return new Fixed((long) result);
		}

		public static Fixed operator /(Fixed _lhs, Fixed _rhs)
		{
			var signedUpperR = _rhs.rawValue >> BitsOfPrecision;
			var signedLowerR = _rhs.rawValue & DecimalMask;

			if (signedLowerR == 0)
			{
				return new Fixed(_lhs.rawValue / signedUpperR);
			}

			var signL = System.Math.Sign(_lhs.rawValue);
			var magL = signL < 0 ? (ulong) (-_lhs.rawValue) : (ulong) _lhs.rawValue;
			var signR = System.Math.Sign(_rhs.rawValue);
			var magR = signR < 0 ? (ulong) (-_rhs.rawValue) : (ulong) _rhs.rawValue;

			var upperL = magL >> BitsOfPrecision;
			var lowerL = magL & DecimalMask;

			if (upperL == 0)
			{
				return new Fixed(signL * signR * (long) ((lowerL << BitsOfPrecision) / magR));
			}

			var upperR = magR >> BitsOfPrecision;
			var lowerR = magR & DecimalMask;

			if (upperR == 0)
			{
				var upperLRemainder = upperL % lowerR;
				var reassembledL1 = (upperLRemainder << BitsOfPrecision) | lowerL;
				var upperRes = reassembledL1 / lowerR;
				var lowerLRemainder = reassembledL1 % lowerR;
				var lowerRes = (lowerLRemainder << BitsOfPrecision) / lowerR;
				
				return new Fixed(signL * signR * (long) ((upperRes << BitsOfPrecision) | lowerRes));
			}

			var upperRes2 = magL / magR;
			var remainder = magL % magR;
			var upperRRoundedUp = upperR + 1;
			var lowerResLow = remainder / upperRRoundedUp;
			var lowerRemainderLow = remainder % upperRRoundedUp;
			var error = lowerResLow * ((upperRRoundedUp << BitsOfPrecision) - magR); 
			var lowerRemainder = (lowerRemainderLow << BitsOfPrecision) + error;
			var lowerResCorrected = lowerResLow + lowerRemainder / magR;
			
			return new Fixed(signL * signR * (long) ((upperRes2 << BitsOfPrecision) | lowerResCorrected));
		}

		public override string ToString()
		{
			return ToString(NumberFormatInfo.CurrentInfo);
		}

		[Pure]
		public string ToString(IFormatProvider _provider)
		{
			var wholeNumber = rawValue >> BitsOfPrecision;
			var decimals = rawValue & DecimalMask;

			if (decimals == 0)
			{
				return wholeNumber.ToString(_provider);
			}

			var numberFormatInfo = NumberFormatInfo.GetInstance(_provider);

			StringBuilder sb = new StringBuilder(12 + BitsOfPrecision);

			if (wholeNumber < 0)
			{
				decimals = Multiplier - decimals;
				wholeNumber += 1;
				if (wholeNumber == 0)
				{
					sb.Append(numberFormatInfo.NegativeSign);
				}
			}

			sb.Append(wholeNumber.ToString(_provider));
			sb.Append(numberFormatInfo.NumberDecimalSeparator);

			while (decimals != 0)
			{
				decimals *= 10;
				var digit = decimals / Multiplier;
				decimals -= digit * Multiplier;

				sb.Append(digit);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Rounds to the nearest integer value. If exactly in between, rounds to the even one.
		/// </summary>
		/// Needed because <see cref="Math.Round(double)"/> fails at specific values, such as 0.500000000000001D
		/// (incorrectly rounded to 0).
		/// 
		/// <param name="_d">The value to round</param>
		/// <returns>The nearest integer</returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		private static long RoundEven(double _d)
		{
			var intPart = System.Math.Truncate(_d);
			if (intPart > ((double) ((1L << 53) - 1) * (1L << 10)) || intPart < -((double) (1L << 53) * (1L << 10)))
			{
				throw new OverflowException();
			}

			var fracPart = _d - intPart;
			var longIntPart = (long) intPart;
			if (fracPart < -0.5 || fracPart == -0.5D && (longIntPart & 1) == 1)
			{
				return longIntPart - 1;
			}
			if (fracPart > 0.5 || fracPart == 0.5D && (longIntPart & 1) == 1)
			{
				return longIntPart + 1;
			}
			return longIntPart;
		}
	}
}