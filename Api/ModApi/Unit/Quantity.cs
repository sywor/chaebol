using System;
using System.Collections.Generic;
using System.Linq;

namespace ModApi.Unit
{
	public enum BaseQuantity
	{
		Length,
		Mass,
		Time,
		ElectricCurrent,
		ThermodynamicTemperature,
		AmountOfSubstance,
		LuminousIntensity
	}

	public struct QuantityPower : IComparable<QuantityPower>
	{
		public readonly BaseQuantity BaseQuantity;
		public readonly int Exponent;

		public QuantityPower(BaseQuantity _baseQuantity, int _exponent)
		{
			BaseQuantity = _baseQuantity;
			Exponent = _exponent;
		}

		public int CompareTo(QuantityPower _other)
		{
			var baseQuantityComparison = BaseQuantity.CompareTo(_other.BaseQuantity);
			if (baseQuantityComparison != 0) return baseQuantityComparison;
			return Exponent.CompareTo(_other.Exponent);
		}
	}

	public class Quantity : IComparable<Quantity>
	{
		public IEnumerable<QuantityPower> QuantityPowers { get; }

		public Quantity()
		{
			QuantityPowers = Enumerable.Empty<QuantityPower>();
		}
		
		internal Quantity(IEnumerable<QuantityPower> _quantities)
		{
			QuantityPowers = _quantities.ToList();
		}

		public static implicit operator Quantity(BaseQuantity _base)
		{
			return new Quantity(new[] {new QuantityPower(_base, 1)});
		}

		public static Quantity operator *(Quantity _lhs, Quantity _rhs)
		{
			using (IEnumerator<QuantityPower> lhsEnumerator = _lhs.QuantityPowers.GetEnumerator(),
				rhsEnumerator = _rhs.QuantityPowers.GetEnumerator())
			{
				var lhsHasMore = lhsEnumerator.MoveNext();
				var rhsHasMore = rhsEnumerator.MoveNext();

				var resultQuatities = new List<QuantityPower>();

				while (lhsHasMore && rhsHasMore)
				{
					var lhsFront = lhsEnumerator.Current;
					var rhsFront = rhsEnumerator.Current;

					if (lhsFront.BaseQuantity == rhsFront.BaseQuantity)
					{
						var newExponent = lhsFront.Exponent + rhsFront.Exponent;
						if (newExponent != 0)
						{
							resultQuatities.Add(new QuantityPower(lhsFront.BaseQuantity, newExponent));
						}
						lhsHasMore = lhsEnumerator.MoveNext();
						rhsHasMore = rhsEnumerator.MoveNext();
					}
					else if (lhsFront.BaseQuantity < rhsFront.BaseQuantity)
					{
						resultQuatities.Add(lhsFront);
						lhsHasMore = lhsEnumerator.MoveNext();
					}
					else
					{
						resultQuatities.Add(rhsFront);
						rhsHasMore = rhsEnumerator.MoveNext();
					}
				}

				if (lhsHasMore)
				{
					resultQuatities.Add(lhsEnumerator.Current);
					while (lhsEnumerator.MoveNext())
					{
						resultQuatities.Add(lhsEnumerator.Current);
					}
				}
				else if (rhsHasMore)
				{
					resultQuatities.Add(rhsEnumerator.Current);
					while (rhsEnumerator.MoveNext())
					{
						resultQuatities.Add(rhsEnumerator.Current);
					}
				}

				return new Quantity(resultQuatities);
			}
		}

		public Quantity Invert()
		{
			var quantityPowers = QuantityPowers.Select(_power => new QuantityPower(_power.BaseQuantity, -_power.Exponent));
			return new Quantity(quantityPowers);
		}

		public int CompareTo(Quantity _other)
		{
			return SequenceCompare(QuantityPowers, _other.QuantityPowers);
		}

		private static int SequenceCompare<T>(IEnumerable<T> _source1, IEnumerable<T> _source2) where T : IComparable<T>
		{
			// You could add an overload with this as a parameter
			IComparer<T> elementComparer = Comparer<T>.Default;
			using (IEnumerator<T> iterator1 = _source1.GetEnumerator(), iterator2 = _source2.GetEnumerator())
			{
				while (true)
				{
					var next1 = iterator1.MoveNext();
					var next2 = iterator2.MoveNext();
					if (!next1 && !next2) // Both sequences finished
					{
						return 0;
					}
					if (!next1) // Only the first sequence has finished
					{
						return -1;
					}
					if (!next2) // Only the second sequence has finished
					{
						return 1;
					}
					// Both are still going, compare current elements
					var comparison = elementComparer.Compare(iterator1.Current,
						iterator2.Current);
					// If elements are non-equal, we're done
					if (comparison != 0)
					{
						return comparison;
					}
				}
			}
		}
	}
}