-- ============================================================
-- CureWell Hospital – SQL Server Stored Procedures
-- Run this script against your CureWellHospitalDB database
-- ============================================================

-- ============================================================
-- Surgery Table (create if not already present)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Surgery')
BEGIN
    CREATE TABLE Surgery (
        SurgeryId       INT           IDENTITY(1,1) PRIMARY KEY,
        DoctorId        INT           NOT NULL,
        SurgeryDate     DATETIME      NOT NULL,
        StartTime       INT           NOT NULL CHECK (StartTime BETWEEN 1 AND 24),
        EndTime         INT           NOT NULL CHECK (EndTime   BETWEEN 1 AND 24),
        SurgeryCategory VARCHAR(3)    NOT NULL
    );
    PRINT 'Surgery table created.';
END
ELSE
    PRINT 'Surgery table already exists.';
GO

-- ============================================================
-- 1. usp_AddSurgeryDetails
--    Inserts a new surgery record.
--    Returns  1  on success
--    Returns -1  if DoctorId does not exist (optional guard)
--    Returns -2  if a time-slot conflict exists for that doctor
--    @SurgeryId OUTPUT returns the new primary-key value
-- ============================================================
IF OBJECT_ID('usp_AddSurgeryDetails', 'P') IS NOT NULL
    DROP PROCEDURE usp_AddSurgeryDetails;
GO

CREATE PROCEDURE usp_AddSurgeryDetails
    @DoctorId        INT,
    @SurgeryDate     DATETIME,
    @StartTime       INT,
    @EndTime         INT,
    @SurgeryCategory VARCHAR(3),
    @SurgeryId       INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Optional: validate DoctorId exists (uncomment when Doctor table is available)
    -- IF NOT EXISTS (SELECT 1 FROM Doctor WHERE DoctorId = @DoctorId)
    -- BEGIN
    --     SET @SurgeryId = 0;
    --     RETURN -1;
    -- END

    -- Check for overlapping surgery slot for the same doctor on the same date
    IF EXISTS (
        SELECT 1
        FROM   Surgery
        WHERE  DoctorId    = @DoctorId
          AND  SurgeryDate = @SurgeryDate
          AND  @StartTime  < EndTime
          AND  @EndTime    > StartTime
    )
    BEGIN
        SET @SurgeryId = 0;
        RETURN -2;   -- Time-slot conflict
    END

    -- Insert the new surgery record
    INSERT INTO Surgery (DoctorId, SurgeryDate, StartTime, EndTime, SurgeryCategory)
    VALUES (@DoctorId, @SurgeryDate, @StartTime, @EndTime, @SurgeryCategory);

    -- Return the newly generated identity value
    SET @SurgeryId = SCOPE_IDENTITY();

    RETURN 1;   -- Success
END
GO

-- ============================================================
-- 2. usp_UpdateSurgeryTime
--    Updates StartTime and EndTime for an existing surgery.
--    Returns  1  on success
--    Returns -1  if SurgeryId does not exist
--    Returns -2  if a time-slot conflict exists for that doctor
-- ============================================================
IF OBJECT_ID('usp_UpdateSurgeryTime', 'P') IS NOT NULL
    DROP PROCEDURE usp_UpdateSurgeryTime;
GO

CREATE PROCEDURE usp_UpdateSurgeryTime
    @SurgeryId  INT,
    @StartTime  INT,
    @EndTime    INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate: surgery must exist
    IF NOT EXISTS (SELECT 1 FROM Surgery WHERE SurgeryId = @SurgeryId)
    BEGIN
        RETURN -1;   -- Surgery not found
    END

    -- Get the doctor and date for the surgery being updated
    DECLARE @DoctorId    INT;
    DECLARE @SurgeryDate DATETIME;

    SELECT @DoctorId    = DoctorId,
           @SurgeryDate = SurgeryDate
    FROM   Surgery
    WHERE  SurgeryId = @SurgeryId;

    -- Check for time-slot conflict with OTHER surgeries for the same doctor/date
    IF EXISTS (
        SELECT 1
        FROM   Surgery
        WHERE  DoctorId    = @DoctorId
          AND  SurgeryDate = @SurgeryDate
          AND  SurgeryId  <> @SurgeryId       -- exclude the record being updated
          AND  @StartTime  < EndTime
          AND  @EndTime    > StartTime
    )
    BEGIN
        RETURN -2;   -- Time-slot conflict
    END

    -- Perform the update
    UPDATE Surgery
    SET    StartTime = @StartTime,
           EndTime   = @EndTime
    WHERE  SurgeryId = @SurgeryId;

    RETURN 1;   -- Success
END
GO

-- ============================================================
-- Smoke Test – Run manually in SSMS or Azure Data Studio
-- ============================================================
/*
DECLARE @NewId INT, @Ret INT;

-- Test AddSurgeryDetails
EXEC @Ret = usp_AddSurgeryDetails
    @DoctorId        = 101,
    @SurgeryDate     = '2025-08-15',
    @StartTime       = 9,
    @EndTime         = 11,
    @SurgeryCategory = 'GEN',
    @SurgeryId       = @NewId OUTPUT;

SELECT @Ret AS ReturnCode, @NewId AS NewSurgeryId;

-- Test UpdateSurgeryTime
EXEC @Ret = usp_UpdateSurgeryTime
    @SurgeryId = @NewId,
    @StartTime = 10,
    @EndTime   = 13;

SELECT @Ret AS ReturnCode;
*/
