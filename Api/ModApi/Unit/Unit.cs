using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModApi.Math;

namespace ModApi.Unit
{
	public interface IUnit
	{
		Quantity Quantity { get; }
		string Symbol { get; }
		string PublicKey { get; }
		Fixed Ratio { get; }
		IEnumerable<SingleUnitPower> UnitPowers { get; }
		IBase Base { get; }
		IUnit Invert();
	}

	public interface ISingle : IUnit
	{
		BaseQuantity BaseQuantity { get; }
		ISingleBase SingleBase { get; }
	}

	public interface IBase : IUnit
	{
		IBase InvertBase();
	}

	public interface ISingleBase : ISingle, IBase
	{
	}

	public class SingleUnitPower
	{
		public readonly ISingle Unit;
		public readonly int Exponent;

		public SingleUnitPower(ISingle _unit, int _exponent)
		{
			Unit = _unit;
			Exponent = _exponent;
		}

//		public int CompareTo(UnitPower _other)
//		{
//			var unitComparison = Unit.CompareTo(_other.Unit);
//			if (unitComparison != 0) return unitComparison;
//			return Exponent.CompareTo(_other.Exponent);
//		}

		public override string ToString()
		{
			return Exponent == 1 ? Unit.Symbol : $"{Unit.Symbol}^{Exponent}";
		}

		public string GetPublicKey()
		{
			return Exponent == 1 ? Unit.PublicKey : $"{Unit.PublicKey}^{Exponent}";
		}
	}

	public class BaseSingleUnitPower : SingleUnitPower
	{
		public readonly ISingleBase BaseUnit;

		public BaseSingleUnitPower(ISingleBase _baseUnit, int _exponent) : base(_baseUnit, _exponent)
		{
			BaseUnit = _baseUnit;
		}
	}

	public abstract class Unit : IComparable<Unit>, IUnit
	{
		public Quantity Quantity { get; }
		public string Symbol { get; }
		public string PublicKey { get; }
		public Fixed Ratio { get; }
		public abstract IEnumerable<SingleUnitPower> UnitPowers { get; }
		public abstract IBase Base { get; }

		public Unit(Quantity _quantity, string _symbol, string _publicKey, Fixed _ratio)
		{
			Quantity = _quantity;
			Symbol = _symbol;
			PublicKey = _publicKey;
			Ratio = _ratio;
		}

		protected abstract Fixed ConvertToBaseUnit(Fixed _valueInCurrentUnit);
		protected abstract Fixed ConvertFromBaseUnit(Fixed _valueInBaseUnit);
		public abstract IUnit Invert();

		public static Unit operator *(Unit _lhs, IUnit _rhs)
		{
			using (IEnumerator<SingleUnitPower> lhsEnumerator = _lhs.UnitPowers.GetEnumerator(),
				rhsEnumerator = _rhs.UnitPowers.GetEnumerator())
			{
				var lhsHasMore = lhsEnumerator.MoveNext();
				var rhsHasMore = rhsEnumerator.MoveNext();

				var resultUnits = new List<SingleUnitPower>();

				while (lhsHasMore && rhsHasMore)
				{
					var lhsFront = lhsEnumerator.Current;
					var rhsFront = rhsEnumerator.Current;

					// ReSharper disable once PossibleNullReferenceException
					var unitComparison = lhsFront.Unit.Quantity.CompareTo(
						// ReSharper disable once PossibleNullReferenceException
						rhsFront.Unit.Quantity);
					if (unitComparison == 0)
					{
						var newExponent = lhsFront.Exponent + rhsFront.Exponent;
						if (newExponent != 0)
						{
							resultUnits.Add(new SingleUnitPower(lhsFront.Unit, newExponent));
						}
						lhsHasMore = lhsEnumerator.MoveNext();
						rhsHasMore = rhsEnumerator.MoveNext();
					}
					else if (unitComparison < 0)
					{
						resultUnits.Add(lhsFront);
						lhsHasMore = lhsEnumerator.MoveNext();
					}
					else
					{
						resultUnits.Add(rhsFront);
						rhsHasMore = rhsEnumerator.MoveNext();
					}
				}

				if (lhsHasMore)
				{
					resultUnits.Add(lhsEnumerator.Current);
					while (lhsEnumerator.MoveNext())
					{
						resultUnits.Add(lhsEnumerator.Current);
					}
				}
				else if (rhsHasMore)
				{
					resultUnits.Add(rhsEnumerator.Current);
					while (rhsEnumerator.MoveNext())
					{
						resultUnits.Add(rhsEnumerator.Current);
					}
				}

				var compoundQuantity = CreateQuantity(resultUnits);
				var compoundSymbol = CreateSymbol(resultUnits);
				var compoundPublicKey = CreatePublicKey(resultUnits);
				var baseUnits = GetBaseUnits(resultUnits);
				var baseUnit = CreateBaseUnit(baseUnits, compoundQuantity, compoundSymbol, compoundPublicKey);
				var compoundRatio = GetRatio(resultUnits);
				return new CompoundProportionalUnit(baseUnit,
					resultUnits,
					compoundQuantity,
					compoundSymbol,
					compoundPublicKey,
					compoundRatio);
			}
		}

