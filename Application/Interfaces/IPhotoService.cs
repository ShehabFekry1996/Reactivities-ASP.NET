using Application.Profiles.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPhotoService
    {
        Task<PhotoUploadResult?> UploadPhotoAsync(IFormFile file);

        Task<string> DeletePhotoAsync(string publicId);
    }
}
