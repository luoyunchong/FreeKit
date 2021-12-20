using FreeSql.Extensions.EfCoreFluentApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public interface IEntityTypeConfiguration<TEntity> where TEntity : class
{
    void Configure(EfCoreTableFluent<TEntity> model);
}

