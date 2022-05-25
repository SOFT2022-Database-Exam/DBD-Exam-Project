﻿using lib.DTO;
using lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrescriptionService.Util
{
    public class DtoMapper
    {
        public static PrescriptionDto ToDto(Prescription prescription)
            => new PrescriptionDto
            {
                Creation = prescription.Creation,
            Expiration = prescription.Expiration,
            Patient = new PatientDto
            {
                FirstName = prescription.PrescribedToNavigation.PersonalData.FirstName,
                LastName = prescription.PrescribedToNavigation.PersonalData.LastName,
                Email = prescription.PrescribedToNavigation.PersonalData.Email ?? "",
            },
            Medicine = new MedicineDto {
                Name = prescription.Medicine.Name
            }
        };

        public static PatientDto ToDto(Patient patient)
            => new PatientDto
            {
                Email = patient.PersonalData.Email ?? "",
                FirstName = patient.PersonalData.FirstName,
                LastName = patient.PersonalData.LastName,
                Id = patient.Id
            };

        public static PharmacyDto ToDto(Pharmacy pharmacy)
            => new PharmacyDto
            {
                Id = pharmacy.Id,
                Name = pharmacy.PharmacyName,
                Address = pharmacy.Address != null ? new AddressDto
                {
                    Id = pharmacy.Address.Id,
                    StreetName = pharmacy.Address.StreetName,
                    StreetNumber = pharmacy.Address.StreetNumber,
                    ZipCode = pharmacy.Address.ZipCode
                } : new(),
            };
    }
}