		public static Unit operator /(Unit _lhs, Unit _rhs)
		{
			return _lhs * _rhs.Invert();
		}

		private static Fixed GetRatio(IEnumerable<SingleUnitPower> _units)
		{
			Fixed ratio = 1;
			foreach (var unit in _units)
			{
				if (unit.Exponent > 0)
				{
					for (var i = 0; i < unit.Exponent; i++)
					{
						ratio *= unit.Unit.Ratio;
					}
				}
				else
				{
					Fixed divisor = 1;
					for (var i = unit.Exponent; i < 0; i++)
					{
						divisor *= unit.Unit.Ratio;
					}
					ratio /= divisor;
				}
			}
			return ratio;
		}

		public int CompareTo(Unit _other)
		{
			return string.Compare(Symbol, _other.Symbol, StringComparison.Ordinal);
		}

		private static Quantity CreateQuantity(IEnumerable<SingleUnitPower> _units)
		{
			return new Quantity(_units.Select(_power => new QuantityPower(_power.Unit.BaseQuantity, _power.Exponent)));
		}

		protected static string CreateSymbol(IEnumerable<SingleUnitPower> _units)
		{
			return string.Join("*", _units);
		}

		protected static string CreatePublicKey(IEnumerable<SingleUnitPower> _units)
		{
			return _units
				.Aggregate(new StringBuilder(),
					(_builder, _power) =>
					{
						if (_builder.Length > 0)
						{
							_builder.Append("*");
						}
						return _builder.Append(_power.GetPublicKey());
					})
				.ToString();
		}

		private static IEnumerable<BaseSingleUnitPower> GetBaseUnits(IEnumerable<SingleUnitPower> _units)
		{
			return _units.Select(_power => new BaseSingleUnitPower(_power.Unit.SingleBase, _power.Exponent));
		}

		private static BaseUnit CreateBaseUnit(IEnumerable<BaseSingleUnitPower> _units,
			Quantity _quantity,
			string _symbol,
			string _publicKey)
		{
			return new CompoundBaseUnit(_units, _quantity, _symbol, _publicKey);
		}

		public Fixed ConvertTo(Fixed _value, Unit _targetUnit)
		{
			if (Quantity.CompareTo(_targetUnit.Quantity) != 0)
			{
				throw new ArgumentException("Incompatible units: " + ToString() + ", " + _targetUnit);
			}
			return _targetUnit.ConvertFromBaseUnit(ConvertToBaseUnit(_value));
		}
	}

	public abstract class BaseUnit : Unit, IBase
	{
		public override IBase Base => this;

		public BaseUnit(Quantity _quantity, string _symbol, string _publicKey) : base(_quantity,
			_symbol,
			_publicKey,
			1)
		{
		}

