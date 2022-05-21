using ConsultationService.Entities;
using lib.DTO;

namespace ConsultationService.Util
{
    public static class ConsultationMapper
    {
        public static ConsultationDto ToDto(ConsultationEntity entity){
            var dto = new ConsultationDto(){
                Id = entity.ConsultationId,
                DoctorId = entity.DoctorId,
                PatientId = entity.PatientId,
                Regarding = entity.Regarding

            };
            return dto;
        }

        public static ConsultationEntity FromDto(ConsultationDto dto){
            var entity = new ConsultationEntity(){
                DoctorId = dto.DoctorId,
                ConsultationId = dto.Id,
                PatientId = dto.PatientId,
                Regarding = dto.Regarding
            };
            return entity;
        }

        public static ConsultationEntity FromDto(ConsultationBookingRequestDto dto){
            var entity = new ConsultationEntity(){
                ConsultationId = dto.Id,
                PatientId = dto.PatientId,
                Regarding = dto.Regarding
            };
            return entity;
        }

        public static ConsultationEntity FromDto(ConsultationCreationDto dto){
            var entity = new ConsultationEntity(){
                ConsultationStartUtc = dto.ConsultationStartUtc,
                DoctorId = dto.DoctorId
            };
            return entity;
        }
    }
}