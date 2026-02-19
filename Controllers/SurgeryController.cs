using Microsoft.AspNetCore.Mvc;
using System;
using CureWellHospital.Models;
using CureWellHospital.Interfaces;

namespace CureWellHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurgeryController : ControllerBase
    {
        private readonly ICureWellHospitalRepository _repository;

        public SurgeryController(ICureWellHospitalRepository repository)
        {
            _repository = repository;
        }

        // POST api/Surgery/AddSurgeryDetails
        [HttpPost]
        [Route("AddSurgeryDetails")]
        public IActionResult AddSurgeryDetails([FromBody] Surgery surgery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int newSurgeryId;
                int dalReturnCode = _repository.AddSurgeryDetails(
                    surgery.DoctorId,
                    surgery.SurgeryDate,
                    surgery.StartTime,
                    surgery.EndTime,
                    surgery.SurgeryCategory,
                    out newSurgeryId);

                string message = dalReturnCode == 1
                    ? $"Successful addition operation! with SurgeryId {newSurgeryId}"
                    : "Unsuccessful addition operation!!";

                return new JsonResult(new { Message = message });
            }
            catch (Exception)
            {
                return new JsonResult(new { Message = "Some error occured, please try again!!!" });
            }
        }

        // PUT api/Surgery/UpdateSurgeryTime
        [HttpPut]
        [Route("UpdateSurgeryTime")]
        public IActionResult UpdateSurgeryTime([FromBody] Surgery surgery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int dalReturnCode = _repository.UpdateSurgeryTime(
                    surgery.SurgeryId,
                    surgery.StartTime,
                    surgery.EndTime);

                return Ok(dalReturnCode);
            }
            catch (Exception)
            {
                return StatusCode(500, -98);
            }
        }
    }
}
