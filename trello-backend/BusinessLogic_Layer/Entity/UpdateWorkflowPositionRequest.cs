using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic_Layer.Entity
{
    public class UpdateWorkflowPositionRequest
    {
        public Guid WorkflowId { get; set; }
        public Guid BoardId { get; set; }
        public int NewPosition { get; set; }
    }
}
