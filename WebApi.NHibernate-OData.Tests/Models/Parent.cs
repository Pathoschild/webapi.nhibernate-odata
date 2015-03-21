using System.Collections.Generic;
using System.Diagnostics;

namespace WebApi.NHibernate_OData.Tests.Models
{
    [DebuggerDisplay("Child {ToString()}")]
    public class Parent
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual IList<Child> Children { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, Children: {2}", this.Id, this.Name, this.Children.Count);
        }
    }
}
