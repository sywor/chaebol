using System.Collections;
using System.Collections.Generic;

namespace ModApi
{
	public interface IModInfo
	{
		string Id { get; }
		IEnumerable<string> Dependencies { get; }
	}
}