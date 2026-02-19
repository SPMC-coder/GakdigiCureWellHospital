using System;

namespace CureWellHospital.Interfaces
{
    public interface ICureWellHospitalRepository
    {
        /// <summary>
        /// Adds a new surgery record via stored procedure.
        /// </summary>
        /// <param name="surgeryId">OUTPUT: the newly generated Surgery ID (0 on failure)</param>
        /// <returns>1 on success; -98 on exception; other negative codes from SP on business-rule failure</returns>
        int AddSurgeryDetails(int doctorId, DateTime surgeryDate, int startTime, int endTime,
                              string surgeryCategory, out int surgeryId);

        /// <summary>
        /// Updates StartTime and EndTime of an existing surgery record.
        /// </summary>
        /// <returns>1 on success; -98 on exception; other negative codes from SP on business-rule failure</returns>
        int UpdateSurgeryTime(int surgeryId, int startTime, int endTime);
    }
}
