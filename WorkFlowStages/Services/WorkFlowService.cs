using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;
using WorkFlowStages.Dto;
using WorkFlowStages.Model;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkFlowStages.Services
{
    public class WorkFlowService
    {
        private readonly Data.AppContext appDbContext;
        private readonly string _jsonFilePath = "./data.json";

        public WorkFlowService(Data.AppContext _appDbContext)
        {
            appDbContext = _appDbContext;
        }




        public async Task<ResponceModel<List<WorkFlowDto>>> getAll()
        {
            var data = await appDbContext.workflowstages.ToListAsync();
            var workFlowDto = new List<WorkFlowDto>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    workFlowDto.Add(new WorkFlowDto
                    {
                        dependent_on = item.dependent_on,
                        template_id = item.template_id,
                        description = item.description,
                        name = item.name,   
                        parallel = item.parallel,
                        sequence_number = item.sequence_number,
                        stage_type = item.stage_type
                    });
                }
            }

            return new ResponceModel<List<WorkFlowDto>>
            {
                Code = 200,
                Message = "OK",
                Data = workFlowDto
            };
        }



        public async Task<ResponceModel> saveStage(WorkflowData workflow)
        {
            try
            {

                string workFlowData = JsonSerializer.Serialize(workflow);
                if (workFlowData != null)
                {
                    addJsonIntoDb(workFlowData);
                }

               // string jsonData = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
               // WorkflowData workflow = JsonSerializer.Deserialize<WorkflowData>(jsonData);
                var stages = new List<WorkFlowStages.Model.WorkFlowStages>();
                if (workflow.initialEdges != null && workflow.initialNodes != null)
                {
                    int templateId = 1;
                    int sequenceNumber = 1;
                    foreach (var node in workflow.initialNodes)
                    {
                        int stageId = int.Parse(node.id);
                        string name = node.data?.label?.ToString();
                        string description = name;
                        int parallel = 0;
                        string stageType = node.type?.ToString() ?? "default";

                        stages.Add(new WorkFlowStages.Model.WorkFlowStages
                        {
                            stage_id = stageId,
                            template_id = templateId,
                            sequence_number = sequenceNumber++,
                            name = name,
                            description = description,
                            parallel = parallel,
                            stage_type = stageType,
                            dependent_on = GetDependenton(stageId.ToString(), workflow.initialEdges)
                        });
                    }
                }
                if (stages != null)
                {
                    foreach (var stage in stages)
                    {
                        appDbContext.workflowstages.Add(stage);
                    }
                    await appDbContext.SaveChangesAsync();
                }

                return new ResponceModel
                {
                    Code = 200,
                    Message = "Workflow data saved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponceModel
                {
                    Code = 500,
                    Message = "Error saving workflow data : " + ex.Message
                };   
            }
        }


        public async Task<ResponceModel> updateStage(WorkflowData workflowData)
        {
            try
            {
                return new ResponceModel
                {
                    Code = 200,
                    Message = "Workflow data updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponceModel
                {
                    Code = 500,
                    Message = "Error updating workflow data : " + ex.Message
                };
            }
        }


        public async Task<ResponceModel> deleteStage(int stageId)
        {
            try
            {
                if (stageId != null|| stageId != 0)
                {
                    return new ResponceModel
                    {
                        Code = 200,
                        Message = "Workflow data updated successfully"
                    };
                }
                else
                {
                    return new ResponceModel
                    {
                        Code = 500,
                        Message = "StageId should not be null or 0"
                    };
                }
               
            }
            catch (Exception ex)
            {
                return new ResponceModel
                {
                    Code = 500,
                    Message = "Error updating workflow data : " + ex.Message
                };
            }
        }

        public async Task<ResponceModel<List<WorkflowData>>> getAllJson()
        {
            var workflowData = new List<WorkflowData>();
            var data = await appDbContext.workflowstagesjson.ToListAsync();
            if(data.Count > 0 && data != null)
            {
                foreach (var item in data)
                {
                    WorkflowData workflow = JsonSerializer.Deserialize<WorkflowData>(item.jsondata);
                    workflowData.Add(workflow);
                }
                return new ResponceModel<List<WorkflowData>>
                {
                    Code = 200,
                    Message = "OK",
                    Data = workflowData
                };
            }
            else{
                return new ResponceModel<List<WorkflowData>>
                {
                    Code = 500,
                    Message = "No record present",
                    Data = workflowData
                };
            }
        }




        private int GetDependenton(string stageId, List<Edge> initialEdges)
        {
            if (initialEdges == null) { return 0; }
            int res = 0;
            foreach (var initialEdge in initialEdges)
            {

                if (initialEdge.source != null && initialEdge.target == stageId)
                {
                    res = int.Parse(initialEdge.source);
                }

            }
            return res;
        }

        private void addJsonIntoDb(string jsonData)
        {
            appDbContext.workflowstagesjson.Add(new WorkFlowStageJson
            {
                createdat  = DateTime.UtcNow, 
                createdby = "System",
                jsondata = jsonData
            });
            appDbContext.SaveChanges();
        }

    }
}
