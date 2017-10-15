using System;
using System.Globalization;
using ModApi.Math;
using NUnit.Framework;

namespace ModApiTest.Math
{
	public class FixedTest
	{
		[Test]
		public void TestIntRoundTrip([Values(int.MinValue, -1, 0, 1, int.MinValue), Random(5)] int _i)
		{
			Assert.That((int)(Fixed) _i, Is.EqualTo(_i));
		}

		[Test]
		public void TestFloatRoundTrip([Values(0, 1, 1337.125f), Random(1E-2f, 1E9f, 5)] float _f, [Values] bool _positive)
		{
			var val = _positive ? _f : -_f;
			Assert.That((float)(Fixed) val, Is.EqualTo(val));
		}

		[Test]
		public void TestDoubleRoundTrip([Values(0, 1, 1337.125), Random(4194304D, 1073741824D, 5)] double _d, [Values] bool _positive)
		{
			var val = _positive ? _d : -_d;
			Assert.That((double)(Fixed) val, Is.EqualTo(val));
		}

		[Test]
		public void TestNegate([Values(0, 1, 42.125f), Random(0f, 1E9f, 5)] float _f)
		{
			var pos = (Fixed) _f;
			var neg = (Fixed) (-_f);

			Assert.That(-pos, Is.EqualTo(neg));
			Assert.That(-neg, Is.EqualTo(pos));
		}

		[Test]
		public void TestAdd(
			[Values(-100, 0, 1, 42.125f), Random(1f, 512f, 5)] float _lhs,
			[Values(-100, 0, 1, 42.125f), Random(1f, 512f, 5)] float _rhs)
		{
			Assert.That((Fixed) _lhs + _rhs, Is.EqualTo((Fixed) ((double) _lhs + _rhs)));
		}

		[Test]
		public void TestSubtract(
			[Values(-100, 0, 1, 42.125f), Random(1f, 512f, 5)] float _lhs,
			[Values(-100, 0, 1, 42.125f), Random(1f, 512f, 5)] float _rhs)
		{
			Assert.That((Fixed) _lhs - _rhs, Is.EqualTo((Fixed) ((double) _lhs - _rhs)));
		}

		[Test]
		public void TestMultiplyExact(
			[Values(0, 1, 2, -1, 10, 0.5f, -0.25f)] float _lhs,
			[Values(0, 1, 2, -1, 10, 0.5f, -0.25f)] float _rhs)
		{
			Fixed expected = _lhs * _rhs;

			Fixed res = (Fixed) _lhs * _rhs;
			Assert.That(res, Is.EqualTo(expected));
		}

		[Test]
		public void TestDivideExactSimple(
			[Values(0, 1, 2, -1, 10, 0.5f, -0.25f)] float _lhs,
			[Values(1, 2, -1, 16, 0.5f, -0.25f)] float _rhs)
		{
			Fixed expected = _lhs / _rhs;
			
			Fixed res = (Fixed) _lhs / _rhs;
			Assert.That(res, Is.EqualTo(expected));
		}

		[Test]
		public void TestDivideExactAdvanced(
			[Values(1.3125f, 1337, -0.9921875f, -2591f / 256f, 1, 0.5f)] float _divisor,
			[Values(0, 1.3125f, 1337, -0.9921875f, -2591f / 256f, 1, 0.5f)] float _quotient)
		{
			Fixed dividend = (Fixed) _divisor * _quotient;
			
			Assert.That(dividend / _divisor, Is.EqualTo((Fixed) _quotient));
		}

		[Test]
		public void TestWrappingUp()
		{
			Fixed before = Fixed.MaxValue;
			Fixed after = before + Fixed.Epsilon;

			Assert.That(after, Is.EqualTo(Fixed.MinValue));
		}

		[Test]
		public void TestWrappingDown()
		{
			Fixed before = Fixed.MinValue;
			Fixed after = before - Fixed.Epsilon;

			Assert.That(after, Is.EqualTo(Fixed.MaxValue));
		}

		[Test]
		public void TestToString()
		{
			AssertToString(Fixed.Zero, "0");
			AssertToString(Fixed.One, "1");
			Assert.That(Fixed.Pi.ToString(NumberFormatInfo.InvariantInfo), Does.StartWith("3.141592653"));
			
			AssertToString(10, "10");
			AssertToString(-1, "-1");
			AssertToString(42, "42");
			AssertToString(0.5, "0.5");
			AssertToString(0.1, "0.1000000000931322574615478515625");
			AssertToString(-0.1, "-0.1000000000931322574615478515625");
			
			AssertToString(Fixed.MaxValue, "2147483647.99999999976716935634613037109375");
			AssertToString(Fixed.MinValue, "-2147483648");
			AssertToString(Fixed.Epsilon, "0.00000000023283064365386962890625");
			AssertToString(-Fixed.Epsilon, "-0.00000000023283064365386962890625");
		}

		[Test]
		public void TestFloatUpperBoundary()
		{
			const int signBit = 1;
			const int bitsForWholeNumber = 64 - Fixed.BitsOfPrecision - signBit;
			const int floatPrecisionBits = 24;

			const float maxPosAcceptableFloat = ((1L << floatPrecisionBits) - 1) << (bitsForWholeNumber - floatPrecisionBits);
			const float minPosUnacceptableFloat = 1L << bitsForWholeNumber;

			AssertIsNeighbors(maxPosAcceptableFloat, minPosUnacceptableFloat);
			Assert.That(() => (Fixed) maxPosAcceptableFloat, Throws.Nothing);
			Assert.That(() => (Fixed) minPosUnacceptableFloat, Throws.InstanceOf<OverflowException>());
		}

