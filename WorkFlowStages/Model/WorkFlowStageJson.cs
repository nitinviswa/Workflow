using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace WorkFlowStages.Model
{
    public class WorkFlowStageJson
    {

        public int id { get; set; }
        [Column(TypeName = "jsonb")]
        public string jsondata { get; set; }
        public string createdby { get; set; }
        public DateTime createdat { get; set; }
    }
}