		protected override Fixed ConvertToBaseUnit(Fixed _valueInCurrentUnit)
		{
			return _valueInCurrentUnit;
		}

		protected override Fixed ConvertFromBaseUnit(Fixed _valueInBaseUnit)
		{
			return _valueInBaseUnit;
		}

		public abstract IBase InvertBase();
	}

	public class SingleBaseUnit : BaseUnit, IComparable<SingleBaseUnit>, ISingleBase
	{
		public static readonly SingleBaseUnit Metre = new SingleBaseUnit(BaseQuantity.Length, "m", "metre");
		public static readonly SingleBaseUnit Kilogram = new SingleBaseUnit(BaseQuantity.Mass, "kg", "kilogram");
		public static readonly SingleBaseUnit Second = new SingleBaseUnit(BaseQuantity.Time, "s", "second");
		public static readonly SingleBaseUnit Ampere = new SingleBaseUnit(BaseQuantity.ElectricCurrent, "A", "ampere");

		public static readonly SingleBaseUnit Kelvin =
			new SingleBaseUnit(BaseQuantity.ThermodynamicTemperature, "K", "kelven");

		public static readonly SingleBaseUnit Mole = new SingleBaseUnit(BaseQuantity.AmountOfSubstance, "mol", "mole");
		public static readonly SingleBaseUnit Candela = new SingleBaseUnit(BaseQuantity.LuminousIntensity, "cd", "candela");

		public static readonly IEnumerable<SingleBaseUnit> BaseUnits =
			new List<SingleBaseUnit> {Metre, Kilogram, Second, Ampere, Kelvin, Mole, Candela};

		public override IEnumerable<SingleUnitPower> UnitPowers { get; }

		public BaseQuantity BaseQuantity { get; }
		public ISingleBase SingleBase { get; }

		private SingleBaseUnit(BaseQuantity _quantity, string _symbol, string _publicKey) : base(_quantity,
			_symbol,
			_publicKey)
		{
			UnitPowers = new[] {new BaseSingleUnitPower(this, 1)};
			BaseQuantity = _quantity;
			SingleBase = this;
		}

		public int CompareTo(SingleBaseUnit _other)
		{
			var quantityComparison = BaseQuantity.CompareTo(_other.BaseQuantity);
			if (quantityComparison != 0) return quantityComparison;
			var symbolComparison = string.Compare(Symbol, _other.Symbol, StringComparison.Ordinal);
			if (symbolComparison != 0) return symbolComparison;
			return string.Compare(PublicKey, _other.PublicKey, StringComparison.Ordinal);
		}

		public override IUnit Invert()
		{
			return MyInvert();
		}

		public override IBase InvertBase()
		{
			return MyInvert();
		}

		private CompoundBaseUnit MyInvert()
		{
			return new CompoundBaseUnit(new[] {new BaseSingleUnitPower(this, -1)},
				Quantity.Invert(),
				Symbol + "^-1",
				PublicKey + "^-1");
		}
	}

	public class ProportionalDerivedUnit : Unit, ISingle
	{
		public override IEnumerable<SingleUnitPower> UnitPowers { get; }
		public override IBase Base { get; }
		public BaseQuantity BaseQuantity { get; }
		public ISingleBase SingleBase { get; }

		public ProportionalDerivedUnit(SingleBaseUnit _singleBaseUnit, string _symbol, Fixed _ratio, string _publicKey) :
			base(
				_singleBaseUnit.Quantity,
				_symbol,
				_publicKey,
				_ratio)
		{
			UnitPowers = new[] {new SingleUnitPower(this, 1)};
			Base = _singleBaseUnit;
			BaseQuantity = _singleBaseUnit.BaseQuantity;
			SingleBase = _singleBaseUnit;
		}

		protected override Fixed ConvertToBaseUnit(Fixed _valueInCurrentUnit)
		{
			return _valueInCurrentUnit * Ratio;
		}

		protected override Fixed ConvertFromBaseUnit(Fixed _valueInBaseUnit)
		{
			return _valueInBaseUnit / Ratio;
		}

