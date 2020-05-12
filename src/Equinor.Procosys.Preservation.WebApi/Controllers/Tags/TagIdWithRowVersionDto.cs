using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class TagIdWithRowVersionDto
    {
        public int Id { get; set; }
        public string RowVersion { get; set; }
    }
}
