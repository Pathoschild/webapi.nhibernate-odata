using System.Diagnostics;

namespace Pathoschild.WebApi.NhibernateOdata.Tests.Models
{
	[DebuggerDisplay("Child {ToString()}")]
	public class Child
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Parent Parent { get; set; }

		public override string ToString()
		{
			return string.Format("Id: {0}, Name: {1}, Parent: {2}", this.Id, this.Name, this.Parent);
		}
	}
}