		public override IUnit Invert()
		{
			var invertedUnits = new[] {new SingleUnitPower(this, -1)};
			return new CompoundProportionalUnit(SingleBase.InvertBase(),
				invertedUnits,
				Quantity.Invert(),
				Symbol + "^-1",
				PublicKey + "^-1",
				Ratio);
		}
	}

//	public class ComplexDerivedUnit : Unit
//	{
//		public override IEnumerable<UnitPower> UnitPowers { get; }
//		private readonly Func<Fixed, Fixed> toBase;
//		private readonly Func<Fixed, Fixed> fromBase;
//
//		public ComplexDerivedUnit(BaseUnit _baseUnit,
//			string _symbol,
//			string _publicKey,
//			Func<Fixed, Fixed> _toBase,
//			Func<Fixed, Fixed> _fromBase) :
//			base(_baseUnit.Quantity, _symbol, _publicKey)
//		{
//			UnitPowers = new[] {new UnitPower(_baseUnit, 1)};
//			toBase = _toBase;
//			fromBase = _fromBase;
//		}
//
//		protected override Fixed ConvertToBaseUnit(Fixed _valueInCurrentUnit)
//		{
//			return toBase(_valueInCurrentUnit);
//		}
//
//		protected override Fixed ConvertFromBaseUnit(Fixed _valueInBaseUnit)
//		{
//			return fromBase(_valueInBaseUnit);
//		}
//	}

	public class CompoundBaseUnit : BaseUnit
	{
		public override IEnumerable<SingleUnitPower> UnitPowers { get; }
		public override IBase Base => this;

		public CompoundBaseUnit(IEnumerable<SingleUnitPower> _units,
			Quantity _compoundQuantity,
			string _compoundSymbol,
			string _compoundPublicKey) : base(
			_compoundQuantity,
			_compoundSymbol,
			_compoundPublicKey
		)
		{
			UnitPowers = _units;
		}

		protected override Fixed ConvertToBaseUnit(Fixed _valueInCurrentUnit)
		{
			return _valueInCurrentUnit;
		}

		protected override Fixed ConvertFromBaseUnit(Fixed _valueInBaseUnit)
		{
			return _valueInBaseUnit;
		}

		public override IUnit Invert()
		{
			return MyInvert();
		}

		public override IBase InvertBase()
		{
			return MyInvert();
		}

		private CompoundBaseUnit MyInvert()
		{
			var invertedUnits = UnitPowers.Select(_power => new SingleUnitPower(_power.Unit, -_power.Exponent)).ToList();
			return new CompoundBaseUnit(invertedUnits,
				Quantity.Invert(),
				CreateSymbol(invertedUnits),
				CreatePublicKey(invertedUnits));
		}
	}

	public class CompoundProportionalUnit : Unit
	{
		public override IEnumerable<SingleUnitPower> UnitPowers { get; }
		public override IBase Base { get; }

		public CompoundProportionalUnit(IBase _baseUnit,
			IEnumerable<SingleUnitPower> _parts,
			Quantity _compoundQuantity,
			string _compoundSymbol,
			string _compoundPublicKey,
			Fixed _ratio) : base(_compoundQuantity, _compoundSymbol, _compoundPublicKey, _ratio)
		{
			UnitPowers = _parts;
			Base = _baseUnit;
		}

		protected override Fixed ConvertToBaseUnit(Fixed _valueInCurrentUnit)
		{
			return _valueInCurrentUnit * Ratio;
		}

		protected override Fixed ConvertFromBaseUnit(Fixed _valueInBaseUnit)
		{
			return _valueInBaseUnit / Ratio;
		}

		public override IUnit Invert()
		{
			var invertedUnits = UnitPowers.Select(_power => new SingleUnitPower(_power.Unit, -_power.Exponent)).ToList();
			return new CompoundProportionalUnit(Base.InvertBase(),
				invertedUnits,
				Quantity.Invert(),
				CreateSymbol(invertedUnits),
				CreatePublicKey(invertedUnits),
				Ratio);
		}
	}
}