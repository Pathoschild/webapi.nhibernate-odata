using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.NHibernate_OData.Tests.Models
{
    public class TestData
    {
        public static IQueryable<Child> GetTestChildren()
        {
            return new[] { new Child() { Id = 11, Parent = new Parent() { Id = 61 } } }.AsQueryable();
        }
    }
}
