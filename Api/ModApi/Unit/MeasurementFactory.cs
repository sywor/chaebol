using System;
using System.Collections.Generic;
using ModApi.Math;

namespace ModApi.Unit
{
	public class MeasurementFactory
	{
		private readonly Dictionary<string, Unit> fromSymbol = new Dictionary<string, Unit>();
		private readonly Dictionary<string, Unit> fromPublicKey = new Dictionary<string, Unit>();

		private MeasurementFactory(IEnumerable<Unit> _units)
		{
			foreach (var baseUnit in SingleBaseUnit.BaseUnits)
			{
				fromSymbol.Add(baseUnit.Symbol, baseUnit);
				fromPublicKey.Add(baseUnit.PublicKey, baseUnit);
			}
			
			foreach (var unit in _units)
			{
				if (fromSymbol.ContainsKey(unit.Symbol))
				{
					throw new ArgumentException("Duplicate unit symbol: " + unit.Symbol);
				}

				if (fromPublicKey.ContainsKey(unit.PublicKey))
				{
					throw new ArgumentException("Duplicate unit public key: " + unit.PublicKey);
				}

				fromSymbol.Add(unit.Symbol, unit);
				fromPublicKey.Add(unit.PublicKey, unit);
			}
		}

		public Unit FindBySymbol(string _symbol)
		{
			return fromSymbol[_symbol];
		}

		public Unit FindByPublicKey(string _publicKey)
		{
			return fromPublicKey[_publicKey];
		}

		public class Builder
		{
			private readonly List<Unit> units = new List<Unit>();

			public Builder Add(Unit _unit)
			{
				units.Add(_unit);
				return this;
			}

			public Builder AddRange(IEnumerable<Unit> _units)
			{
				units.AddRange(_units);
				return this;
			}

			public MeasurementFactory Build()
			{
				return new MeasurementFactory(units);
			}
		}
	}
}