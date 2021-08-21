using System;
using ModApi.Unit;
using NUnit.Framework;

namespace ModApiTest.Unit
{
	public class UnitTest
	{
//		[Test]
//		public void TestBasicUnit()
//		{
//			var kelvin = SingleBaseUnit.Kelvin;
//			var measurementFactory = new MeasurementFactory.Builder()
//				.Add(new ComplexDerivedUnit(kelvin, "°C", "celsius", _celcius => _celcius + 273.15, _kelvin => _kelvin - 273.15))
//				.Add(new ComplexDerivedUnit(kelvin, "°F", "fahrenheit", _fahrenheit => (_fahrenheit + 459.67) * 5f / 9f, _kelvin => _kelvin * 9f / 5f - 459.67))
//				.Build();
//
//			var celsius = measurementFactory.FindByPublicKey("celsius");
//			var fahrenheit = measurementFactory.FindByPublicKey("fahrenheit");
//
//			var boilingInKelvin = celsius.ConvertTo(100, kelvin);
//			var boilingInFahrenheit = celsius.ConvertTo(100, fahrenheit);
//			var backAgain = fahrenheit.ConvertTo(boilingInFahrenheit, celsius);
//
//			Console.WriteLine("100 " + celsius.Symbol + " = " + boilingInKelvin + " " + kelvin.Symbol);
//			Console.WriteLine("100 " + celsius.Symbol + " = " + boilingInFahrenheit + " " + fahrenheit.Symbol);
//			Console.WriteLine("100 " + celsius.Symbol + " = " + backAgain + " " + celsius.Symbol);
//		}

		[Test]
		public void TestUnits()
		{
			var measurementFactory = new MeasurementFactory.Builder()
				.Add(new ProportionalDerivedUnit(SingleBaseUnit.Metre, "km", 1000, "kilometre"))
				.Add(new ProportionalDerivedUnit(SingleBaseUnit.Second, "h", 3600, "hour"))
				.Build();

			var kilometre = measurementFactory.FindByPublicKey("kilometre");
			var hour = measurementFactory.FindByPublicKey("hour");
			var kilometrePerHour = kilometre / hour;
			var metrePerSecond = SingleBaseUnit.Metre / SingleBaseUnit.Second;

			Console.WriteLine(kilometrePerHour.Symbol + " : " + kilometrePerHour.PublicKey + " : " + kilometrePerHour.Ratio);
			Console.WriteLine(metrePerSecond.Symbol + " : " + metrePerSecond.PublicKey + " : " + metrePerSecond.Ratio);

			Console.WriteLine(metrePerSecond.ConvertTo(100, kilometrePerHour));
			Console.WriteLine(kilometrePerHour.ConvertTo(100, metrePerSecond));
		}
	}
}