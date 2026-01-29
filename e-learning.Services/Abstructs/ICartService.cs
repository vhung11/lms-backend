using e_learning.Data.Helpers;
using e_learning.Services.DTOs;
using Microsoft.AspNetCore.Http;

namespace e_learning.Services.Abstructs
{
    public interface ICartService
    {
        public Task<string> CheckoutAsync(int studentId, string ipAddress);
        Task<VnPayResultDto> ProcessVnPayReturnAsync(IQueryCollection query);

        public Task<CartDto> AddToCartAsync(int studentId, int courseId);
        Task<CartDto?> GetCartAsync(int studentId);
        Task RemoveFromCartAsync(int studentId, int courseId);
    }

}
