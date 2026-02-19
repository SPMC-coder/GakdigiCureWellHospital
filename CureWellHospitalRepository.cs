using System;
using System.Data;
using Microsoft.Data.SqlClient;
using CureWellHospital.Data;
using CureWellHospital.Interfaces;

namespace CureWellHospital
{
    public class CureWellHospitalRepository : ICureWellHospitalRepository
    {
        private readonly CureWellHospitalDbContext _context;

        public CureWellHospitalRepository(CureWellHospitalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calls usp_AddSurgeryDetails stored procedure to insert a new surgery record.
        /// Returns 1 on success; -98 on any exception.
        /// The newly created SurgeryId is returned via the out parameter.
        /// </summary>
        public int AddSurgeryDetails(int doctorId, DateTime surgeryDate, int startTime, int endTime,
                                     string surgeryCategory, out int surgeryId)
        {
            // Initialize the out parameter to 0 (required for exceptions/failures)
            surgeryId = 0;

            // EXEC captures the SP RETURN value into @return_value
            // @SurgeryId is an OUTPUT parameter
            var sql = "EXEC @return_value = usp_AddSurgeryDetails @DoctorId, @SurgeryDate, @StartTime, @EndTime, @SurgeryCategory, @SurgeryId OUT";

            try
            {
                // 1. Define input SqlParameters
                var doctorIdParam        = new SqlParameter("@DoctorId",        doctorId);
                var surgeryDateParam     = new SqlParameter("@SurgeryDate",      surgeryDate);
                var startTimeParam       = new SqlParameter("@StartTime",        startTime);
                var endTimeParam         = new SqlParameter("@EndTime",          endTime);
                var surgeryCategoryParam = new SqlParameter("@SurgeryCategory",  surgeryCategory);

                // 2. OUTPUT parameter – receives the newly generated SurgeryId
                var surgeryIdOutParam = new SqlParameter("@SurgeryId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value     = surgeryId   // initialized to 0
                };

                // 3. ReturnValue parameter – captures the SP's RETURN statement value
                var returnValueParam = new SqlParameter("@return_value", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };

                // 4. Execute the stored procedure via EF Core raw SQL
                _context.Database.ExecuteSqlRaw(
                    sql,
                    returnValueParam,
                    doctorIdParam,
                    surgeryDateParam,
                    startTimeParam,
                    endTimeParam,
                    surgeryCategoryParam,
                    surgeryIdOutParam
                );

                // 5. Read back OUTPUT and RETURN values
                int returnValue = (int)returnValueParam.Value;
                surgeryId       = (int)surgeryIdOutParam.Value;

                // 6. Return the SP's RETURN value to the caller
                return returnValue;
            }
            catch (Exception)
            {
                // On any exception: surgeryId = 0, return -98
                surgeryId = 0;
                return -98;
            }
        }

        /// <summary>
        /// Calls usp_UpdateSurgeryTime stored procedure to update a surgery's time slot.
        /// Returns 1 on success; -98 on any exception.
        /// </summary>
        public int UpdateSurgeryTime(int surgeryId, int startTime, int endTime)
        {
            var sql = "EXEC @return_value = usp_UpdateSurgeryTime @SurgeryId, @StartTime, @EndTime";

            try
            {
                var surgeryIdParam  = new SqlParameter("@SurgeryId",  surgeryId);
                var startTimeParam  = new SqlParameter("@StartTime",  startTime);
                var endTimeParam    = new SqlParameter("@EndTime",    endTime);

                var returnValueParam = new SqlParameter("@return_value", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };

                _context.Database.ExecuteSqlRaw(
                    sql,
                    returnValueParam,
                    surgeryIdParam,
                    startTimeParam,
                    endTimeParam
                );

                return (int)returnValueParam.Value;
            }
            catch (Exception)
            {
                return -98;
            }
        }
    }
}
