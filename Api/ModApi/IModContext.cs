using System.Collections.Generic;

namespace ModApi
{
	public interface IModContext
	{
		TBuilder GetBuilder<TBuilder, TPlaceable>()
			where TBuilder : IBuilder<TPlaceable> where TPlaceable : IModObject;

		IEnumerable<IResource> GetResources();
	}
}