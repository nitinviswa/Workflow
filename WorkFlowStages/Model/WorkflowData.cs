using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkFlowStages.Model
{
    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
    }
    
    public class NodeData
    {
        public string label { get; set; }
    }
    public class Node
    {
        public string id { get; set; }
        public string type { get; set; }        
        public NodeData data { get; set; }
        public string sourcePosition { get; set; }
        public string targetPosition { get; set; }        
        public Position position { get; set; }
    }

    public class Edge
    {

        public string id { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public string sourceHandle { get; set; }
        public string targetHandle { get; set; }
        public string type { get; set; }
        public bool? animated { get; set; }
        public string label { get; set; }
    }
    public class WorkflowData
    {

        public List<Node> initialNodes { get; set; }
        public List<Edge> initialEdges { get; set; }
    }

}
