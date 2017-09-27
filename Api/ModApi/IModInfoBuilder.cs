using System.Collections.Generic;

namespace ModApi
{
	public interface IModInfoBuilder
	{
		IModInfoBuilder WithId(string _id);
		IModInfoBuilder WithDependencies(IEnumerable<string> _dependencies);
		IModInfo Register();
	}
}