using lib.DTO;
namespace ConsultationService.Services
{
    public interface IConsultationService
    {
        public ConsultationDto GetConsultation(string id);
        public Task<ConsultationDto> GetConsultationAsync(string id);
        public IEnumerable<ConsultationDto> GetConsultationsForPatient(string patientId);
        public Task<IEnumerable<ConsultationDto>> GetConsultationsForPatientAsync(string patientId);
        public IEnumerable<ConsultationDto> GetConsultationsForDoctor(string doctorId);
        public Task<IEnumerable<ConsultationDto>> GetConsultationsForDoctorAsync(string doctorId);
        public Task<ConsultationDto> CreateConsultationAsync(ConsultationCreationDto consultationDto);
        public ConsultationDto UpdateConsultation(ConsultationDto consultationDto);
        public Task<ConsultationDto> UpdateConsultationAsync(ConsultationDto consultationDto);
        public ConsultationDto BookConsultation(ConsultationBookingRequestDto consultationDto);
        public Task<ConsultationDto> BookConsultationAsync(ConsultationBookingRequestDto consultationDto);
    }
}