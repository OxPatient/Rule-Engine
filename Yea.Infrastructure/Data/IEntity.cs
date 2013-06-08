using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yea.Infrastructure.Data
{
    public interface IEntity
    {

    }

    public interface IGenericEntity<TIdentity> : IEquatable<IGenericEntity<TIdentity>>
    {
        TIdentity Id { get; set; }
    }
}
