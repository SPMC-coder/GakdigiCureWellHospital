using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CureWellHospital.Interfaces;

namespace CureWellHospital
{
    public class CureWellHospitalRepository : ICureWellHospitalRepository
    {
        private readonly string _connectionString;

        public CureWellHospitalRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CureWellHospitalDB")!;
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

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("usp_AddSurgeryDetails", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.AddWithValue("@DoctorId",        doctorId);
                cmd.Parameters.AddWithValue("@SurgeryDate",     surgeryDate);
                cmd.Parameters.AddWithValue("@StartTime",       startTime);
                cmd.Parameters.AddWithValue("@EndTime",         endTime);
                cmd.Parameters.AddWithValue("@SurgeryCategory", surgeryCategory);

                // OUTPUT parameter - receives the newly generated SurgeryId
                SqlParameter surgeryIdOutParam = new SqlParameter("@SurgeryId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(surgeryIdOutParam);

                // ReturnValue parameter - captures the SP's RETURN statement value
                SqlParameter returnValueParam = new SqlParameter("@return_value", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(returnValueParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                // Read back OUTPUT and RETURN values
                surgeryId = (int)surgeryIdOutParam.Value;
                return (int)returnValueParam.Value;
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
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("usp_UpdateSurgeryTime", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.AddWithValue("@SurgeryId", surgeryId);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime",   endTime);

                // ReturnValue parameter - captures the SP's RETURN statement value
                SqlParameter returnValueParam = new SqlParameter("@return_value", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                cmd.Parameters.Add(returnValueParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                return (int)returnValueParam.Value;
            }
            catch (Exception)
            {
                return -98;
            }
        }
    }
}
