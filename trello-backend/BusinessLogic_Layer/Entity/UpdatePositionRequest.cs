using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic_Layer.Entity
{
    public class UpdatePositionRequest
    {
        public Guid MoveId { get; set; }
        public Guid SpaceId { get; set; }
        public int NewPosition { get; set; }
    }
}
