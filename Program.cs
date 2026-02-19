using CureWellHospital.Interfaces;
using CureWellHospital;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register repository for dependency injection (uses IConfiguration for connection string)
builder.Services.AddScoped<ICureWellHospitalRepository, CureWellHospitalRepository>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================================
// SAMPLE DATA TEST – runs once on startup in Development mode
// ============================================================
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var repo = scope.ServiceProvider.GetRequiredService<ICureWellHospitalRepository>();

    Console.WriteLine("==============================================");
    Console.WriteLine("   CureWell Hospital – Sample Data Tests");
    Console.WriteLine("==============================================");

    // ----------------------------------------------------------
    // TEST 1: AddSurgeryDetails – valid data (should succeed)
    // ----------------------------------------------------------
    Console.WriteLine("\n[TEST 1] AddSurgeryDetails – Valid Data");
    int returnCode1 = repo.AddSurgeryDetails(
        doctorId        : 1,
        surgeryDate     : new DateTime(2025, 8, 15),
        startTime       : 9,
        endTime         : 11,
        surgeryCategory : "GEN",
        surgeryId       : out int newSurgeryId1
    );
    Console.WriteLine($"  Return Code : {returnCode1}");
    Console.WriteLine($"  Surgery Id  : {newSurgeryId1}");
    Console.WriteLine(returnCode1 == 1
        ? $"  Result      : SUCCESS – SurgeryId {newSurgeryId1} created"
        : $"  Result      : FAILED  – return code {returnCode1}");

    // ----------------------------------------------------------
    // TEST 2: AddSurgeryDetails – same doctor, overlapping slot
    //         (should return -2 = time-slot conflict)
    // ----------------------------------------------------------
    Console.WriteLine("\n[TEST 2] AddSurgeryDetails – Overlapping Time Slot");
    int returnCode2 = repo.AddSurgeryDetails(
        doctorId        : 1,
        surgeryDate     : new DateTime(2025, 8, 15),
        startTime       : 10,   // overlaps with TEST 1 (9-11)
        endTime         : 12,
        surgeryCategory : "ORT",
        surgeryId       : out int newSurgeryId2
    );
    Console.WriteLine($"  Return Code : {returnCode2}");
    Console.WriteLine($"  Surgery Id  : {newSurgeryId2}");
    Console.WriteLine(returnCode2 == -2
        ? "  Result      : EXPECTED CONFLICT detected (-2)"
        : $"  Result      : Unexpected return code {returnCode2}");

    // ----------------------------------------------------------
    // TEST 3: AddSurgeryDetails – different doctor, same slot
    //         (should succeed – no conflict across doctors)
    // ----------------------------------------------------------
    Console.WriteLine("\n[TEST 3] AddSurgeryDetails – Different Doctor, Same Slot");
    int returnCode3 = repo.AddSurgeryDetails(
        doctorId        : 2,
        surgeryDate     : new DateTime(2025, 8, 15),
        startTime       : 9,
        endTime         : 11,
        surgeryCategory : "CAR",
        surgeryId       : out int newSurgeryId3
    );
    Console.WriteLine($"  Return Code : {returnCode3}");
    Console.WriteLine($"  Surgery Id  : {newSurgeryId3}");
    Console.WriteLine(returnCode3 == 1
        ? $"  Result      : SUCCESS – SurgeryId {newSurgeryId3} created"
        : $"  Result      : FAILED  – return code {returnCode3}");

    // ----------------------------------------------------------
    // TEST 4: UpdateSurgeryTime – valid update on Test 1's record
    // ----------------------------------------------------------
    if (newSurgeryId1 > 0)
    {
        Console.WriteLine($"\n[TEST 4] UpdateSurgeryTime – Valid Update on SurgeryId {newSurgeryId1}");
        int returnCode4 = repo.UpdateSurgeryTime(
            surgeryId : newSurgeryId1,
            startTime : 14,
            endTime   : 16
        );
        Console.WriteLine($"  Return Code : {returnCode4}");
        Console.WriteLine(returnCode4 == 1
            ? "  Result      : SUCCESS – Time updated to 14:00–16:00"
            : $"  Result      : FAILED  – return code {returnCode4}");
    }

    // ----------------------------------------------------------
    // TEST 5: UpdateSurgeryTime – non-existent SurgeryId
    //         (should return -1 = not found)
    // ----------------------------------------------------------
    Console.WriteLine("\n[TEST 5] UpdateSurgeryTime – Non-existent SurgeryId (99999)");
    int returnCode5 = repo.UpdateSurgeryTime(
        surgeryId : 99999,
        startTime : 8,
        endTime   : 10
    );
    Console.WriteLine($"  Return Code : {returnCode5}");
    Console.WriteLine(returnCode5 == -1
        ? "  Result      : EXPECTED NOT FOUND (-1)"
        : $"  Result      : Unexpected return code {returnCode5}");

    Console.WriteLine("\n==============================================");
    Console.WriteLine("   Tests Complete – Starting Web API...");
    Console.WriteLine("==============================================\n");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map API controllers
app.MapControllers();

app.Run();
