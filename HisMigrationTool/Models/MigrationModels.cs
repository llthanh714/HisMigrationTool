using System;

namespace HisMigrationTool.Models
{
    // Đối tượng chứa dữ liệu được query từ DB cũ (ArcusAir)
    public class PatientMigrationDto
    {
        // Thông tin cơ bản từ bảng Patients (p)
        public string MRN { get; set; } = string.Empty;
        public string? FullName { get; set; } // Mapping từ p.firstname
        public DateTime? DateOfBirth { get; set; }
        public bool? IsVIP { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public bool? IsMCReg { get; set; }
        public string PassportNo { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Thông tin từ Reference Values (valuedescription)
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Race { get; set; }
        public string Occupation { get; set; }
        public string BloodGroup { get; set; }
        public string RhFactor { get; set; }
        public string Religion { get; set; }
        public string PatientType { get; set; }

        // Thông tin từ Organisations (o)
        public string OrganisationName { get; set; }
        public string OrgCode { get; set; }

        // Thông tin từ Patients_Address (pa) - Lấy qua hàm MIN
        public string Address { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        // Thông tin từ Patients_Contact (pc) - Lấy qua hàm MIN
        public string MobilePhone { get; set; }
        public string EmailID { get; set; }
        public string HomePhone { get; set; }
    }

    // Đối tượng trả về kết quả thao tác cho giao diện UI
    public class MigrationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}