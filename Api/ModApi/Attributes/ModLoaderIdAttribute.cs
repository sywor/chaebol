using System;

namespace ModApi.Attributes
{
	public class ModLoaderIdAttribute : Attribute
	{
		public string Id { get; private set; }

		public ModLoaderIdAttribute(string _id)
		{
			Id = _id;
		}
	}
}