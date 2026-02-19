using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CureWellHospital
{
    public class CureWellHospitalRepository
    {

        // Assuming this code is within the CureWellHospitalRepository class
        public int AddSurgeryDetails(int doctorId, DateTime surgeryDate, int startTime, int endTime, string surgeryCategory, out int surgeryId)
        {
            // Initialize the out parameter to 0, as required for exceptions/failures.
            surgeryId = 0;

            // The stored procedure execution will be done via raw SQL.
            // The EXECUTE command captures the return value into @return_value.
            // The @SurgeryId parameter is marked as an OUTPUT parameter.
            var sql = "EXEC @return_value = usp_AddSurgeryDetails @DoctorId, @SurgeryDate, @StartTime, @EndTime, @SurgeryCategory, @SurgeryId OUT";

            try
            {
                // 1. Define all necessary SqlParameters

                var doctorIdParam = new SqlParameter("@DoctorId", doctorId);
                var surgeryDateParam = new SqlParameter("@SurgeryDate", surgeryDate);
                var startTimeParam = new SqlParameter("@StartTime", startTime);
                var endTimeParam = new SqlParameter("@EndTime", endTime);
                var surgeryCategoryParam = new SqlParameter("@SurgeryCategory", surgeryCategory);

                // 2. Define the OUTPUT parameter for the new ID
                var surgeryIdOutParam = new SqlParameter("@SurgeryId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    // The value is initialized to 0, as required.
                    Value = surgeryId
                };

                // 3. Define the Return Value parameter
                var returnValueParam = new SqlParameter("@return_value", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };

                // 4. Execute the stored procedure
                context.Database.ExecuteSqlRaw(
                    sql,
                    returnValueParam,
                    doctorIdParam,
                    surgeryDateParam,
                    startTimeParam,
                    endTimeParam,
                    surgeryCategoryParam,
                    surgeryIdOutParam // This is the output parameter
                );

                // 5. Retrieve the values and set the method's out parameter
                int returnValue = (int)returnValueParam.Value;

                // This sets the method's 'out int surgeryId' parameter with the value 
                // returned by the stored procedure's OUTPUT parameter.
                surgeryId = (int)surgeryIdOutParam.Value;

                // 6. Return the value returned by the stored procedure's RETURN statement
                return returnValue;
            }
            catch (Exception)
            {
                // In case of any exception, the requirements are:
                // 1. Return -98.
                // 2. 'surgeryId' should be initialized as 0 (which was done at the start 
                // of the method, but we ensure it here too).
                surgeryId = 0;
                return -98;
            }
        }


    }

}
