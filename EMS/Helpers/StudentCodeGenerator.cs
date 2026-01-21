using EMS.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.Helpers
{
    public static class StudentCodeGenerator
    {
        public static async Task<string> GenerateStudentCodeAsync(ApplicationDbContext db)
        {
            while (true)
            {
                var year = DateTime.UtcNow.Year;
                var code = $"STD-{year}-{RandomChunk(4)}"; // STD-2026-AB12

                var exists = await db.Students.AnyAsync(s => s.StudentCode == code);
                if (!exists) return code;
            }
        }

        private static string RandomChunk(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = new Random();
            return new string(Enumerable.Range(0, len).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }
    }
}
