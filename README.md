# CureWell Hospital – Surgery Web API

A .NET 8 Web API for managing hospital surgery records, backed by SQL Server stored procedures via Entity Framework Core raw SQL execution.

---

## Project Structure

```
GakdigiCureWellHospital/
├── Controllers/
│   ├── HomeController.cs
│   └── SurgeryController.cs          ← POST AddSurgeryDetails | PUT UpdateSurgeryTime
├── Data/
│   └── CureWellHospitalDbContext.cs  ← EF Core DbContext
├── Interfaces/
│   └── ICureWellHospitalRepository.cs
├── Models/
│   ├── ErrorViewModel.cs
│   └── Surgery.cs                    ← Model with Data Annotations
├── CureWellHospitalRepository.cs     ← DAL: calls SPs via EF Core ExecuteSqlRaw
├── StoredProcedures.sql              ← SQL Server stored procedures (run once on DB)
├── Program.cs                        ← DI registrations, middleware pipeline
├── appsettings.json                  ← Connection string configuration
└── CureWellHospital.csproj           ← NuGet packages
```

---

## Setup

### 1. Configure the connection string
Edit `appsettings.json` and replace `YOUR_SERVER` with your SQL Server instance:
```json
"ConnectionStrings": {
  "CureWellHospitalDB": "Server=YOUR_SERVER;Database=CureWellHospitalDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2. Run the SQL script
Open `StoredProcedures.sql` in SSMS or Azure Data Studio and execute it against your database. This creates the `Surgery` table, `usp_AddSurgeryDetails`, and `usp_UpdateSurgeryTime`.

### 3. Run the application
```bash
dotnet run
```
Swagger UI: `https://localhost:{port}/swagger`

---

## API Endpoints

### POST `/api/Surgery/AddSurgeryDetails`
**Request:**
```json
{ "doctorId": 101, "surgeryDate": "2025-08-15T09:00:00", "startTime": 9, "endTime": 11, "surgeryCategory": "GEN" }
```
**Responses:** `{ "Message": "Successful addition operation! with SurgeryId 42" }` or `{ "Message": "Unsuccessful addition operation!!" }`

### PUT `/api/Surgery/UpdateSurgeryTime`
**Request:**
```json
{ "surgeryId": 42, "startTime": 10, "endTime": 13 }
```
**Response:** Returns integer return code (`1` success, `-98` on exception).

---

## Stored Procedure Return Codes
| Code | Meaning |
|------|---------|
| `1` | Success |
| `-1` | Record not found |
| `-2` | Time-slot conflict |
| `-98` | Unhandled exception (DAL catch block) |

---

## Push to GitHub
```bash
git add .
git commit -m "feat: Add Surgery API module - Controller, Model, Repository, Interface, SP"
git push origin main
```
