using System;

namespace DT.FileSystem.Data.Exceptions
{
	/// <summary>
	/// thrown when attempting to save a record when one already exists
	/// i.e POSTing a vehicle location when one already exists for the vehicle
	/// </summary>
	[Serializable]
	public class DuplicateEntityException : Exception
	{
		public DuplicateEntityException() { }
		public DuplicateEntityException(string message) : base(message) { }
		public DuplicateEntityException(string message, Exception inner) : base(message, inner) { }
		protected DuplicateEntityException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