		[Test]
		public void TestFloatLowerBoundary()
		{
			const int signBit = 1;
			const int bitsForWholeNumber = 64 - Fixed.BitsOfPrecision - signBit;
			const int floatPrecisionBits = 24;

			const float maxNegAcceptableFloat = -(float) (1L << bitsForWholeNumber);
			const float minNegUnacceptableFloat =
				-(float) ((1L << bitsForWholeNumber) + (1L << (bitsForWholeNumber - floatPrecisionBits + 1)));

			AssertIsNeighbors(maxNegAcceptableFloat, minNegUnacceptableFloat);
			Assert.That(() => (Fixed) maxNegAcceptableFloat, Throws.Nothing);
			Assert.That(() => (Fixed) minNegUnacceptableFloat, Throws.InstanceOf<OverflowException>());
		}

		[Test]
		public void TestSmallestFloatBoundary()
		{
			const int floatPrecisionBits = 24;

			const float smallestNonZeroFloat = 1.0f / (1L << (Fixed.BitsOfPrecision + 1)) +
			                                   1.0f / (1L << (Fixed.BitsOfPrecision + floatPrecisionBits));
			const float largestZeroFloat = 1.0f / (1L << (Fixed.BitsOfPrecision + 1));

			AssertIsNeighbors(smallestNonZeroFloat, largestZeroFloat);
			Assert.That((Fixed) smallestNonZeroFloat, Is.Not.EqualTo(Fixed.Zero));
			Assert.That((Fixed) largestZeroFloat, Is.EqualTo(Fixed.Zero));
			Assert.That((Fixed) (-smallestNonZeroFloat), Is.Not.EqualTo(Fixed.Zero));
			Assert.That((Fixed) (-largestZeroFloat), Is.EqualTo(Fixed.Zero));
		}

		[Test]
		public void TestDoubleUpperBoundary()
		{
			const int signBit = 1;
			const int bitsForWholeNumber = 64 - Fixed.BitsOfPrecision - signBit;
			const int doublePrecisionBits = 53;

			const double maxPosAcceptableDouble =
				(double) ((1L << doublePrecisionBits) - 1) / (1L << (doublePrecisionBits - bitsForWholeNumber));
			const double minPosUnacceptableDouble = 1L << bitsForWholeNumber;

			AssertIsNeighbors(maxPosAcceptableDouble, minPosUnacceptableDouble);
			Assert.That(() => (Fixed) maxPosAcceptableDouble, Throws.Nothing);
			Assert.That(() => (Fixed) minPosUnacceptableDouble, Throws.InstanceOf<OverflowException>());
		}

		[Test]
		public void TestDoubleLowerBoundary()
		{
			const int signBit = 1;
			const int bitsForWholeNumber = 64 - Fixed.BitsOfPrecision - signBit;
			const int doublePrecisionBits = 53;

			const double maxNegAcceptableDouble = -(double) (1L << bitsForWholeNumber);
			const double minNegUnacceptableDouble =
				-((1L << bitsForWholeNumber) + 1.0 / (1L << (doublePrecisionBits - bitsForWholeNumber - 1)));

			AssertIsNeighbors(maxNegAcceptableDouble, minNegUnacceptableDouble);
			Assert.That(() => (Fixed) maxNegAcceptableDouble, Throws.Nothing);
			Assert.That(() => (Fixed) minNegUnacceptableDouble, Throws.InstanceOf<OverflowException>());
		}

		[Test]
		public void TestSmallestDoubleBoundary()
		{
			const int doublePrecisionBits = 53;

			const double smallestNonZeroDouble = 1.0 / (1L << (Fixed.BitsOfPrecision + 1)) +
			                                     1.0 / (1L << Fixed.BitsOfPrecision) / (1L << doublePrecisionBits);
			const double largestZeroDouble = 1.0 / (1L << (Fixed.BitsOfPrecision + 1));

			AssertIsNeighbors(smallestNonZeroDouble, largestZeroDouble);
			Assert.That((Fixed) smallestNonZeroDouble, Is.Not.EqualTo(Fixed.Zero));
			Assert.That((Fixed) largestZeroDouble, Is.EqualTo(Fixed.Zero));
			Assert.That((Fixed) (-smallestNonZeroDouble), Is.Not.EqualTo(Fixed.Zero));
			Assert.That((Fixed) (-largestZeroDouble), Is.EqualTo(Fixed.Zero));
		}

		private static void AssertToString(Fixed _f, string _expectedInvariant)
		{
			var sep = CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
			
			Assert.That(_f.ToString(NumberFormatInfo.InvariantInfo), Is.EqualTo(_expectedInvariant));
			Assert.That(_f.ToString(), Is.EqualTo(_expectedInvariant.Replace(".", sep)));
		}

		private static void AssertIsNeighbors(float _x, float _y)
		{
			var diff = _x - _y;
			var halfDiff = diff / 2;
			Assert.That(_y + halfDiff, Is.EqualTo(_x).Or.EqualTo(_y));
		}

		private static void AssertIsNeighbors(double _x, double _y)
		{
			var diff = _x - _y;
			var halfDiff = diff / 2;
			Assert.That(_y + halfDiff, Is.EqualTo(_x).Or.EqualTo(_y));
		}
	}
}