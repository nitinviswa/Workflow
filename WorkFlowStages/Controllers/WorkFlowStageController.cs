using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WorkFlowStages.Model;
using WorkFlowStages.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkFlowStages.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    public class WorkFlowStageController : ControllerBase
    {

        private readonly ILogger<WorkFlowStageController> _logger;
        private readonly WorkFlowService workFlowService;

        public WorkFlowStageController(ILogger<WorkFlowStageController> logger, WorkFlowService service)
        {
            _logger = logger;
            workFlowService = service;
        }

        [HttpGet]
        public async Task<ActionResult<ResponceModel>> Get()
        {
            return Ok(await workFlowService.getAll());
        }

        [HttpPost(Name = "Stage")]
        public async Task<ActionResult<ResponceModel>> PostWorkFlowStage(WorkflowData workflow)
        {
            return Ok(await workFlowService.saveStage(workflow));
        }

        [HttpPut]
        public async Task<ActionResult<ResponceModel>> UpdateWorkFlowStage(WorkflowData workflowData)
        {
            return Ok(await workFlowService.updateStage(workflowData));
        }

        [HttpDelete]
        public async Task<ActionResult<ResponceModel>> DeleteWorkFlowStage(int stageId)
        {
            return Ok(await workFlowService.deleteStage(stageId));
        }

        [HttpGet("getJson")]
        public async Task<ActionResult<ResponceModel>> GetJson()
        {
            return Ok(await workFlowService.getAllJson());
        }
    }
 }
